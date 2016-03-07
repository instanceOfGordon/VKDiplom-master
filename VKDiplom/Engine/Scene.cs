using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Graphics;
using System.Windows.Media;
using HermiteInterpolation.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VKDiplom.Engine.Utils;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;
using WindowsColor = System.Windows.Media.Color;

namespace VKDiplom.Engine
{

    public class Scene : IList<IDrawable>
    {
        public enum LightingQuality
        {
            Low,
            Medium,
            High
        }

        // Avoid allocating in Draw method as it is called each frame.
        private static readonly Color BackgroundColor = Color.White;//new Color(244, 244, 240, 255);
        // List of every shape in scene.
        // Rotation matrix for z-axe facing up.
        private readonly Matrix _zAxeFacingUpRotation = Matrix.CreateRotationZ(MathHelper.ToRadians(-135))*
                                                        Matrix.CreateRotationX(MathHelper.ToRadians(-90));

        // Coordinate axes
        private CoordinateAxes _axes;
        // Shader used for rendering
        private BasicEffect _effect;
        private int _highlightedShapeIndex = -1;
        private bool _renderingEnabled = true;

        public void EnableRendering()
        {
            _renderingEnabled = true;
        }

        public void DisableRendering()
        {
            _renderingEnabled = false;
        }

        private Matrix _scale = Matrix.CreateScale(1.0f);
        private List<IDrawable> _shapes = new List<IDrawable>();
        private float _zoom = 1.0f;

        public Scene()
        {
            Camera = new Camera();
            InitializeComponent();
        }

        public Scene(Camera camera)
        {
            Camera = camera;
            InitializeComponent();
        }

        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }

        public Camera Camera { get; }

