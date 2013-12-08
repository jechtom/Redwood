using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.Parsing.ViewModel;

namespace Redwood.Framework.Tests.Parsing.ViewModel
{
    [TestClass]
    public class ViewModelTranslatorTests
    {

        [TestMethod]
        public void TestSampleViewModel()
        {
            var translator = new ViewModelTranslator(new ReflectionViewModelMetadataExtractor(), new TypeScriptViewModelTypeMapper(), new TypeScriptViewModelWriter());
            var output = translator.TranslateViewModels(typeof (ReflectionViewModelMetadataExtractorTests.TestViewModel));
        }


    }
}
