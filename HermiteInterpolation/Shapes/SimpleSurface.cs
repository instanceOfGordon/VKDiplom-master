#region

using System;
using System.Linq;
using System.Windows.Graphics;
using HermiteInterpolation.Primitives;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


#endregion

namespace HermiteInterpolation.Shapes
{
    public class SimpleSurface : ISurface, IDisposable
    {
        private readonly VertexBuffer _vertexBuffer;
        private readonly VertexPositionNormalColor[] _vertices;
        private readonly int _xDimension;
        private readonly int _yDimension;
        private DrawStyle _drawStyle;
        private IndexBuffer _indexBuffer;
        private short[] _indices;
        private float? _maxHeight;
        private float? _minHeight;
        private RasterizerState _rasterizerState;
        private bool _disposed;

        public SimpleSurface(VertexPositionNormalColor[] vertices, int xDimension, int yDimension)
        {
           
            _xDimension = xDimension;
            _yDimension = yDimension;
            _vertices = vertices;
           
            if (GraphicsDeviceManager.Current.RenderMode != RenderMode.Hardware) return;
            _vertexBuffer = new VertexBuffer(GraphicsDeviceManager.Current.GraphicsDevice,
            typeof (VertexPositionNormalColor), vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(_vertices);
            DrawStyle = DrawStyle.Surface;
            //_indices = new short[_xDimension]; //_vertices.Length];

            // SquareIndices();
            //_vertexBuffer = vertexBuffer;
        }

//        public virtual void Dispose()
//        {
//            _vertexBuffer.Dispose();
//            _indexBuffer.Dispose();
//        }

        public DrawStyle DrawStyle
        {
            get { return _drawStyle; }
            set
            {
                _drawStyle = value;
                switch (value)
                {
                    case DrawStyle.Wireframe:
                        _rasterizerState = new RasterizerState
                        {
                            FillMode = FillMode.WireFrame,
                            CullMode = CullMode.None
                        };
                        break;
                    case DrawStyle.FramedSurface:

                        break;
                    default:
                        _rasterizerState = new RasterizerState
                        {
                            FillMode = FillMode.Solid,
                            CullMode = CullMode.None
                        };
                        break;                 
                }
                InitializeIndices();
                CalculateLightingNormals();
            }
        }

        // internal TextureStyle TextureStyle { get; private set; }


        public float MinHeight
        {
            get
            {
                if (!_minHeight.HasValue) _minHeight = _vertices.Min(vertex => vertex.Position.Z);
                return _minHeight.Value;
            }
        }

        public float MaxHeight
        {
            get
            {
                if (!_maxHeight.HasValue) _maxHeight = _vertices.Max(vertex => vertex.Position.Z);
                return _maxHeight.Value;
            }
        }

        public void ColoredSimple(Color color)
        {
            for (var i = 0; i < _vertices.Count(); i++)
            {
                _vertices[i].Color = color;
            }
            _vertexBuffer?.SetData(_vertices);
        }

        public void ColoredHeight()
        {
            ColoredHeight(0f, 300f);
        }

        public void ColoredHeight(float fromHue, float toHue)
        {
            var minZ = MinHeight;
            var maxZ = MaxHeight;

            var dZ = maxZ - minZ;
            var dHue = toHue - fromHue;

            for (var i = 0; i < _vertices.Count(); i++)
            {
                var zPercentage = (_vertices[i].Position.Z - minZ)/dZ;
                var hue = (dHue*zPercentage) + fromHue;
                _vertices[i].Color = ColorUtils.FromHsv(hue, 1, 1, 1);
            }
            _vertexBuffer.SetData(_vertices);
        }

        public void Draw()
        {
            var graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;
            graphicsDevice.RasterizerState = _rasterizerState;
            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;
            switch (_drawStyle)
            {
                case DrawStyle.Wireframe:
                    DrawWireframe();
                    break;
                case DrawStyle.Surface:
                    DrawSurface();
                    break;
                case DrawStyle.Contour:
                    DrawContour();
                    break;
                case DrawStyle.Triangles:
                    DrawTriangles();
                    break;
                case DrawStyle.FramedSurface:
                    DrawFramedSurface();
                    break;
            }
        }

       

        public void CalculateLightingNormals()
        {
            //var normalCalc = new VertexNormals();
            switch (_drawStyle)
            {
                case DrawStyle.Wireframe:
                    // TODO: Not supported yet
                    break;
                case DrawStyle.Surface:
                    VertexNormals.CalculateTriangleNormals(_vertices,_indices);
                    break;
                case DrawStyle.Contour:
                    // TODO: Not supported yet
                    break;
                case DrawStyle.Triangles:
                    VertexNormals.CalculateTriangleNormals(_vertices, _indices);
                    break;
            }
            for (var i = 0; i < _vertices.Length; i++)
            {
                _vertices[i].Normal.Normalize();
            }
            _vertexBuffer.SetData(_vertices);
        }

        

        private void InitIndexBuffer(short[] indices)
        {
            _indices = indices;
            _indexBuffer = new IndexBuffer(GraphicsDeviceManager.Current.GraphicsDevice,
                typeof (short), indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);
        }

        private void DrawTriangles()
        {
            var graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Length, 0,
                _indexBuffer.IndexCount/3);
        }

        private void DrawContour()
        {
            var graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, _vertices.Length, 0,
                _indexBuffer.IndexCount/2);
        }

        private void DrawSurface()
        {
            var graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Length, 0,
                _indexBuffer.IndexCount/3);
        }

        private void DrawWireframe()
        {
            var graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, _vertices.Length, 0,
                _indexBuffer.IndexCount/2);
        }

        private void DrawFramedSurface()
        {
            throw new NotImplementedException();
        }

        private void InitializeIndices()
        {
            var indexer = new VertexIndexer();
            short[] indices;
            switch (_drawStyle)
            {
                case DrawStyle.Wireframe:
                    indices = indexer.SquareIndices(_xDimension,_yDimension);
                    break;
                case DrawStyle.Surface:
                    indices = indexer.TriangleIndices(_xDimension, _yDimension);
                    break;
                case DrawStyle.Contour:
                    indices = indexer.ContourIndices(_xDimension, _yDimension);
                    break;
                case DrawStyle.Triangles:
                    indices = indexer.TriangleIndices(_xDimension, _yDimension);
                    break;
//                case DrawStyle.FramedSurface:
//                    break;
                default:
                    indices = indexer.TriangleIndices(_xDimension, _yDimension);
                    break;
            }
            InitIndexBuffer(indices);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

//        ~SimpleSurface()
//        {
//            Dispose(false);
//        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _vertexBuffer.Dispose();
                _indexBuffer.Dispose();
            }

            _disposed = true;
        }
    }
}