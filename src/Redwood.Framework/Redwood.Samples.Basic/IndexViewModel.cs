using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Redwood.Framework.Controls;
using Redwood.Framework.ViewModel;

namespace Redwood.Samples.Basic
{
    public class IndexViewModel : ViewModelBase
    {

        public string NewTaskText { get; set; }

        public List<Task> Tasks { get; set; }

        protected override async System.Threading.Tasks.Task Load(IOwinContext context, bool isPostBack)
        {
            if (!isPostBack)
            {
                Tasks = new List<Task>()
                {
                    new Task() { Title = "Defaultní úkol 1"},
                    new Task() { Title = "Defaultní úkol 2"}
                };
            }
        }

        public void AddTask(RedwoodEventArgs args)
        {
            Tasks.Add(new Task() { Title = NewTaskText });
            NewTaskText = string.Empty;
        }

        public void FinishTask(RedwoodEventArgs args)
        {
            Tasks.RemoveAt(Convert.ToInt32(args.Parameters[0]));
        }

    }

    public class Task
    {
        public string Title { get; set; }
    }
}