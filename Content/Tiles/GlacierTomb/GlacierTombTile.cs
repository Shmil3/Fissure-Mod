using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace FissureMod.Content.Tiles.GlacierTomb
{
    public class GlacierTombTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);

            TileObjectData.newTile.Width = 8;
            TileObjectData.newTile.Height = 5;

            TileObjectData.newTile.Origin = new Point16(3, 4);

            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.AnchorBottom = new AnchorData(
                AnchorType.SolidTile | AnchorType.SolidWithTop,
                TileObjectData.newTile.Width,
                0
            );
            TileObjectData.newTile.LavaDeath = false;

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(130, 195, 235), Language.GetText("Mods.FissureMod.Tiles.GlacierTombTile.MapEntry"));
            DustType = DustID.Ice;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.05f;
            g = 0.20f;
            b = 0.40f;
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Main.NewText("The eye of the glacier stares into the cold abyss...", 140, 220, 255);
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconText = "The eye of the glacier stares into the cold abyss...";
        }
    }
}