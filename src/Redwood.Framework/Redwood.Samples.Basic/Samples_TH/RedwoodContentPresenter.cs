using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redwood.Framework;

namespace Redwood.Samples.Basic.Samples_TH
{
    public abstract class RedwoodContentPresenter : RedwoodPresenter
    {

        public RedwoodPresenter Master { get; private set; }        // presenter rodičovské stránky


        protected abstract RedwoodPresenter ResolveMasterPresenter();

    }
}
