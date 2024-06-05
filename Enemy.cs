using System;
using SplashKitSDK;

namespace SpaceDefender
{
    public abstract class Enemy
    {
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public int Radius { get; protected set; }
        protected Bitmap _EnemyBitmap;
        protected int Speed;
        public Enemy(Window gameWindow, string bitmapName, int speed)
        {
            _EnemyBitmap = new Bitmap(bitmapName, bitmapName + ".png");
            X = SplashKit.Rnd(gameWindow.Width - _EnemyBitmap.Width);
            Y = -_EnemyBitmap.Height;
            Radius = _EnemyBitmap.Width / 2;
            Speed = speed;
        }

        public virtual void Update()
        {
            Y += Speed;
        }

        public void Draw()
        {
            _EnemyBitmap.Draw(X, Y);
        }

        public Circle CollisionCircle
        {
            get { return SplashKit.CircleAt(X + Radius, Y + Radius, Radius); }
        }

        public bool IsOffscreen(Window screen)
        {
            return Y > screen.Height;
        }
    }

    public class NormalEnemy : Enemy
    {
        public NormalEnemy(Window gameWindow): base(gameWindow, "NormalEnemy", 2) 
        {
        }
    }

    public class FastEnemy : Enemy
    {
        public FastEnemy(Window gameWindow): base(gameWindow, "FastEnemy", 6) 
        {
        }
    }
}