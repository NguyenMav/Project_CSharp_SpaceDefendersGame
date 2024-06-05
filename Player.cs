using System.Collections.Generic;
using SplashKitSDK;

namespace SpaceDefender
{
    public class Player
    {
        protected Bitmap _PlayerBitmap;
        protected Window _gameWindow;
        protected double _rotationAngle = -90;
        private long _lastShootTime = 0; 
        private const long ShootingInterval = 1000; 
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public bool Quit { get; protected set; }
        public bool Active { get; set; } 
        public bool AllowBulletSpawn { get; set; } 
        public int Lives { get; set; } = 3;
        public int Score { get; set; }
        public List<Bullet> Bullets { get; protected set; }

        public Player(Window gameWindow, string bitmapName)
        {
            _gameWindow = gameWindow; 
            Quit = false;
            _PlayerBitmap = new Bitmap(bitmapName, bitmapName + ".png");
            X = (gameWindow.Width - _PlayerBitmap.Width) / 2;
            Y = gameWindow.Height - _PlayerBitmap.Height - 10;
            Bullets = new List<Bullet>();
            Active = true; 
            AllowBulletSpawn = true; 
        }

        public virtual void Draw()
        {
            _PlayerBitmap.Draw(X, Y, SplashKit.OptionRotateBmp(_rotationAngle)); 
            foreach (var bullet in Bullets)
            {
                bullet.Draw();
            }
        }

        public virtual void HandleInput() { }

        public virtual void StayOnWindow(Window gameWindow)
        {
            const int GAP = 10;
            if (X < GAP)
            {
                X = GAP;
            }
            else if (X > gameWindow.Width - GAP - _PlayerBitmap.Width)
            {
                X = gameWindow.Width - GAP - _PlayerBitmap.Width;
            }
        }

        public virtual bool CollideWith(Enemy enemy)
        {
            return _PlayerBitmap.CircleCollision(X, Y, enemy.CollisionCircle);
        }

        public virtual bool CollideWith(PowerUp powerUp)
        {
            return _PlayerBitmap.CircleCollision(X, Y, powerUp.CollisionCircle);
        }

        public virtual void Shoot()
        {
            Bullets.Add(new Bullet(X + _PlayerBitmap.Width / 2, Y));
            SplashKit.PlaySoundEffect("Shoot");
        }

        public virtual void UpdateBullets()
        {
            foreach (var bullet in Bullets)
            {
                bullet.Update();
            }
            Bullets.RemoveAll(b => b.IsOffscreen());
        }

        public virtual void Update()
        {
            HandleInput();
            StayOnWindow(_gameWindow);
            UpdateBullets();
            if ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _lastShootTime >= ShootingInterval)
            {
                Shoot();
                _lastShootTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond; 
            }
        }
    }

    public class Player1 : Player
    {
        public Player1(Window gameWindow) : base(gameWindow, "Player")
        {
            X = gameWindow.Width / 4;
            Y = gameWindow.Height - _PlayerBitmap.Height - 10;
        }

        public override void HandleInput()
        {
            const int speed = 5;
            if (SplashKit.KeyDown(KeyCode.AKey))
            {
                X -= speed;
            }
            if (SplashKit.KeyDown(KeyCode.DKey))
            {
                X += speed;
            }
            if (SplashKit.KeyDown(KeyCode.EscapeKey))
            {
                Quit = true;
            }
        }
    }

    public class Player2 : Player
    {
        public Player2(Window gameWindow) : base(gameWindow, "Player2")
        {
            X = (gameWindow.Width / 4) * 2.6; 
            Y = gameWindow.Height - _PlayerBitmap.Height - 10;
        }

        public override void HandleInput()
        {
            const int speed = 5;
            if (SplashKit.KeyDown(KeyCode.LeftKey))
            {
                X -= speed;
            }
            if (SplashKit.KeyDown(KeyCode.RightKey))
            {
                X += speed;
            }
            if (SplashKit.KeyDown(KeyCode.EscapeKey))
            {
                Quit = true;
            }
        }
    }
}