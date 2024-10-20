using Godot;
using System;


namespace IsekaiPlatformerGD4.Scripts
{
    public class GameTimer
    {
        public static string Time { get; private set; } = "";

        private static string GetFormattedTime(float gameTime)
        {
            int minutes = (int)gameTime / 60;
            int seconds = (int)gameTime % 60;
            int milliseconds = (int)((gameTime - Math.Floor(gameTime)) * 100);
            return $"{minutes:D2}:{seconds:D2}:{milliseconds:D2}";
        }

        public static void OnGameFinish(float gameTime)
        {
            string finalTime = GetFormattedTime(gameTime);
            GD.Print("You finished the level in: " + finalTime);
            Time = finalTime;
            // Do other tasks like saving the time, etc.
        }
    }
}
