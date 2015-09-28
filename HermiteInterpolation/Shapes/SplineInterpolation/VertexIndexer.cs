namespace HermiteInterpolation.Shapes.SplineInterpolation
{
    public class VertexIndexer
    {
        public short[] SquareIndices(int xDimension, int yDimension)
        {
            var indices = new short[(xDimension - 1) * (yDimension - 1) * 5];
            var counter = 0;

            for (var y = 0; y < yDimension - 1; y++)
            {
                for (var x = 0; x < xDimension - 1; x++)
                {
                    var lowerLeft = x + y * yDimension;
                    var lowerRight = (x + 1) + y * yDimension;
                    var topLeft = x + (y + 1) * yDimension;
                    var topRight = (x + 1) + (y + 1) * yDimension;

                    //                    _indices[counter++] = (short)topLeft;
                    //                    _indices[counter++] = (short)lowerRight;
                    //                    _indices[counter++] = (short)lowerLeft;
                    //
                    //                    _indices[counter++] = (short)topLeft;
                    //                    _indices[counter++] = (short)topRight;
                    //                    _indices[counter++] = (short)lowerRight;
                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)lowerLeft;
                    indices[counter++] = (short)lowerRight;

                    //_indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)topRight;
                    indices[counter++] = (short)topLeft;
                    //_indices[counter++] = (short)lowerRight;
                }
            }
            return indices;
        }

        public short[] TriangleIndices(int xDimension, int yDimension)
        {
            var indices = new short[(xDimension - 1) * (yDimension - 1) * 6];
            var counter = 0;

            for (var x = 0; x < xDimension - 1; x++)
            //for (var y = 0; y < yDimension - 1; y++)
            {
                for (var y = 0; y < yDimension - 1; y++)
                //for (var x = 0; x < _xDimension - 1; x++)
                {
                    //                    var lowerLeft = x + y*yDimension;
                    //                    var lowerRight = (x + 1) + y*yDimension;
                    //                    var topLeft = x + (y + 1)*yDimension;
                    //                    var topRight = (x + 1) + (y + 1)*yDimension;

                    var lowerLeft = y + x * yDimension;
                    var lowerRight = lowerLeft + 1;
                    var topLeft = y + (x + 1) * yDimension;
                    var topRight = topLeft + 1;

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)lowerRight;
                    indices[counter++] = (short)lowerLeft;

                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)topRight;
                    indices[counter++] = (short)lowerRight;
                }
            }
           return indices;
        }

        public short[] ContourIndices(int xDimension, int yDimension)
        {
            var indices = new short[(xDimension - 1) * (yDimension - 1) * 4];
            var counter = 0;

            for (var y = 0; y < yDimension - 1; y++)
            {
                for (var x = 0; x < xDimension - 1; x++)
                {
                    var lowerLeft = x + y * yDimension;
                    var lowerRight = (x + 1) + y * yDimension;
                    var topLeft = x + (y + 1) * yDimension;
                    var topRight = (x + 1) + (y + 1) * yDimension;

                    //                    _indices[counter++] = (short)topLeft;
                    //                    _indices[counter++] = (short)lowerRight;
                    //                    _indices[counter++] = (short)lowerLeft;
                    //
                    //                    _indices[counter++] = (short)topLeft;
                    //                    _indices[counter++] = (short)topRight;
                    //                    _indices[counter++] = (short)lowerRight;
                    indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)lowerLeft;
                    indices[counter++] = (short)lowerRight;

                    //_indices[counter++] = (short)topLeft;
                    indices[counter++] = (short)topRight;
                    //indices[counter++] = (short) topLeft;
                    //_indices[counter++] = (short)lowerRight;
                }
            }
            return indices;
        }
    }
}
