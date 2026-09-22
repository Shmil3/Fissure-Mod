using System;
using FissureMod.Content.Items.Metarial;
using FissureMod.Content.Projectiles.Enemy_Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace FissureMod.Content.NPCs
{
    public class ShardboundBasilisk : ModNPC
    {
        // State Machine enums
        private enum ActionState
        {
            Crawl,
            Telegraph,
            Fire
        }

        // Map state and timers directly to NPC.ai array for multiplayer sync
        private ref float State => ref NPC.ai[0];
        private ref float Timer => ref NPC.ai[1];
        private ref float AttackCooldown => ref NPC.ai[2];

        private const int TelegraphDuration = 50; // ~0.8s crouching down
        private const int FireDuration = 30;      // ~0.5s recovery after shooting
        private const int CooldownDuration = 300; // 5 seconds between artillery barrages

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;

            NPCID.Sets.CountsAsCritter[Type] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 24;
            NPC.scale = 1.35f;

            NPC.damage = 18;
            NPC.defense = 8;
            NPC.lifeMax = 75;
            NPC.knockBackResist = 0.35f;

            NPC.HitSound = SoundID.NPCHit31;
            NPC.DeathSound = SoundID.NPCDeath34;
            NPC.value = Item.buyPrice(silver: 1, copper: 20);

            NPC.aiStyle = NPCAIStyleID.Fighter;
            AIType = NPCID.WalkingAntlion;

            Banner = Item.NPCtoBanner(NPCID.DesertBeast);
            BannerItem = Item.BannerToItem(Banner);
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player target = Main.player[NPC.target];

            if (!target.active || target.dead)
                return;

            float distance = Vector2.Distance(NPC.Center, target.Center);

            switch ((ActionState)State)
            {
                case ActionState.Crawl:
                    // Enable Fighter AI movement
                    NPC.aiStyle = NPCAIStyleID.Fighter;
                    AIType = NPCID.WalkingAntlion;

                    // Boosted movement speed
                    float maxSpeed = 2.5f;
                    float acceleration = 0.08f;

                    if (NPC.velocity.Y == 0f)
                    {
                        if (NPC.direction == 1 && NPC.velocity.X < maxSpeed)
                            NPC.velocity.X += acceleration;
                        else if (NPC.direction == -1 && NPC.velocity.X > -maxSpeed)
                            NPC.velocity.X -= acceleration;
                    }

                    // Count down mortar cooldown
                    if (AttackCooldown > 0)
                        AttackCooldown--;

                    // Trigger Mortar Stance if grounded, off cooldown, and in medium range (150 to 450 px)
                    if (AttackCooldown <= 0 && NPC.velocity.Y == 0f && distance >= 150f && distance <= 450f)
                    {
                        State = (float)ActionState.Telegraph;
                        Timer = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Telegraph:
                    // Halt movement completely
                    NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
                    NPC.velocity.X *= 0.5f;

                    Timer++;

                    // Spark dust telegraph along the back ridge
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 spinePos = NPC.Center + new Vector2(NPC.direction * 4, -8);
                        Dust spark = Dust.NewDustDirect(spinePos, 10, 6, DustID.Copper, 0f, -2f, 100, default, 0.9f);
                        spark.noGravity = true;
                    }

                    // Switch to firing pose and unleash volley
                    if (Timer >= TelegraphDuration)
                    {
                        State = (float)ActionState.Fire;
                        Timer = 0;
                        LaunchMortarBarrage(target);
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Fire:
                    NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
                    NPC.velocity.X *= 0.5f;

                    Timer++;

                    // Recovery done, return to crawl
                    if (Timer >= FireDuration)
                    {
                        State = (float)ActionState.Crawl;
                        Timer = 0;
                        AttackCooldown = CooldownDuration;
                        NPC.netUpdate = true;
                    }
                    break;
            }
        }

        private void LaunchMortarBarrage(Player target)
        {
            SoundEngine.PlaySound(SoundID.Item61 with { Pitch = -0.1f, Volume = 0.85f }, NPC.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Fire a spread of 2 to 3 crystal mortars
                int shardCount = Main.rand.Next(2, 4);

                for (int i = 0; i < shardCount; i++)
                {
                    float horizontalSpeed = (target.Center.X - NPC.Center.X) / 90f + Main.rand.NextFloat(-1.5f, 1.5f);
                    float verticalSpeed = Main.rand.NextFloat(-15f, -10f);

                    Vector2 launchVelocity = new(horizontalSpeed, verticalSpeed);

                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        NPC.Center + new Vector2(NPC.direction * 10, -50),
                        launchVelocity,
                        ModContent.ProjectileType<ShardboundBasiliskProjectile>(),
                        25,
                        1.5f,
                        Main.myPlayer
                    );
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;

            switch ((ActionState)State)
            {
                case ActionState.Crawl:
                    if (NPC.velocity.Y != 0f)
                    {
                        NPC.frame.Y = 1 * frameHeight;
                        NPC.frameCounter = 0.0;
                        return;
                    }

                    if (NPC.velocity.X == 0f)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0.0;
                        return;
                    }

                    NPC.frameCounter += Math.Abs(NPC.velocity.X);
                    if (NPC.frameCounter >= 8.0)
                    {
                        NPC.frameCounter = 0.0;
                        NPC.frame.Y += frameHeight;

                        if (NPC.frame.Y >= 4 * frameHeight)
                        {
                            NPC.frame.Y = 0;
                        }
                    }
                    break;

                case ActionState.Telegraph:
                    if (Timer < TelegraphDuration / 2)
                    {
                        NPC.frame.Y = 4 * frameHeight; 
                    }
                    else
                    {
                        NPC.frame.Y = 5 * frameHeight;
                    }
                    break;

                case ActionState.Fire:
                    NPC.frame.Y = 6 * frameHeight;
                    break;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Mods.FissureMod.Bestiary.ShardboundBasilisk")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneDesert && spawnInfo.Player.ZoneOverworldHeight && !spawnInfo.Invasion)
            {
                return SpawnCondition.OverworldDayDesert.Chance * 0.25f;
            }

            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FissureShard>(), 1, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ItemID.FossilOre, 3, 1, 3));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int dustCount = NPC.life <= 0 ? 25 : 5;
            for (int i = 0; i < dustCount; i++)
            {
                int dustType = Main.rand.NextBool() ? DustID.Sand : DustID.Copper;
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType, hit.HitDirection * 1.5f, -1f);
                dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
            }
        }
    }
}