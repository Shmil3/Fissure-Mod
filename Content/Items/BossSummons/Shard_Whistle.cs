using FissureMod.Content.Items.Metarial;
using FissureMod.Content.NPCs.Bosses;
using FissureMod.Content.NPCs.Bosses.Shardozaurus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Content.Items.BossSummons
{
    public class Shard_Whistle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 20;
            Item.value = 1000;
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ZoneDesert && !NPC.AnyNPCs(ModContent.NPCType<Shardozaurus>());
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<Shardozaurus>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnBoss(
                        (int)player.Center.X +player.direction * 450,
                        (int)player.Center.Y - 100,
                        ModContent.NPCType<Shardozaurus>(),
                        player.whoAmI);
                }
            }

            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<FissureShard>(), 10)
                .AddIngredient(ItemID.SandBlock, 15)
                .AddIngredient(ItemID.IronBar, 8)
                .AddTile(TileID.DemonAltar)
                .Register();

            CreateRecipe()
                .AddIngredient(ModContent.ItemType<FissureShard>(), 10)
                .AddIngredient(ItemID.SandBlock, 15)
                .AddIngredient(ItemID.LeadBar, 8)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
