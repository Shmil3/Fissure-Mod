using FissureMod.Content.Items.Metarial.Glacier;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FissureMod.Content.Tiles.Ores
{
    public class GlacierOreTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 255;
            Main.tileShine[Type] = 975;
            Main.tileShine2[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            MineResist = 2.0f;
            MinPick = 50;

            RegisterItemDrop(ModContent.ItemType<GlacierOre>());
            AddMapEntry(new Color(200, 200, 255), CreateMapEntryName());

            HitSound = SoundID.Tink;
            DustType = DustID.GemDiamond;
        }
    }
}
