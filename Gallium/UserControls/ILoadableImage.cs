using System;

namespace Gallium.UserControls
{
    public interface ILoadableImage
    {
        event OnMiniatureLoadedHandler MiniatureLoaded;
        void LoadImage();
    }
}