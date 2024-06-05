using System;
using SplashKitSDK;

namespace SpaceDefender
{
    public class Program
    {
        public static void Main()
        {
            Window gameWindow = new Window("Space Defender Game", 800, 600);
            SpaceDefender spaceDefender = new SpaceDefender(gameWindow);
            do
            {
                SplashKit.ProcessEvents();
                spaceDefender.HandleInput();
                spaceDefender.Update();
                spaceDefender.Draw();
            } while (!spaceDefender.Quit && !gameWindow.CloseRequested);
            gameWindow.Close();
        }
    }
}