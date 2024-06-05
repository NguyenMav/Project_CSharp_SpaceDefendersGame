using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace SpaceDefender
{
    public class SpaceDefender
    {
        private List<Player> _Players;
        private Window _GameWindow;
        private List<Enemy> _Enemies;
        private List<PowerUp> _PowerUps;
        private Bitmap _HeartBitmap = new Bitmap("Heart", "Heart.png");
        private SplashKitSDK.Timer _Timer;
        private bool _GameOver = false;
        private bool _ScoreUpdateEnabled = true;
        private int _LastSecondScoreUpdate = 0;
        public bool Quit
        {
            get
            {
                foreach (var player in _Players)
                {
                    if (player.Quit)
                        return true;
                }
                return false;
            }
        }

        public SpaceDefender(Window gameWindow)
        {
            _GameWindow = gameWindow;
            _Players = new List<Player>();
            _Players.Add(new Player1(gameWindow));
            _Players.Add(new Player2(gameWindow));
            SplashKit.LoadMusic("BackgroundMusic", "Background.mp3");
            SplashKit.LoadSoundEffect("Shoot", "shoot.wav");
            SplashKit.LoadSoundEffect("gameover", "gameover.wav");
            SplashKit.LoadSoundEffect("killed", "killed.mp3");
            _Enemies = new List<Enemy>();
            _PowerUps = new List<PowerUp>();
            _Timer = new SplashKitSDK.Timer("Game Timer");
            SplashKit.PlayMusic("BackgroundMusic");
            _Timer.Start();
        }

        public void HandleInput()
        {
            foreach (var player in _Players)
            {
                player.HandleInput();
            }
            if (_GameOver)
            {
                if (SplashKit.KeyTyped(KeyCode.SpaceKey))
                {
                    RestartGame();
                }
                else if (SplashKit.KeyTyped(KeyCode.EscapeKey))
                {
                    SplashKit.CloseWindow(_GameWindow);
                }
            }
        }

        public void Update()
        {
            if (!_GameOver) 
            {
                foreach (var player in _Players)
                {
                    player.StayOnWindow(_GameWindow);
                    player.Update();
                }
                foreach (var enemy in _Enemies)
                {
                    enemy.Update();
                }
                foreach (var powerUp in _PowerUps)
                {
                    powerUp.Update();
                }
                if (SplashKit.Rnd() < 0.04) AddRandomEnemy();
                if (SplashKit.Rnd() < 0.001) AddRandomPowerUp();
                CheckCollisions();
                if (_ScoreUpdateEnabled)
                {
                    int currentSecond = (int)(_Timer.Ticks / 1000);
                    if (currentSecond > _LastSecondScoreUpdate)
                    {
                        foreach (var player in _Players)
                        {
                            if (player.Lives > 0)
                            {
                                player.Score += 1;
                            }
                        }
                        _LastSecondScoreUpdate = currentSecond;
                    }
                }
                if (_Players.TrueForAll(player => player.Lives <= 0))
                {
                    _GameOver = true;
                    SplashKit.PlaySoundEffect("gameover");
                    _ScoreUpdateEnabled = false; 
                }
            }
        }

        public void Draw()
        {
            Bitmap backgroundImage = new Bitmap("Background", "Background.jpg");
            backgroundImage.Draw(0, 0);
            foreach (var player in _Players)
            {
                if (player.Lives > 0)
                {
                    player.Draw();
                }
            }
            foreach (var enemy in _Enemies)
            {
                enemy.Draw();
            }
            foreach (var powerUp in _PowerUps)
            {
                powerUp.Draw();
            }
            if (_Players.Count > 0)
            {
                DrawHearts(_Players[0].Lives, 10, 10);
                SplashKit.DrawText("P1 Score: " + _Players[0].Score, Color.White, 20, 60);
            }
            if (_Players.Count > 1)
            {
                DrawHearts(_Players[1].Lives, _GameWindow.Width - (_Players[1].Lives * 50) - 10, 10);
                SplashKit.DrawText("P2 Score: " + _Players[1].Score, Color.White, _GameWindow.Width - 110, 60);
            }
            SplashKit.DrawText("Time: " + ((int)(_Timer.Ticks / 1000)).ToString(), Color.White, 370, 10);
            if (_GameOver)
            {
                Bitmap gameOverImage = new Bitmap("GameOver", "GameOver.png");
                gameOverImage.Draw((_GameWindow.Width - gameOverImage.Width) / 2, (_GameWindow.Height - gameOverImage.Height) / 2);
                SplashKit.DrawText("Press ESC to Exit", Color.White, 340, 450);
                SplashKit.DrawText("Press SPACE to Restart", Color.White, 325, 470);
            }
            _GameWindow.Refresh(60);
        }

        private void AddRandomEnemy()
        {
            if (SplashKit.Rnd() < 0.5)
            {
                _Enemies.Add(new NormalEnemy(_GameWindow));
            }
            else
            {
                _Enemies.Add(new FastEnemy(_GameWindow));
            }
        }

        private void AddRandomPowerUp()
        {
            _PowerUps.Add(new PowerUp(_GameWindow));
        }

        private void CheckCollisions()
        {
            List<Enemy> enemiesToRemove = new List<Enemy>();
            List<PowerUp> powerUpsToRemove = new List<PowerUp>();
            foreach (var enemy in _Enemies)
            {
                foreach (var player in _Players)
                {
                    if (player.Lives > 0 && player.CollideWith(enemy))
                    {
                        player.Lives--;
                        enemiesToRemove.Add(enemy);
                    }
                    if (player.Lives > 0)
                    {
                        foreach (var bullet in player.Bullets)
                        {
                            if (bullet.CollideWith(enemy))
                            {
                                enemiesToRemove.Add(enemy);
                                bullet.Deactivate();
                                player.Score += 1;
                                SplashKit.PlaySoundEffect("killed");
                            }
                        }
                    }
                }
            }
            foreach (var powerUp in _PowerUps)
            {
                foreach (var player in _Players)
                {
                    if (player.Lives > 0 && player.CollideWith(powerUp))
                    {
                        powerUp.ApplyEffect(player);
                        powerUpsToRemove.Add(powerUp);
                    }
                }
            }
            foreach (var enemy in enemiesToRemove)
            {
                _Enemies.Remove(enemy);
            }
            foreach (var powerUp in powerUpsToRemove)
            {
                _PowerUps.Remove(powerUp);
            }
            foreach (var player in _Players)
            {
                if (player.Lives <= 0)
                {
                    player.Active = false;
                    player.AllowBulletSpawn = false;
                }
            }
            foreach (var player in _Players)
            {
                player.Bullets.RemoveAll(b => !b.IsActive);
            }
        }

        private void DrawHearts(int numberOfHearts, int startX, int startY)
        {
            for (int i = 0; i < numberOfHearts; i++)
            {
                SplashKit.DrawBitmap(_HeartBitmap, startX + (i * 50), startY);
            }
        }

        private void RestartGame()
        {
            _GameOver = false;
            _ScoreUpdateEnabled = true;
            _LastSecondScoreUpdate = 0;
            foreach (var player in _Players)
            {
                player.Score = 0;
                player.Lives = 3;
            }
            _Enemies.Clear();
            _PowerUps.Clear();
            _Timer.Reset();
            SplashKit.PlayMusic("BackgroundMusic");
        }
    }
}