using HermiteInterpolation.Shapes;

namespace VKDiplom.Engine
{
    public partial class Scene
    {
        public void Add(IDrawable drawable)
        {
            _shapes.Add(drawable);
        }

        public void Clear()
        {
            _shapes.Clear();
        }
    }
}