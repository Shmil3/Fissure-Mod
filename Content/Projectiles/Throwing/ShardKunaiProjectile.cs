using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Content.Projectiles.Throwing
{
    public class ShardKunaiProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;

            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile; 
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;

            AIType = ProjectileID.ThrowingKnife;
        }
        public override void AI()
        {
            Projectile.rotation -=  MathHelper.PiOver4;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                d.noGravity = true;
                d.velocity = Main.rand.NextVector2Circular(2.5f, 2.5f);
            }
        }
    }
}
