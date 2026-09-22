using FissureMod.Content.Items.Metarial;
using FissureMod.Content.Projectiles.Enemy_Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace FissureMod.Content.NPCs
{
    public class FissureSlime : ModNPC
    {
        private int zapTimer = 0;
        private const int ZapCooldown = 180;
        private const int TelegraphDuration = 45;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.CountsAsCritter[NPC.type] = false;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[NPC.type] = false;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 24;
            NPC.damage = 14;
            NPC.defense = 4;
            NPC.lifeMax = 60;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(copper: 60);
            NPC.knockBackResist = 0.7f;

            NPC.aiStyle = NPCAIStyleID.Slime;
            AIType = NPCID.BlueSlime;

            Banner = Item.NPCtoBanner(NPCID.BlueSlime);
            BannerItem = Item.BannerToItem(Banner);
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player target = Main.player[NPC.target];

            if (!target.active || target.dead)
                return;

            float distance = Microsoft.Xna.Framework.Vector2.Distance(NPC.Center, target.Center);

            if (distance < 700f && Collision.CanHitLine(NPC.Center, 1, 1, target.Center, 1, 1)) 
            {
                zapTimer++;

                if (zapTimer >= ZapCooldown - TelegraphDuration && zapTimer < ZapCooldown) 
                {
                    if (Main.rand.NextBool(2))
                    {
                        Microsoft.Xna.Framework.Vector2 sparkPos = NPC.Center + new Microsoft.Xna.Framework.Vector2(0, -2);
                        Dust spark = Dust.NewDustDirect(sparkPos, 4, 4, DustID.Electric, 0f, 0f, 100, Color.Orange, 0.6f);
                        spark.noGravity = true;
                        spark.velocity = Main.rand.NextVector2Circular(1.5f, 1.5f);
                    }
                }

                if (zapTimer >= ZapCooldown) 
                {
                    ShootZap(target);
                    zapTimer = 0;
                }
            }
            else
            {
                if (zapTimer > 0)
                    zapTimer--;
            }

        }

        private void ShootZap(Player target)
        {
            SoundEngine.PlaySound(SoundID.Item12 with { Pitch = 0.4f, Volume = 0.8f }, NPC.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Microsoft.Xna.Framework.Vector2 shootVelocity = 
                    (target.Center - NPC.Center).SafeNormalize(Microsoft.Xna.Framework.Vector2.UnitY) * 6.5f;
                int damage = 8;
                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    shootVelocity,
                    ModContent.ProjectileType<FissureZap>(),
                    damage,
                    1f,
                    Main.myPlayer
                );
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Mods.FissureMod.Bestiary.FissureSlime")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneDesert && spawnInfo.Player.ZoneOverworldHeight && !spawnInfo.Invasion)
            {
                return SpawnCondition.OverworldDayDesert.Chance * 2f;
            }
            return 0f;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;

            
            if (NPC.velocity.Y != 0f)
            {
                NPC.frame.Y = frameHeight;
            }
            else
            {
                NPC.frame.Y = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 1, 2));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FissureShard>(), 1, 1, 2));

            npcLoot.Add(ItemDropRule.Common(ItemID.SlimeStaff, 10000));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Sand, hit.HitDirection * 2f, -1f);
                dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
            }
        }
    }
}
