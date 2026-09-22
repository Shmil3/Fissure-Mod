using FissureMod.Content.Items;
using FissureMod.Content.Projectiles.Boss_Projectiles.Shardozaurus;
using FissureMod.Content.Projectiles.Enemy_Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace FissureMod.Content.NPCs.Bosses.Shardozaurus
{
    [AutoloadBossHead]
    public class Shardozaurus : ModNPC
    {
        private Vector2 storedVelocity;
        public enum ActionState
        {
            PrepareCharge,
            Charging,
            Barrage,
            TailStrike
        }
        public ActionState State
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public ref float Attack_Timer => ref NPC.ai[1];

        //---- LOCAL ATTACK TIMERS ----------------------
        public ref float Charge_Timer => ref NPC.localAI[0];
        public ref float Barrage_Timer => ref NPC.localAI[1];

        public ref float Tail_Timer => ref NPC.localAI[2];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 20;

            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.CountsAsCritter[Type] = false;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        }
        public override void SetDefaults()
        {
            NPC.npcSlots = 6f;
            NPC.width = 180;
            NPC.height = 115;
            NPC.stepSpeed = 3f;

            NPC.damage = 15;
            NPC.defense = 8;
            NPC.lifeMax = 1750;

            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = false;

            NPC.HitSound = SoundID.NPCHit31;
            NPC.DeathSound = SoundID.NPCDeath34;
            NPC.value = Item.buyPrice(silver: 1, copper: 20);

            State = ActionState.PrepareCharge;
            Charge_Timer = 0;

            Banner = Item.NPCtoBanner(NPCID.DesertBeast);
            BannerItem = Item.BannerToItem(Banner);
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        { 
            storedVelocity = NPC.velocity;  
        }


        public override void AI()
        {
            // --- PLAYER TARGETING AND DISPAWN BEHAVIOR -------------------------------------------
            NPC.TargetClosest(faceTarget: true);
            Player targetPlayer = Main.player[NPC.target];

            if (!targetPlayer.active || targetPlayer.dead)
            {
                return;
            }

            // --- JAMPING BEFORE HITTING TILE -----------------------------------------------------

            if ((NPC.direction == 1 && NPC.velocity.X >= 0) || NPC.direction == -1 && NPC.velocity.X <=0) 
            if (NPC.velocity.Y == 0f)
            {
                int tilesAhead = 5;
                float frontFootX = NPC.Center.X + ((NPC.width / 2f) * NPC.direction);
                int footTileY = (int)(NPC.Bottom.Y / 16f) - 1;

                bool foundLedge = false;
                int ledgeHeight = 0;

                for (int step = 1; step <= tilesAhead; step++)
                {
                    int checkTileX = (int)((frontFootX + (step * 16f * NPC.direction)) / 16f);

                    for (int h = 0; h <= 3; h++)
                    {
                        Tile tile = Framing.GetTileSafely(checkTileX, footTileY - h);

                        if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                        {
                            foundLedge = true;
                            ledgeHeight = Math.Max(ledgeHeight, h + 1);
                        }
                    }

                    if (foundLedge)
                    {
                        int headTileX = (int)(NPC.Center.X / 16f);
                        int headTileY = (int)(NPC.Top.Y / 16f) - 2;
                        Tile ceilingTile = Framing.GetTileSafely(headTileX, headTileY);

                        if (!ceilingTile.HasTile || !Main.tileSolid[ceilingTile.TileType])
                        {
                            NPC.velocity.Y = -5.5f - (ledgeHeight * 0.8f);

                            NPC.velocity.X = NPC.direction * Math.Max(Math.Abs(NPC.velocity.X), 5.5f);
                        }
                        break; 
                    }
                }
            }

            if (NPC.Center.Y - targetPlayer.Center.Y > 250) 
            {
                for (int i = 0; i < 40; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Bottom, 50, 5, DustID.Copper, 0f, 0f, 50, default, 1f);
                    dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                    dust.noGravity = false;
                }
                NPC.velocity = new(NPC.direction * 3, -10);
            }

            //---- CHOOSING ATTACK ------------------------------------------------------------

            Attack_Timer++;
            Charge_Timer++;

            if (Charge_Timer >= 60 && State == ActionState.PrepareCharge)
            {
                State = ActionState.Charging;
            }

            if (Attack_Timer >= 750 && State == ActionState.PrepareCharge) 
            {
                Attack_Timer = 0f;
                Barrage_Timer = 0;
                State = ActionState.Barrage;
            }

            if (Attack_Timer >= 200 && State == ActionState.Barrage) 
            {
                Attack_Timer = 0;
                State = ActionState.TailStrike;
            }
            if (Attack_Timer >= 200 && State == ActionState.TailStrike) 
            {
                Attack_Timer = 0;
                State = ActionState.PrepareCharge;
            }
      
            

            // ---- CALL ATTACK AI -----------------

            if (State == ActionState.Charging) 
            {
                ChargeAttack();
            }
            if (State == ActionState.PrepareCharge)
            {
                NPC.velocity.X = 0;
            }
            if (State == ActionState.Barrage) 
            {
                BarrageAttack();
            }
            if (State == ActionState.TailStrike) 
            {
                TailAttack();
            }
        }


        /// -------------------------------------------------ATTACKING AI FUNCTION ----------------------------


        public void ChargeAttack()
        {
            NPC.velocity.X += 0.4f * NPC.direction;
            if (NPC.velocity.X >= 20f) { NPC.velocity.X -= 0.40f; }
            if (NPC.velocity.X <= -20f) { NPC.velocity.X += 0.40f; }
            if (NPC.velocity.X > -0.39f && NPC.velocity.X < 0.39f) 
            {
                State = ActionState.PrepareCharge;
                Charge_Timer = 0;
            }
            if (Main.rand.Next(1, 3) == 2)
            {
                Dust dust = Dust.NewDustDirect(NPC.Bottom, NPC.width/2, -5, DustID.Dirt, 0f, 0f, 50, default, 1f);
                dust.velocity = Main.rand.NextVector2Circular(-3f, 3f);
                dust.noGravity = true;
            }
        }

        public void BarrageAttack() 
        {
            NPC.velocity.X = 0;
            Barrage_Timer++;
            Player player = Main.player[NPC.target];
            if (Barrage_Timer == 45) 
            { 
            
                SoundEngine.PlaySound(SoundID.Item61 with { Pitch = -0.1f, Volume = 0.85f }, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int shardCount = Main.rand.Next(3, 6);

                    for (int i = 0; i < shardCount; i++)
                    {
                        float horizontalSpeed = (player.Center.X - NPC.Center.X) / 90f + Main.rand.NextFloat(-1.5f, 1.5f);
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
                    
                    Barrage_Timer = 0;
                }
            }
        }

        public void TailAttack() 
        {
            NPC.direction = (Main.player[NPC.target].Center.X > NPC.Center.X) ? -1 : 1;
            NPC.spriteDirection = NPC.direction;

            NPC.velocity.X = 0;
                
            Tail_Timer++;
            if (Tail_Timer >= 30 && NPC.frame.Y == 8 * 176) 
            {
                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Bottom + new Vector2(-NPC.direction * 160, -25),
                    new Vector2(-NPC.direction *0.1f, 0),
                    ModContent.ProjectileType<Shardozaurus_Wave>(),
                    25,
                    5f);

                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Bottom + new Vector2(-NPC.direction * 160, -25), 5, -5, DustID.Dirt, 0f, 0f, 50, default, 1f);
                    dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                    dust.noGravity = Main.rand.NextBool();
                }
                Tail_Timer = 0;
            }
        }




        // ------------------------------------- ANIMATION -------------------------------------------------------------------
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (State == ActionState.Charging)
            {
                Texture2D texture = TextureAssets.Npc[NPC.type].Value;
                Vector2 drawOrigin = NPC.frame.Size() / 2f;

                SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                for (int i = 1; i < NPC.oldPos.Length; i++)
                {
                    if (NPC.oldPos[i] == Vector2.Zero)
                        continue;

                    Vector2 drawPos = NPC.oldPos[i] - screenPos
                         + new Vector2(NPC.width * 0.5f, NPC.height - NPC.frame.Height * 0.5f * NPC.scale)
                         + new Vector2(0f, NPC.gfxOffY);

                    float progress = (float)(NPC.oldPos.Length - i) / NPC.oldPos.Length;
                    Color trailColor = NPC.GetAlpha(drawColor) * progress * 0.6f;

                    spriteBatch.Draw(
                        texture,
                        drawPos,
                        NPC.frame,
                        trailColor,
                        NPC.rotation,
                        drawOrigin,
                        NPC.scale,
                        effects,
                        0f
                    );
                }
            }

            return true;
        }


        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = -NPC.direction;
            if (State == ActionState.Charging)
            {
              if(NPC.velocity.X >0.5f || NPC.velocity.X <-0.5f)
              {
                  if (NPC.frame.Y < 9* frameHeight)
                  {
                      NPC.frame.Y = 9 * frameHeight;
                  }
                  NPC.frameCounter++;
                  if (NPC.frameCounter >= 5)
                  {
                      NPC.frame.Y += frameHeight;
                      NPC.frameCounter = 0;
                      if (NPC.frame.Y >= 13 * frameHeight)
                      {
                          NPC.frame.Y = 9 * frameHeight;
                      }
                  }
              }
            }
            if (State == ActionState.PrepareCharge)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 9)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0;
                    if (NPC.frame.Y >= 5 * frameHeight)
                    {
                        NPC.frame.Y = 1 * frameHeight;
                    }
                }
            }
            if (State == ActionState.Barrage)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            if (State == ActionState.TailStrike) 
            {
                if (NPC.frame.Y < 4 * frameHeight || NPC.frame.Y > 8 * frameHeight)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                NPC.frameCounter++;
                if (NPC.frameCounter >= 10) 
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0;
                    if (NPC.frame.Y >= 9 * frameHeight)
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }   
                }
            }
        }


        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            NPC.velocity = storedVelocity;
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            NPC.velocity = storedVelocity;
        }

    }
}
