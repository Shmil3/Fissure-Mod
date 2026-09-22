using FissureMod.Content.Items.Metarial;
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

namespace FissureMod.Content.Items.Weapons.Magic.Staffs
{
    public class ShardStaff : ModItem
    {

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.scale = 1f;

            Item.damage = 14;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.knockBack = 3f;

            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.UseSound = SoundID.Item43;
            Item.shoot = ModContent.ProjectileType<Projectiles.Magic.ShardStaffProjectile>();
            Item.shootSpeed = 8f;

            Item.rare = ItemRarityID.Green;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 30f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai0: 1f);

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai0: -1f);

            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0f, -5f);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient(ModContent.ItemType<FissureShard>(), 12)
               .AddIngredient(ItemID.Ruby, 2)
               .AddTile(TileID.Anvils)
               .Register();
        }
    }
}