        public Vector3 Position { get; set; } = Vector3.Up;

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                _scale = Matrix.CreateScale(_zoom);
            }
        }

        public Vector3 Scale
        {
            get { return new Vector3(_scale.M11, _scale.M22,_scale.M33); }
            set { _scale = Matrix.CreateScale(value.X, value.Y, value.Z); }
        }

        // Use advanced lighting when HW supports it.
        public bool PreferPerPixelLighting
        {
            set { _effect.PreferPerPixelLighting = value; }
        }

        public int HighlightedShapeIndex
        {
            get { return _highlightedShapeIndex; }
            set
            {
                if (value < _shapes.Count && value > -1)
                    _highlightedShapeIndex = value;
               
            }
        }


        public IEnumerator<IDrawable> GetEnumerator()
        {
            return _shapes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IDrawable item, bool makeHighlighted)
        {
            _shapes.Add(item);
            if(makeHighlighted||_highlightedShapeIndex==-1) ++_highlightedShapeIndex;
        }

        public void Add(IDrawable item)
        {
           Add(item,true);         
        }

        public void Clear()
        {         
            DisableRendering();
            // Dirty thing I know, but I'm too lazy
            Thread.Sleep(50);
            _shapes = new List<IDrawable>();
            _highlightedShapeIndex = -1;
            EnableRendering();
        }
        public bool Contains(IDrawable item)
        {
            return _shapes.Contains(item);
        }

        public void CopyTo(IDrawable[] array, int arrayIndex)
        {
            _shapes.CopyTo(array, arrayIndex);
        }

        public bool Remove(IDrawable item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;
            RemoveAt(index);
            return true;
        }

        public int Count => _shapes.Count;
        public bool IsReadOnly => false;

        public int IndexOf(IDrawable item)
        {
            return _shapes.IndexOf(item);
        }

        public void Insert(int index, IDrawable item)
        {
            DisableRendering();
            if (index >= _highlightedShapeIndex) ++_highlightedShapeIndex;
            _shapes.Insert(index, item);
            EnableRendering();
        }

        public void RemoveAt(int index)
        {
            DisableRendering();
            _shapes.RemoveAt(index);
            if (index <= _highlightedShapeIndex)
                --_highlightedShapeIndex;
            if (_highlightedShapeIndex < 0) _highlightedShapeIndex = 0;
            EnableRendering();
        }

        public IDrawable this[int index]
        {
            get { return _shapes[index]; }
            set { _shapes[index] = value; }
        }

        private void InitializeComponent()
        {
            RotationX = 0.0f;
            RotationY = 0.0f;
            RotationZ = 0.0f;
            if (!VkDiplomGraphicsInitializationUtils.IsHardwareAccelerated()) return;
            _axes = new CoordinateAxes();
            _effect = new BasicEffect(GraphicsDeviceManager.Current.GraphicsDevice)
            {
                VertexColorEnabled = true
            };
            SetLighting(LightingQuality.High);
            PreferPerPixelLighting = true;
        }

       
        public void SetLighting(LightingQuality quality)
        {
            switch (quality)
            {
                case LightingQuality.Low:
                    LowQualityLighting();
                    break;
                case LightingQuality.Medium:
                    MediumQualityLighting();
                    break;
                case LightingQuality.High:
                    HighQualityLighting();
                    break;
            }
        }

        private void LowQualityLighting()
        {
            _effect.LightingEnabled = false;
            _effect.AmbientLightColor = new Vector3(0.8f, 0.8f, 0.8f);
            //_effect.EmissiveColor = new Vector3(0.5f, 0.4f, 0.5f);
        }

        private void MediumQualityLighting()
        {
            //_effect.EnableDefaultLighting();
            _effect.LightingEnabled = true;
            // Enable one directional light.
            _effect.DirectionalLight0.Enabled = true;
            // A light...
            _effect.DirectionalLight0.DiffuseColor = new Vector3(0.97f, 0.97f, 0.93f);
            // ... coming from...
            _effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, -1.5f, 0));
            // ... with this highlight
            _effect.DirectionalLight0.SpecularColor = new Vector3(0.8f, 0.8f, 0.8f);
            // Ambient color (i.e light coming from all directions)
            _effect.AmbientLightColor = new Vector3(0.6f, 0.6f, 0.6f);
            // Emissive color of objects (i.e. light emited from all objects)
            //_effect.EmissiveColor = new Vector3(0.5f, 0.4f, 0.5f);
        }

        private void HighQualityLighting()
        {
            //_effect.EnableDefaultLighting();
            _effect.LightingEnabled = true;
            // Enable three light sources.
            _effect.DirectionalLight0.Enabled = true;
            _effect.DirectionalLight1.Enabled = true;
            _effect.DirectionalLight2.Enabled = true;


            // A lights...

            // (Key light)
            _effect.DirectionalLight0.DiffuseColor = new Vector3(0.92f, 0.92f, 0.92f);
            // (Fill light)
            _effect.DirectionalLight1.DiffuseColor = new Vector3(0.32f, 0.32f, 0.32f);
            // (Rim light)
            _effect.DirectionalLight2.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);


            // ... comings from...
            _effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, -1.5f, 0));
            _effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(-1, -1.5f, -1));
            _effect.DirectionalLight2.Direction = Vector3.Normalize(new Vector3(0, -1.5f, 1));


            // ... with this highlights
            _effect.DirectionalLight0.SpecularColor = new Vector3(0.1f, 0.1f, 0.1f);
            _effect.DirectionalLight0.SpecularColor = new Vector3(0.05f, 0.05f, 0.05f);
            _effect.DirectionalLight0.SpecularColor = new Vector3(0.09f, 0.08f, 0.08f);
            // Ambient color (i.e light coming from all directions)
            _effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
            // Emissive color of objects (i.e. light emited from all objects)
            //_effect.EmissiveColor = new Vector3(0.5f, 0.4f, 0.5f);
        }

        public void Draw()
        {
            var graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;
            // clear the existing render target
            graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, BackgroundColor, 1.0f, 0);
            if (!_renderingEnabled) return;
            //Define render world, view and projection.
            _effect.World = _scale
                //*Matrix.CreateFromYawPitchRoll(.01f*RotationY, .01f*RotationX, .01f*RotationZ)
                            *Matrix.CreateRotationZ(MathHelper.ToRadians(RotationZ))
                            *Matrix.CreateRotationY(MathHelper.ToRadians(RotationY))
                            *Matrix.CreateRotationX(MathHelper.ToRadians(RotationX))
                // *Matrix.CreateFromAxisAngle(_rotationAxis, _rotationAngle)
                //* Matrix.CreateTranslation(-1,-1,0)
                //*Matrix.CreateTranslation(_position)
                            *_zAxeFacingUpRotation;
            _effect.View = Camera.ViewTransform;
            _effect.Projection = Camera.Projection;
            //_effect.Alpha = 0.5f;
            //graphicsDevice.BlendState = BlendState.AlphaBlend;
            // Apply all shader rendering passes.
            var effectTechniquePasses = _effect.CurrentTechnique.Passes;

            for (var i = 0; i < effectTechniquePasses.Count; i++)
            {        
                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                //graphicsDevice.BlendFactor
                graphicsDevice.BlendState = BlendState.Opaque;
                _effect.Alpha = 1f;

                effectTechniquePasses[i].Apply();
                _axes.Draw();
                var eff = new AlphaTestEffect(graphicsDevice);
              
                if (_shapes.Count == 0) continue;
                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                graphicsDevice.BlendState = BlendState.AlphaBlend;

                _effect.Alpha = 0.95f;
                effectTechniquePasses[i].Apply();
                //if(_highlightedShapeIndex<_shapes.Count)
                //if(_highlightedShapeIndex>-1)
                    _shapes[_highlightedShapeIndex].Draw();
                //_shapes[_shapes.Count-1].Draw();
                //_shapes[0].Draw();
                for (var j = 0; j < _shapes.Count; j++)
                {
                    if (j == _highlightedShapeIndex)
                        //if (j == 0)
                        continue;
                    _effect.Alpha = 0.4f;
                    effectTechniquePasses[i].Apply();
                    _shapes[j].Draw();
                }
            }
        }

        
    }
}