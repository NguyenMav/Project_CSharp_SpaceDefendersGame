using System;
using SplashKitSDK;

namespace SpaceDefender
{
    public class Bullet
    {
        private double _X;
        private double _Y;
        private const int _Speed = 5;
        private const int _Radius = 5;
        private Color _Color = Color.Yellow;
        public bool IsActive { get; private set; }

        public Bullet(double startX, double startY)
        {
            _X = startX;
            _Y = startY;
            IsActive = true;
        }

        public void Update()
        {
            _Y -= _Speed;
        }

        public void Draw()
        {
            SplashKit.FillCircle(_Color, _X, _Y, _Radius);
        }

        public bool IsOffscreen()
        {
            return _Y < 0;
        }

        public bool CollideWith(Enemy enemy)
        {
            return SplashKit.CirclesIntersect(_X, _Y, _Radius, enemy.X + enemy.Radius, enemy.Y + enemy.Radius, enemy.Radius);
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}