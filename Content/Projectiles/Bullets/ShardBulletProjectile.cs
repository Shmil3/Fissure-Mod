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

namespace FissureMod.Content.Projectiles.Bullets
{
    public class ShardBulletProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 16;
            Projectile.scale = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;

            Projectile.aiStyle = default;
            Projectile.DamageType = DamageClass.Ranged;

            AIType = default;
        }
        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Torch, 0, 0);
                dust.velocity = -Projectile.velocity * 0.1f;
                dust.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.8f, 0.4f, 0.1f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            for (int i = 0; i < 20; i++)
            {
                Dust fire = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.8f);
                fire.velocity *= 1.5f;
                fire.noGravity = true;

                Dust smoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, Color.DimGray, 1.2f);
            }

            int explosionRadius = 60;
            Projectile.position = Projectile.Center;
            Projectile.width = explosionRadius;
            Projectile.height = explosionRadius;
            Projectile.Center = Projectile.position;

            Projectile.Damage();

            Rectangle blastHitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Hitbox.Intersects(blastHitbox))
                {
                    npc.AddBuff(BuffID.OnFire, 180);
                }
            }
        }
    }
}
