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

namespace FissureMod.Content.Projectiles.Enemy_Projectiles
{
    public class FissureZap : ModProjectile
    {

        private const int TotalFrames = 2;
        private const int TicksPerFrame =   10;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = TotalFrames;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 0; // Custom projectile logic
            Projectile.hostile = true; // Damages players
            Projectile.friendly = false; // Does not hurt enemies
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240; // 4 seconds before expiring
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.7f, 0.5f, 0.1f);

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position + (Vector2.UnitX *10), Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, Color.Orange, 0.7f);
                dust.noGravity = true;
                dust.velocity *= 0.2f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= TicksPerFrame)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item93 with { Pitch = 0.2f, Volume = 0.6f }, Projectile.position);

            // Electric burst on impact
            for (int i = 0; i < 12; i++)
            {
                Dust burst = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, Color.Orange, 1f);
                burst.noGravity = true;
                burst.velocity = Main.rand.NextVector2Circular(3f, 3f);
            }
        }
    }
}
