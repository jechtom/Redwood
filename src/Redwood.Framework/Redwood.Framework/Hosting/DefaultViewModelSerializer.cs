using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Redwood.Framework.Controls;
using Redwood.Framework.ViewModel;

namespace Redwood.Framework.Hosting
{
    public class DefaultViewModelSerializer : IViewModelSerializer
    {
        public string SerializeViewModel(ViewModelBase viewModel)
        {
            return JsonConvert.SerializeObject(viewModel);
        }

        public void DeseralizePostData(string data, ViewModelBase target, out Action invokedCommand)
        {
            // deserialize the changes into the view model
            var obj = JObject.Parse(data);
            var cvtr = new JsonSerializer();
            cvtr.Populate(obj["viewModel"].CreateReader(), target);
            
            // determine command info
            var commandName = obj["commandName"].Value<string>();
            var commandTarget = obj["commandTarget"].Value<string>();
            var commandArguments = cvtr.Deserialize<object[]>(obj["commandArguments"].CreateReader());

            // locate the method info
            invokedCommand = ResolveCommand(target, commandName, commandTarget, commandArguments);
        }

        /// <summary>
        /// Resolves the command.
        /// </summary>
        public Action ResolveCommand(object target, string commandName, string commandPath, object[] commandArguments)
        {
            var root = target;

            var parts = commandPath.Split('.');
            if (parts[0] != "$root")
            {
                throw new ArgumentException("The command path must start with '$root'!");
            }

            // resolve path
            for (var i = 1; i < parts.Length; i++)
            {
                var match = Regex.Match(parts[i], @"^([a-zA-Z_][a-zA-Z0-9_]+)(\[([0-9]+)\])?$");
                if (!match.Success)
                {
                    throw new ArgumentException(string.Format("Invalid command path fragment '{0}'!", parts[i]));    
                }

                var property = target.GetType().GetProperty(match.Groups[1].Value);
                target = property.GetValue(target);

                if (match.Groups[3].Captures.Count == 1)
                {
                    var index = int.Parse(match.Groups[3].Captures[0].Value);
                    if (target.GetType().IsArray)
                    {
                        target = ((object[])target)[index];
                    }
                    else 
                    {
                        target = target.GetType().GetProperty("Item").GetValue(target, new object[] { index });
                    }
                }
            }

            // resolve command name
            var method = target.GetType().GetMethod(commandName, new[] { typeof (RedwoodEventArgs) });
            var args = new RedwoodEventArgs()
            {
                CommandName = commandName,
                Root = (ViewModelBase)root,
                Target = target,
                Parameters = commandArguments
            };
            return () =>
            {
                method.Invoke(target, new object[] { args });
            };
        }
    }
}