using System;
using SplashKitSDK;

namespace SpaceDefender
{
    public class PowerUp
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        private Bitmap _PowerUpBitmap;
        private const int Speed = 2;
        public PowerUp(Window gameWindow)
        {
            _PowerUpBitmap = new Bitmap("PowerUp", "PowerUp.png");
            X = SplashKit.Rnd(gameWindow.Width - _PowerUpBitmap.Width);
            Y = -_PowerUpBitmap.Height;
        }

        public void Update()
        {
            Y += Speed;
        }

        public void Draw()
        {
            _PowerUpBitmap.Draw(X, Y);
        }

        public Circle CollisionCircle
        {
            get { return SplashKit.CircleAt(X + _PowerUpBitmap.Width / 2, Y + _PowerUpBitmap.Height / 2, _PowerUpBitmap.Width / 2); }
        }

        public void ApplyEffect(Player player)
        {
            if (player.Lives < 5) 
            {
                player.Lives += 1; 
            }
        }
    }
}