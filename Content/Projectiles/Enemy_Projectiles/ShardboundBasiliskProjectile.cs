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
    public class ShardboundBasiliskProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.22f;
            if (Projectile.velocity.Y > 12f)
            {
                Projectile.velocity.Y = 12f;
            }

            // Amber glow and dust trail
            Lighting.AddLight(Projectile.Center, 0.6f, 0.35f, 0.05f);

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Sandnado, 0f, 0f, 100, default, 0.7f);
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }

            // Tumble or rotate along velocity vector
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27 with { Pitch = -0.2f, Volume = 0.7f }, Projectile.position);

            // Shatter into crystal dust on ground impact
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Copper, 0f, 0f, 50, default, 1f);
                dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                dust.noGravity = Main.rand.NextBool();
            }
        }
    }
}
