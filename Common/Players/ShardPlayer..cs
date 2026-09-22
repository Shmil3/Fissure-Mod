using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FissureMod.Common.Players
{
    public class ShardPlayer : ModPlayer
    {
        public bool shardExplosionOnKill;

        public override void ResetEffects()
        {
            shardExplosionOnKill = false;
        }
    }
}
