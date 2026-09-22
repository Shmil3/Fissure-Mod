using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using FissureMod.Content;
using FissureMod.Content.Projectiles.Yoyos;

namespace FissureMod.Content.Items.Weapons.Melee.Yoyos
{
    public class FissureYoyo : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.width = 30;
            Item.height = 30;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.shootSpeed = 16f;
            Item.knockBack = 3.5f;
            Item.damage = 19;
            Item.rare = ItemRarityID.Blue;

            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.channel = true;      
            Item.noMelee = true;       
            Item.noUseGraphic = true;   
            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<FissureYoyoProj>();
        }
    }
}
