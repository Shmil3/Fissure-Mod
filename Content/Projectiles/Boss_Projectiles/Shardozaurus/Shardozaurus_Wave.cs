using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Content.Projectiles.Boss_Projectiles.Shardozaurus
{
    public class Shardozaurus_Wave : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }
        public override void AI() 
        {
            if (Projectile.velocity.X > 0) 
            {
                Projectile.direction = 1;
            }
            if (Projectile.velocity.X < 0)
            {
                Projectile.direction = -1;
            }
            Projectile.spriteDirection = Projectile.direction;
            Projectile.velocity.X += Projectile.direction * 0.1f;

            if(Main.rand.Next(1,3) ==2)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Bottom, Projectile.width, -5, DustID.Copper, 0f, 0f, 50, default, 1f);
                dust.velocity = Main.rand.NextVector2Circular(-3f, 3f);
                dust.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Copper, 0f, 0f, 50, default, 1f);
                dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                dust.noGravity = Main.rand.NextBool();
            }
        }
    }
}
