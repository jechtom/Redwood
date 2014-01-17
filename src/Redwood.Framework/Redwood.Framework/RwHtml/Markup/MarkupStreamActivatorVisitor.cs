using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Reads objects and members and activates CLR objects.
    /// </summary>
    public class MarkupStreamActivatorVisitor : MarkupStreamVisitor
    {
        ControlTypeActivator typeActivator;

        public MarkupStreamActivatorVisitor()
        {
            typeActivator = ControlTypeActivator.Default;
        }

        protected override void OnFramePushing(MarkupFrame markupFrame)
        {
            switch (markupFrame.FrameType)
            {
                case MarkupFrameType.Object:
                    OnBeginObjectFrame(markupFrame);
                    break;
                case MarkupFrameType.Member:
                    OnBeginMemberFrame(markupFrame);
                    break;
            }

            base.OnFramePushing(markupFrame);
        }

        private void OnBeginMemberFrame(MarkupFrame markupFrame)
        {
            throw new NotImplementedException();
        }

        private void OnBeginObjectFrame(MarkupFrame markupFrame)
        {
            var node = markupFrame.Node;
            var instance = typeActivator.Activate(node.Type.ClrType);
        }
    }
}
