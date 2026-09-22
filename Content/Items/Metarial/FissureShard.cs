using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Content.Items.Metarial
{
    public class FissureShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 10;
            Item.maxStack = 9999;
            Item.value = 150;
            Item.rare = ItemRarityID.White;
            Item.scale = 0.75f;
        }

      
    }
}
