using System;
using System.Windows.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HermiteInterpolation.Shapes
{
    public sealed class CoordinateAxes : IDrawable, IDisposable
    {
        private const int NumberOfVerts = 22;
        private const int NumberOfVertsArrows = 54;

        private VertexBuffer _arrowsVertexBuffer;
        private VertexBuffer _axesVertexBuffer;
        private bool _disposed;


        public CoordinateAxes()
        {
            
            CreateShape();
        }

//        protected override void SetupView(ref GraphicsDevice graphicsDevice,  BasisMatrix viewProjection)
//        {
//
//            //var projection = CreateFinalProjection(viewProjection);
//            graphicsDevice.SetVertexBuffer(VertexBuffer);
//            graphicsDevice.DrawPrimitives(PrimitiveType.LineList,0,NumberOfVerts/2);
//            graphicsDevice.SetVertexBuffer(_arrowsVertexBuffer);
//            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList,0,NumberOfVertsArrows/3);
//        }

        //  protected override void SetupView(ref GraphicsDevice graphicsDevice,  BasisMatrix viewProjection)
        // {

//            //var projection = CreateFinalProjection(viewProjection);
//            graphicsDevice.SetVertexBuffer(VertexBuffer);
//            graphicsDevice.DrawPrimitives(PrimitiveType.LineList,0,NumberOfVerts/2);
//            graphicsDevice.SetVertexBuffer(_arrowsVertexBuffer);
//            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList,0,NumberOfVertsArrows/3);
//        }
        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _arrowsVertexBuffer.Dispose();
                _axesVertexBuffer.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CoordinateAxes()
        {
             Dispose(false);
        }

        //public BlendState BlendState { get; set; } => BlendState.O

        public void Draw()
        {
            var graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;
            graphicsDevice.SetVertexBuffer(_axesVertexBuffer);
            graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, NumberOfVerts/2);
            graphicsDevice.SetVertexBuffer(_arrowsVertexBuffer);
            graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, NumberOfVertsArrows/3);
        }

        private void CreateShape()
        {
            const float axisLength = 5f;
            var color0 = new Color(200, 20, 20);
            var color1 = new Color(20, 200, 20);
            var color2 = new Color(20, 20, 200);

            var color0d = new Color(100, 20, 20);
            var color1d = new Color(20, 100, 20);
            var color2d = new Color(20, 20, 100);

            var axes = new VertexPositionColor[NumberOfVerts];
            var arrows = new VertexPositionColor[NumberOfVertsArrows];
            //X
            axes[0] = new VertexPositionColor(new Vector3(0, 0, 0), color0);
            axes[1] = new VertexPositionColor(new Vector3(axisLength, 0, 0), color0);
            const float shift = 0.2f;
            var xx = new Vector3(axisLength, 0, 0);
            var xa = new Vector3(axisLength - shift, -shift, -shift);
            var xb = new Vector3(axisLength - shift, shift, -shift);
            var xc = new Vector3(axisLength - shift, shift, shift);
            var xd = new Vector3(axisLength - shift, -shift, shift);

            arrows[0] = new VertexPositionColor(xx, color0);
            arrows[1] = new VertexPositionColor(xb, color0);
            arrows[2] = new VertexPositionColor(xa, color0);

            arrows[3] = new VertexPositionColor(xx, color0);
            arrows[4] = new VertexPositionColor(xc, color0);
            arrows[5] = new VertexPositionColor(xb, color0);

            arrows[6] = new VertexPositionColor(xx, color0);
            arrows[7] = new VertexPositionColor(xd, color0);
            arrows[8] = new VertexPositionColor(xc, color0);

            arrows[9] = new VertexPositionColor(xx, color0);
            arrows[10] = new VertexPositionColor(xa, color0);
            arrows[11] = new VertexPositionColor(xd, color0);

            arrows[12] = new VertexPositionColor(xb, color0);
            arrows[13] = new VertexPositionColor(xc, color0);
            arrows[14] = new VertexPositionColor(xa, color0);

            arrows[15] = new VertexPositionColor(xd, color0);
            arrows[16] = new VertexPositionColor(xa, color0);
            arrows[17] = new VertexPositionColor(xc, color0);

            //Y
            axes[2] = new VertexPositionColor(new Vector3(0, 0, 0), color1);
            axes[3] = new VertexPositionColor(new Vector3(0, axisLength, 0), color1);
//            axes[3] = new VertexPositionColor(new Vector3(0, 0, axisLength), color1);

            var yy = new Vector3(0, axisLength, 0);
//            var yy = new Vector3(0, 0, axisLength);
            var ya = new Vector3(-shift, axisLength - shift, -shift);
            var yb = new Vector3(shift, axisLength - shift, -shift);
            var yc = new Vector3(shift, axisLength - shift, shift);
            var yd = new Vector3(-shift, axisLength - shift, shift);
//            var ya = new Vector3(-shift, -shift, axisLength - shift);
//            var yb = new Vector3(shift, -shift, axisLength - shift);
//            var yc = new Vector3(shift, shift, axisLength - shift);
//            var yd = new Vector3(-shift, shift, axisLength - shift);
            arrows[18] = new VertexPositionColor(yy, color1);
            arrows[19] = new VertexPositionColor(yb, color1);
            arrows[20] = new VertexPositionColor(ya, color1);

            arrows[21] = new VertexPositionColor(yy, color1);
            arrows[22] = new VertexPositionColor(yc, color1);
            arrows[23] = new VertexPositionColor(yb, color1);

            arrows[24] = new VertexPositionColor(yy, color1);
            arrows[25] = new VertexPositionColor(yd, color1);
            arrows[26] = new VertexPositionColor(yc, color1);

            arrows[27] = new VertexPositionColor(yy, color1);
            arrows[28] = new VertexPositionColor(ya, color1);
            arrows[29] = new VertexPositionColor(yd, color1);

            arrows[30] = new VertexPositionColor(yb, color1);
            arrows[31] = new VertexPositionColor(yc, color1);
            arrows[32] = new VertexPositionColor(ya, color1);

            arrows[33] = new VertexPositionColor(yd, color1);
            arrows[34] = new VertexPositionColor(ya, color1);
            arrows[35] = new VertexPositionColor(yc, color1);

            //Z
            axes[4] = new VertexPositionColor(new Vector3(0, 0, 0), color2);
//            axes[5] = new VertexPositionColor(new Vector3(0, axisLength, 0), color2);
            axes[5] = new VertexPositionColor(new Vector3(0, 0, axisLength), color2);


            var zz = new Vector3(0, 0, axisLength);
//            var zz = new Vector3(0, axisLength, 0);
            var za = new Vector3(-shift, -shift, axisLength - shift);
            var zb = new Vector3(shift, -shift, axisLength - shift);
            var zc = new Vector3(shift, shift, axisLength - shift);
            var zd = new Vector3(-shift, shift, axisLength - shift);
//            var za = new Vector3(-shift, axisLength - shift, -shift);
//            var zb = new Vector3(shift, axisLength - shift, -shift);
//            var zc = new Vector3(shift, axisLength - shift, shift);
//            var zd = new Vector3(-shift, axisLength - shift, shift);

            arrows[36] = new VertexPositionColor(zz, color2);
            arrows[37] = new VertexPositionColor(zb, color2);
            arrows[38] = new VertexPositionColor(za, color2);

            arrows[39] = new VertexPositionColor(zz, color2);
            arrows[40] = new VertexPositionColor(zc, color2);
            arrows[41] = new VertexPositionColor(zb, color2);

            arrows[42] = new VertexPositionColor(zz, color2);
            arrows[43] = new VertexPositionColor(zd, color2);
            arrows[44] = new VertexPositionColor(zc, color2);

            arrows[45] = new VertexPositionColor(zz, color2);
            arrows[46] = new VertexPositionColor(za, color2);
            arrows[47] = new VertexPositionColor(zd, color2);

            arrows[48] = new VertexPositionColor(zb, color2);
            arrows[49] = new VertexPositionColor(zc, color2);
            arrows[50] = new VertexPositionColor(za, color2);

            arrows[51] = new VertexPositionColor(zd, color2);
            arrows[52] = new VertexPositionColor(za, color2);
            arrows[53] = new VertexPositionColor(zc, color2);

            //X oznacenie
            axes[6] = new VertexPositionColor(new Vector3(axisLength, 0.2f, 0), color0d);
            axes[7] = new VertexPositionColor(new Vector3(axisLength + 0.2f, 0.5f, 0), color0d);
            axes[8] = new VertexPositionColor(new Vector3(axisLength + 0.2f, 0.2f, 0), color0d);
            axes[9] = new VertexPositionColor(new Vector3(axisLength, 0.5f, 0), color0d);

            //Y oznacenie
            axes[10] = new VertexPositionColor(new Vector3(0.2f, axisLength + 0.35f, 0), color1d);
            axes[11] = new VertexPositionColor(new Vector3(0.3f, axisLength + 0.2f, 0), color1d);
            axes[12] = new VertexPositionColor(new Vector3(0.3f, axisLength + 0.2f, 0), color1d);
            axes[13] = new VertexPositionColor(new Vector3(0.4f, axisLength + 0.35f, 0), color1d);
            axes[14] = new VertexPositionColor(new Vector3(0.3f, axisLength + 0.2f, 0), color1d);
            axes[15] = new VertexPositionColor(new Vector3(0.3f, axisLength + 0.05f, 0), color1d);

//            axes[10] = new VertexPositionColor(new Vector3(0.2f,0.35f,  axisLength + 0), color1d);
//            axes[11] = new VertexPositionColor(new Vector3(0.3f, 0.2f, axisLength + 0), color1d);
//            axes[12] = new VertexPositionColor(new Vector3(0.3f,  0.2f, axisLength + 0), color1d);
//            axes[13] = new VertexPositionColor(new Vector3(0.4f, 0.35f, axisLength + 0), color1d);
//            axes[14] = new VertexPositionColor(new Vector3(0.3f,  0.2f, axisLength + 0), color1d);
//            axes[15] = new VertexPositionColor(new Vector3(0.3f,  0.05f, axisLength + 0), color1d);

            //Z oznacenie
//            axes[16] = new VertexPositionColor(new Vector3(0, axisLength + 0.1f, 0.4f), color2d);
//            axes[17] = new VertexPositionColor(new Vector3(0, axisLength + 0.4f,  0.4f), color2d);
//            axes[18] = new VertexPositionColor(new Vector3(0, axisLength + 0.4f, + 0.4f), color2d);
//            axes[19] = new VertexPositionColor(new Vector3(0, axisLength + 0.1f, 0.2f), color2d);
//            axes[20] = new VertexPositionColor(new Vector3(0, axisLength + 0.1f,0.2f), color2d);
//            axes[21] = new VertexPositionColor(new Vector3(0, axisLength + 0.4f,  0.2f), color2d);

            axes[16] = new VertexPositionColor(new Vector3(0, 0.1f, axisLength + 0.4f), color2d);
            axes[17] = new VertexPositionColor(new Vector3(0, 0.4f, axisLength + 0.4f), color2d);
            axes[18] = new VertexPositionColor(new Vector3(0, 0.4f, axisLength + 0.4f), color2d);
            axes[19] = new VertexPositionColor(new Vector3(0, 0.1f, axisLength + 0.2f), color2d);
            axes[20] = new VertexPositionColor(new Vector3(0, 0.1f, axisLength + 0.2f), color2d);
            axes[21] = new VertexPositionColor(new Vector3(0, 0.4f, axisLength + 0.2f), color2d);
            if (GraphicsDeviceManager.Current.RenderMode != RenderMode.Hardware) return;

            var g = GraphicsDeviceManager.Current.GraphicsDevice;
            _axesVertexBuffer = new VertexBuffer(g, typeof (VertexPositionColor), NumberOfVerts, BufferUsage.WriteOnly);
            _axesVertexBuffer.SetData(axes);
            _arrowsVertexBuffer = new VertexBuffer(g, typeof (VertexPositionColor), NumberOfVertsArrows,
                BufferUsage.WriteOnly);
            _arrowsVertexBuffer.SetData(arrows);
        }
    }
}