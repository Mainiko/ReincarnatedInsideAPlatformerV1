using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsekaiPlatformerGD4.Scripts
{
    public class DeathCounter
    {
        public static int DeathCount { get; private set; } = 0;

        public static void IncrementDeathCount()
        {
            DeathCount++;
        }

        public static void ResetDeathCount()
        {
            DeathCount = 0;
        }
    }

}
