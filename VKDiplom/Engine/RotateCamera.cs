using System;
using HermiteInterpolation.Utils;
using Microsoft.Xna.Framework;

namespace VKDiplom.Engine
{
    public class RotateCamera : Camera
    {
        private float _distance;
        private float _verticalAngle;
        private float _horizontalAngle;

        public float MaxDistance { get; set; }

        public RotateCamera()
        {
            MaxDistance = 100f;
            _distance = 20;
            _verticalAngle = MathHelper.PiOver4;
            _horizontalAngle = MathHelper.PiOver2;
            Position = CoordinateSystems.FromSphericalToCartesian(_distance, _verticalAngle, _horizontalAngle);
        }

        public RotateCamera(float distance, float verticalAngle, float horizontalAngle)
        {
            _distance = distance;
            _verticalAngle = verticalAngle;
            _horizontalAngle = horizontalAngle;
            Position = CoordinateSystems.FromSphericalToCartesian(_distance,_verticalAngle,_horizontalAngle);
        }

        public float Distance
        {
            get { return _distance; }
            set
            {
                _distance = MathHelper.Clamp(value, 0.01f, 100f);
                Position = CoordinateSystems.FromSphericalToCartesian(_distance, _verticalAngle, _horizontalAngle);
            }
        }

        public float VerticalAngle
        {
            get { return _verticalAngle; }
            set
            {
                _verticalAngle = MathHelper.Clamp(value,0.01f,MathHelper.Pi-0.01f);
                Position = CoordinateSystems.FromSphericalToCartesian(_distance, _verticalAngle, _horizontalAngle);
                //Position = CoordinateSystems.FromSphericalToCartesian(_distance, _horizontalAngle, _verticalAngle);
            }
        }

        public float HorizontalAngle
        {
            get { return _horizontalAngle; }
            set
            {
                _horizontalAngle = value % MathHelper.TwoPi;
                Position = CoordinateSystems.FromSphericalToCartesian(_distance, _verticalAngle, _horizontalAngle);
            }
        }

        public Vector3 SphericalPosition
        {
            get { return new Vector3(_distance,_verticalAngle,_horizontalAngle);}
            set
            {
                _distance = value.X;
                _verticalAngle = value.Y;
                _horizontalAngle = value.Z;
                Position = CoordinateSystems.FromSphericalToCartesian(value);
            }
        }
    }
}
