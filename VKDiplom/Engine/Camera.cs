using Microsoft.Xna.Framework;

namespace VKDiplom.Engine
{
    public class Camera
    {
        protected Vector3 UpVector { get; set; } = Vector3.Up;
        private Vector3 _position;
        private Vector3 _target;
        private Matrix _view; // The _view or camera transform
        private float _renderDistance = 200f;
        private float _aspectRatio;

        public Camera()
        {
            _target = Vector3.Zero;
            _position = new Vector3(0, 10, 19f);
            _view = Matrix.CreateLookAt(_position, _target, UpVector);
           
        }

        public Matrix ViewTransform
        {
            get { return _view; }
            set { _view = value; }
        }

        /// <summary>
        ///     Projection transform to convert 3D space to 2D screen space
        /// </summary>
        public Matrix Projection { get; private set; }

        /// <summary>
        ///     Aspect ratio of the viewport for this camera
        /// </summary>
        public float AspectRatio
        {
            set
            {
                _aspectRatio = value;
                // update the screen space transform every time the aspect ratio changes
                Projection = Matrix.CreatePerspectiveFieldOfView(0.85f, _aspectRatio, 0.01f, _renderDistance);
            }
        }

        public float RenderDistance
        {
            get { return _renderDistance; }
            set
            {
                _renderDistance = value;
                Projection = Matrix.CreatePerspectiveFieldOfView(0.85f, _aspectRatio, 0.01f, _renderDistance);
            }
        }

        /// <summary>
        ///     Position in world coordinates
        /// </summary>
        public Vector3 Position
        {
            set
            {
                _position = value;
                UpdateView();
            }
            get { return _position; }
        }

        private void UpdateView()
        {
            _view = Matrix.CreateLookAt(_position, _target, UpVector);
        }
    }
}