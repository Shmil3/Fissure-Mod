using FissureMod.Content.Items.Metarial;
using FissureMod.Content.Projectiles.Weapon_Projectiles;
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

namespace FissureMod.Content.Items.Weapons.Melee.Swords
{
    public class Shard_Saber : ModItem
    {
        public int ShootCount = 0;
        public override void SetDefaults()
        {
            Item.width = 45;
            Item.height = 45;
            Item.scale = 1.15f;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 14;
            Item.knockBack = 3;

            Item.value = 5000;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<Shard_Saber_Projectile>();
            Item.shootSpeed = 17f; 
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ShootCount++;
            if (ShootCount == 3) 
            {
                Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                Projectile.NewProjectile(source, position, velocity, type, 15, knockback, player.whoAmI);
                ShootCount = 0;
            }
            
           
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient(ModContent.ItemType<FissureShard>(), 12)
               .AddIngredient(ItemID.Wood, 15)
               .AddTile(TileID.Anvils)
               .Register();
        }
    }
}
