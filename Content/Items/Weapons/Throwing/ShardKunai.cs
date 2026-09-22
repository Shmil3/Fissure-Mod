using FissureMod.Content.Items.Metarial;
using FissureMod.Content.Projectiles;
using FissureMod.Content.Projectiles.Throwing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Content.Items.Weapons.Throwing
{
    public class ShardKunai : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing; 
            Item.knockBack = 1f;
            Item.rare = ItemRarityID.Green;

            Item.autoReuse = true;
            Item.noUseGraphic = true; 
            Item.noMelee = true;      

            Item.shoot = ModContent.ProjectileType<ShardKunaiProjectile>();
            Item.shootSpeed = 15.0f;

            Item.consumable = true;
            Item.maxStack = 9999;
        }
        public override void AddRecipes()
        {
            CreateRecipe(30)
                .AddIngredient(ModContent.ItemType<FissureShard>(), 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
