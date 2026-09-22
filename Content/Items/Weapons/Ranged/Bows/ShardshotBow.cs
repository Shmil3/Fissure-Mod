using FissureMod.Content.Items.Metarial;
using FissureMod.Content.Projectiles.Arrows;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Content.Items.Weapons.Ranged.Bows
{
    public class ShardshotBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 40;
            Item.scale = 1.25f;

            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 2f;

            Item.useTime = 30; 
            Item.useAnimation = 30; 
            Item.useStyle = ItemUseStyleID.Shoot; 
            Item.noMelee = true; 
            Item.autoReuse = true; 

            Item.UseSound = SoundID.Item5; 

            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 8.5f; 
            Item.useAmmo = AmmoID.Arrow;

            Item.rare = ItemRarityID.Green;
        }
        public int comboIndex = 3;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            comboIndex++;

            if (comboIndex >= 3)
            {
                type = ModContent.ProjectileType<FissureArrowProjectile>();
                comboIndex = 0;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<FissureShard>(), 10)
                .AddIngredient(ItemID.Wood, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
