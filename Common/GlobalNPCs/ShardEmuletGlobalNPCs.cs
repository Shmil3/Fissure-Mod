using FissureMod.Common.Players;
using FissureMod.Content.Projectiles.Enemy_Projectiles;
using FissureMod.Content.Projectiles.Throwing;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Common.GlobalNPCs
{
    public class ShardExplosionGlobalNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (npc.friendly || npc.lifeMax <= 5)
                return;
            

                if (npc.lastInteraction >= 0 && npc.lastInteraction < Main.maxPlayers)
            {
                Player killer = Main.player[npc.lastInteraction];

                if (killer.active && killer.GetModPlayer<ShardPlayer>().shardExplosionOnKill)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item27, npc.Center);
                        for (int i = 0; i < 20; i++)
                        {
                            Dust fire = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Copper, 0f, 0f, 100, default, 1.8f);
                            fire.velocity *= 1.5f;
                            fire.noGravity = true;
                        }   

                        if (Main.myPlayer == killer.whoAmI)
                        {
                            int shardProjType = ModContent.ProjectileType<ShardboundBasiliskProjectile>();
                            int damage = 15; 
                            float knockback = 3f;

                            float randomStartAngle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                            float speed = 8f;

                            for (int i = 0; i < 3; i++)
                            {
                                float angle = randomStartAngle + MathHelper.ToRadians(120f * i);
                                Vector2 velocity = angle.ToRotationVector2() * speed;

                                Projectile.NewProjectile(
                                    npc.GetSource_Death(),
                                    npc.Center,
                                    velocity,
                                    shardProjType,
                                    damage,
                                    knockback,
                                    killer.whoAmI
                                );
                            }
                        }
                    }
                }
            }
        }
    }
}
