using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Content.Projectiles.Yoyos
{
    public class FissureYoyoProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
          
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 10f;

            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 240f;

            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 13f;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Yoyo; 
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1; 
        }
        public override void PostAI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Torch,
                    0f, 0f, 100, default, 1.2f
                );
                dust.noGravity = true;
                dust.velocity *= 0.6f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
        }
    }
}
