using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Primitives
{
    /// <summary>
    /// Helper class for calculating normal vectors of procedurally created 3D shapes. 
    /// </summary>
    public static class VertexNormals
    {
        /// <summary>
        /// Calculate lighing normals for specified vertex array.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        public static void CalculateTriangleNormals(VertexPositionNormalColor[] vertices, short[] indices)
        {
          
            ZeroNormals(vertices);
            for (var i = 0; i < indices.Length/3; i++)
            {
                var idx0 = indices[i*3];
                var idx1 = indices[i*3 + 1];
                var idx2 = indices[i*3 + 2];

                var side0 = vertices[idx0].Position - vertices[idx2].Position;
                var side1 = vertices[idx0].Position - vertices[idx1].Position;
                var normal = Vector3.Cross(side0, side1);

                vertices[idx0].Normal -= normal;
                vertices[idx1].Normal -= normal;
                vertices[idx2].Normal -= normal;
            }
        }

        public static void ZeroNormals(VertexPositionNormalColor[] vertices)
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal = Vector3.Zero;
            }
        }
    }
}