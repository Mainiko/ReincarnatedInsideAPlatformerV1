using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsekaiPlatformerGD4.Scripts
{
    public class LevelHandeler
    {
        public static string CurrentLevel { get; set; }
        public static string NextLevel { get; set; }

        public static void UpdateLevels(string currentLevel, string nextLevel)
        {
            CurrentLevel = currentLevel;
            NextLevel = nextLevel;
        }
    }
}
