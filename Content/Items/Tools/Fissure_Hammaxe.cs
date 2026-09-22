using FissureMod.Content.Items.Metarial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Content.Items.Tools
{
    public class Fissure_Hammaxe : ModItem  
    {
        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 13;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2.5f;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.axe = 9;
            Item.hammer = 45;
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<FissureShard>(), 15)
                .AddIngredient(ItemID.Wood, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
