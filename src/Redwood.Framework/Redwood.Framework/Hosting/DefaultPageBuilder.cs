using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Redwood.Framework.Controls;
using Redwood.Framework.Parsing;

namespace Redwood.Framework.Hosting
{
    public class DefaultPageBuilder : IPageBuilder
    {

        public Page BuildPage(RedwoodRequestContext context, MarkupFile markupFile)
        {
            try
            {
                var serializer = new RwHtml.RwHtmlSerializer();
                var result = serializer.LoadFromString(markupFile.Contents);
                return (Page)result;
            }
            catch (ParserException ex)
            {
                // add the file name to the exception and rethrow
                ex.FileName = markupFile.FileName;
                throw;
            }
        }
    }
}