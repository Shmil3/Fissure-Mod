using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Content.Items.Metarial.Glacier
{
    public class GlacierBar : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.maxStack = 9999;
            Item.material = true;
            Item.value = 2000;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GlacierOre>(), 3)
                .AddIngredient(ModContent.ItemType<FissureShard>(), 1) /// REPLACE WITH GLACIER SHARD LATER
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
