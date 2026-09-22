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

namespace FissureMod.Content.Projectiles.Magic
{
    public class ShardStaffProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.ai[1]++;

            float waveFrequency = 0.12f;
            float waveAmplitude = 2.5f;

            Vector2 forwardDir = Vector2.Normalize(Projectile.velocity);
            Vector2 normal = new(-forwardDir.Y, forwardDir.X);

            float oscillation = (float)Math.Cos(Projectile.ai[1] * waveFrequency) * waveAmplitude * Projectile.ai[0];

            Projectile.position += normal * oscillation;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0.8f, 0.4f, 0.1f);

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 150, default, 1.1f);
                d.noGravity = true;
                d.velocity *= 0.2f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            for (int i = 0; i < 20; i++)
            {
                Dust fire = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.8f);
                fire.velocity *= 0.5f;
                fire.noGravity = true;

                Dust smoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, Color.DimGray, 1.2f);
                smoke.velocity *= 0.5f;
            }

            int explosionRadius = 50;
            Projectile.position = Projectile.Center;
            Projectile.width = explosionRadius;
            Projectile.height = explosionRadius;
            Projectile.Center = Projectile.position;

            Rectangle blastHitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Hitbox.Intersects(blastHitbox))
                {
                    npc.AddBuff(BuffID.OnFire, 90);
                }
            }
        }
    }
}
