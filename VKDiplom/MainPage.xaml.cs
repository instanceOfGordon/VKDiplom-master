using HermiteInterpolation.Shapes;
using Microsoft.Xna.Framework;
using VKDiplom.Engine;
using Point = System.Windows.Point;

namespace VKDiplom
{
    // If this code works, it is written by Viliam Kačala.
    // If it does not work, I don't know who wrote it.
    public partial class MainPage
    {
        private const float RotationFactor = .01f;
        private const float ZoomFactor = .1f;
        //GraphicsDevice _graphicsDevice;
        //private SceneTime _sceneTime;
        private readonly RotateCamera _camera = new RotateCamera();
        // Distance

        private static readonly Color DefaultColor = Color.FromNonPremultiplied(128, 128, 128, 208);
        private readonly Color _color = DefaultColor;
        private bool _canDrag;
        private DrawStyle _drawStyle = DrawStyle.Surface;

        // Scenes
        private Scene _firstDerScene;
        private Scene _functionScene; //= new Scene();
        private Scene _secondDerScene;
        private Point _mouseDownPosition;


        //private readonly InterpolatedFunction _aproximationFunction = new InterpolatedFunction();

        private TextureStyle _textureStyle = TextureStyle.HeightColored;

        public MainPage()
        {
            InitializeComponent();
        }

       
    }
}