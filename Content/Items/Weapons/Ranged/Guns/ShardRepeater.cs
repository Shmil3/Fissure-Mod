using FissureMod.Content.Items.Metarial;
using FissureMod.Content.Projectiles.Bullets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Content.Items.Weapons.Ranged.Guns
{
    public class ShardRepeater : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 20;
            Item.scale = 1f;

            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 2f;
            Item.crit = 2; 

            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true; 
            Item.autoReuse = true;

            Item.UseSound = SoundID.Item11; 

            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Bullet; 

            Item.rare = ItemRarityID.Green;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(5f,-2f); 
        }
        public int comboIndex = 0;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            comboIndex++;
            if (comboIndex >= 3) 
            {
                type = ModContent.ProjectileType<ShardBulletProjectile>();
                comboIndex = 0;
            }
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f; 
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient(ModContent.ItemType<FissureShard>(), 12)
               .AddIngredient(ItemID.Wood, 15)
               .AddIngredient(ItemID.SilverBar, 7)
               .AddTile(TileID.Anvils)
               .Register();

            CreateRecipe()
              .AddIngredient(ModContent.ItemType<FissureShard>(), 12)
              .AddIngredient(ItemID.Wood, 15)
              .AddIngredient(ItemID.TungstenBar, 7)
              .AddTile(TileID.Anvils)
              .Register();
        }
    }
}
