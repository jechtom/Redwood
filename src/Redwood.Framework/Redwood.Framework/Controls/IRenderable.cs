using Redwood.Framework.Generation;
using System;
namespace Redwood.Framework.Controls
{
    public interface IRenderable
    {
        void Render(IHtmlWriter writer);
    }
}
