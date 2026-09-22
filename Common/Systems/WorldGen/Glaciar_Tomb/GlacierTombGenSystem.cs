using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using FissureMod.Content.Tiles.GlacierTomb;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using FissureMod.Content.Tiles.Ores;

namespace FissureMod.Common.Systems.WorldGen.Glaciar_Tomb
{
    public class GlacierTombGenSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int microBiomesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));

            if (microBiomesIndex != -1)
            {
                tasks.Insert(microBiomesIndex + 1, new PassLegacy("Glacier Tomb Carve", (progress, configuration) =>
                {
                    progress.Message = "Sculpting the Glacier Tomb...";
                    CarveGlacierTomb();
                }));
            }
        }

        private void CarveGlacierTomb()
        {
            if (GenVars.snowMinX == null || GenVars.snowMaxX == null)
                return;

            int radiusX = 150;
            int radiusY = 50;
            int borderThickness = 6;

            // 1. Placement depth locked in lower Underground
            int originY = (int)GenVars.rockLayer - 20;
            originY = Math.Clamp(originY, 0, GenVars.snowMinX.Length - 1);

            // 2. Center within the snow slice
            int sliceMinX = GenVars.snowMinX[originY];
            int sliceMaxX = GenVars.snowMaxX[originY];
            int originX = sliceMinX + ((sliceMaxX - sliceMinX) / 2);

            // 3. Dungeon clearance
            int dungeonX = GenVars.dungeonX;
            if (Math.Abs(originX - dungeonX) < radiusX + borderThickness + 25)
            {
                int pushDirection = (originX < Main.maxTilesX / 2) ? 1 : -1;
                originX += pushDirection * ((radiusX + borderThickness + 25) - Math.Abs(originX - dungeonX));
            }

            Mod.Logger.Info($"[Glacier Tomb] Sculpting expanded chamber at: X = {originX}, Y = {originY}");

            int searchBoundX = radiusX + borderThickness + 6;
            int searchBoundY = radiusY + borderThickness + 6;

            // --- PHASE 1: CARVE ORGANIC HOLLOW CAVITY & BORDER ---
            for (int x = originX - searchBoundX; x <= originX + searchBoundX; x++)
            {
                for (int y = originY - searchBoundY; y <= originY + searchBoundY; y++)
                {
                    if (x < 10 || x >= Main.maxTilesX - 10 || y < 10 || y >= Main.maxTilesY - 10)
                        continue;

                    Tile tile = Main.tile[x, y];

                    if (Main.wallDungeon[tile.WallType] ||
                        tile.TileType == TileID.BlueDungeonBrick ||
                        tile.TileType == TileID.GreenDungeonBrick ||
                        tile.TileType == TileID.PinkDungeonBrick ||
                        tile.TileType == TileID.LihzahrdBrick)
                    {
                        continue;
                    }

                    float dx = x - originX;
                    float dy = y - originY;
                    double angle = Math.Atan2(dy, dx);

                    float distortion = 1.0f
                        + 0.08f * (float)Math.Sin(angle * 6.0)
                        + 0.04f * (float)Math.Cos(angle * 10.0);

                    float effRx = radiusX * distortion;
                    float effRy = radiusY * distortion;

                    float normalizedDist = (dx * dx) / (effRx * effRx) + (dy * dy) / (effRy * effRy);
                    float borderLimit = (float)Math.Pow((effRx + borderThickness) / effRx, 2);

                    if (normalizedDist <= 1.0f)
                    {
                        tile.HasTile = false;
                        tile.LiquidAmount = 0;
                        tile.WallType = WallID.None;
                    }
                    else if (normalizedDist <= borderLimit)
                    {
                        tile.HasTile = true;
                        tile.TileType = TileID.IceBlock;
                        tile.WallType = WallID.IceUnsafe;
                    }
                }
            }

            // --- PHASE 2: RESCALED STALACTITES ---
            int stalactiteCount = 5;
            int usableSpan = (int)(radiusX * 1.15f);
            int startSpanX = originX - (usableSpan / 2);
            int stepX = usableSpan / (stalactiteCount + 1);

            for (int s = 1; s <= stalactiteCount; s++)
            {
                int anchorX = startSpanX + (s * stepX) + Terraria.WorldGen.genRand.Next(-6, 7);

                int ceilingY = originY - radiusY - borderThickness;
                while (ceilingY < originY && Main.tile[anchorX, ceilingY].HasTile)
                {
                    ceilingY++;
                }

                int startY = ceilingY - 3;
                int stalactiteLength = Terraria.WorldGen.genRand.Next(20, 34);
                int baseWidth = Terraria.WorldGen.genRand.Next(8, 13);

                for (int h = 0; h < stalactiteLength; h++)
                {
                    int currentY = startY + h;
                    float progress = (float)h / stalactiteLength;
                    int currentRadius = (int)Math.Round(baseWidth * (1f - progress));

                    for (int w = -currentRadius; w <= currentRadius; w++)
                    {
                        int currentX = anchorX + w;

                        if (currentX >= 10 && currentX < Main.maxTilesX - 10 && currentY >= 10 && currentY < Main.maxTilesY - 10)
                        {
                            Tile tile = Main.tile[currentX, currentY];
                            tile.HasTile = true;
                            tile.TileType = TileID.IceBlock;
                            tile.WallType = WallID.IceUnsafe;
                        }
                    }
                }
            }

            // --- PHASE 3: LEFT-ALIGNED POND & DRY RIGHT TOMB PLATEAU ---
            int floorBaseline = originY + radiusY - 16;
            int pondLeft = originX - 110;
            int pondRight = originX - 20;
            int pondMaxDepth = 10;

            for (int x = originX - radiusX; x <= originX + radiusX; x++)
            {
                if (x < 10 || x >= Main.maxTilesX - 10)
                    continue;

                int targetFloorY = floorBaseline + (int)(Math.Sin(x * 0.08) * 2);
                int pondDip = 0;
                bool inPond = false;

                if (x >= pondLeft && x <= pondRight)
                {
                    float t = (float)(x - pondLeft) / (pondRight - pondLeft);
                    pondDip = (int)(Math.Sin(t * Math.PI) * pondMaxDepth);
                    inPond = true;
                }

                int waterBedY = targetFloorY + pondDip;
                int scanStartY = originY + radiusY + borderThickness;

                for (int y = scanStartY; y >= targetFloorY; y--)
                {
                    if (y < 10 || y >= Main.maxTilesY - 10)
                        continue;

                    float dx = x - originX;
                    float dy = y - originY;
                    double angle = Math.Atan2(dy, dx);
                    float distortion = 1.0f + 0.08f * (float)Math.Sin(angle * 6.0) + 0.04f * (float)Math.Cos(angle * 10.0);
                    float effRx = radiusX * distortion;
                    float effRy = radiusY * distortion;

                    if ((dx * dx) / (effRx * effRx) + (dy * dy) / (effRy * effRy) <= 1.0f)
                    {
                        Tile tile = Main.tile[x, y];

                        if (inPond && y < waterBedY)
                        {
                            tile.HasTile = false;
                            tile.LiquidType = LiquidID.Water;
                            tile.LiquidAmount = 255;
                            tile.WallType = WallID.None;
                        }
                        else
                        {
                            tile.HasTile = true;
                            tile.TileType = TileID.IceBlock;
                            tile.LiquidAmount = 0;
                            tile.WallType = WallID.IceUnsafe;
                        }
                    }
                }
            }

            // --- PHASE 3.5: SUSPENDED ICE LEDGES (WEST + CENTER) ---
            (int xOff, int yOff, int width, int thickness)[] shelves = new[]
            {
                (-95, 12, 38, 6),  // Low shelf directly over the pond
                (-55, -4, 40, 7),  // Mid shelf bridging west to center
                (-90, -18, 30, 5), // High shelf under the leftmost glaciers
                (-12, -6, 36, 6),  // Mid-Center shelf matching height of mid-west shelf
                (25, -20, 32, 5)   // High-Center shelf matching height of top-west shelf
            };

            foreach (var (xOff, yOff, width, maxThick) in shelves)
            {
                int shelfCenterX = originX + xOff;
                int shelfCenterY = originY + yOff;
                int halfW = width / 2;

                for (int sx = shelfCenterX - halfW; sx <= shelfCenterX + halfW; sx++)
                {
                    float progress = (float)(sx - (shelfCenterX - halfW)) / width;
                    int currentThickness = (int)(Math.Sin(progress * Math.PI) * maxThick);

                    for (int sy = shelfCenterY; sy <= shelfCenterY + currentThickness; sy++)
                    {
                        if (sx >= 10 && sx < Main.maxTilesX - 10 && sy >= 10 && sy < Main.maxTilesY - 10)
                        {
                            Tile tile = Main.tile[sx, sy];
                            tile.ResetToType(TileID.IceBlock);
                            tile.WallType = WallID.None;
                        }
                    }
                }
            }

            // --- PHASE 3.6: HOURGLASS GLACIAL PILLAR ---
            int pillarCenterX = originX + 25;
            int pillarTopY = originY - 20;
            int pillarBottomY = floorBaseline + 2;

            int columnHeight = pillarBottomY - pillarTopY;
            int waistRadius = 8;
            int capitalRadius = 16;
            int baseRadius = 15;

            for (int y = pillarTopY; y <= pillarBottomY; y++)
            {
                if (y < 10 || y >= Main.maxTilesY - 10)
                    continue;

                float t = (float)(y - pillarTopY) / columnHeight;
                float blend = (t < 0.45f) ? (1f - (t / 0.45f)) : ((t - 0.45f) / 0.55f);
                int currentRadius = waistRadius + (int)(blend * blend * ((t < 0.45f) ? (capitalRadius - waistRadius) : (baseRadius - waistRadius)));
                currentRadius += (int)(Math.Sin(y * 0.35) * 1.2);

                for (int px = -currentRadius; px <= currentRadius; px++)
                {
                    int x = pillarCenterX + px;

                    if (x >= 10 && x < Main.maxTilesX - 10)
                    {
                        Tile tile = Main.tile[x, y];
                        tile.ResetToType(TileID.IceBlock);
                        tile.WallType = WallID.None;
                    }
                }
            }

            // --- PHASE 4: GLACIER ORE VEINS ---
            int glacierOreID = ModContent.TileType<GlacierOreTile>();

            for (int k = 0; k < 35; k++)
            {
                int oreX = originX + Terraria.WorldGen.genRand.Next(-radiusX + 6, radiusX - 6);
                int oreY = originY + Terraria.WorldGen.genRand.Next(-radiusY - borderThickness, radiusY + borderThickness);

                if (oreX >= 10 && oreX < Main.maxTilesX - 10 && oreY >= 10 && oreY < Main.maxTilesY - 10)
                {
                    if (Main.tile[oreX, oreY].HasTile && Main.tile[oreX, oreY].TileType == TileID.IceBlock)
                    {
                        Terraria.WorldGen.TileRunner(
                            oreX, oreY,
                            Terraria.WorldGen.genRand.Next(5, 9),
                            Terraria.WorldGen.genRand.Next(6, 13),
                            glacierOreID,
                            false, 0f, 0f, false, true
                        );
                    }
                }
            }

            // --- PHASE 5: SUNKEN CHEST AT LAKE BED ---
            int pondMidX = (pondLeft + pondRight) / 2;
            int chestScanY = floorBaseline;

            while (chestScanY < originY + radiusY + borderThickness &&
                  (!Main.tile[pondMidX, chestScanY].HasTile || Main.tile[pondMidX, chestScanY].LiquidAmount > 0))
            {
                chestScanY++;
            }

            Tile baseLeft = Main.tile[pondMidX, chestScanY];
            baseLeft.ResetToType(TileID.IceBlock);

            Tile baseRight = Main.tile[pondMidX + 1, chestScanY];
            baseRight.ResetToType(TileID.IceBlock);

            int chestY = chestScanY - 1;
            for (int cx = 0; cx <= 1; cx++)
            {
                for (int cy = -1; cy <= 0; cy++)
                {
                    Tile chestTile = Main.tile[pondMidX + cx, chestScanY + cy];
                    chestTile.ClearTile();
                }
            }

            int chestIndex = Terraria.WorldGen.PlaceChest(pondMidX, chestY, TileID.Containers, false, 11);

            if (chestIndex != -1)
            {
                Chest chest = Main.chest[chestIndex];
                int slot = 0;

                chest.item[slot++].SetDefaults(Terraria.WorldGen.genRand.NextBool() ? ItemID.BlizzardinaBottle : ItemID.IceMirror);
                chest.item[slot++].SetDefaults(ItemID.IceBlade);
                chest.item[slot++].SetDefaults(ItemID.FrostburnArrow);
                chest.item[slot - 1].stack = Terraria.WorldGen.genRand.Next(40, 75);
                chest.item[slot++].SetDefaults(ItemID.WarmthPotion);
                chest.item[slot - 1].stack = Terraria.WorldGen.genRand.Next(2, 4);
                chest.item[slot++].SetDefaults(ItemID.GoldCoin);
                chest.item[slot - 1].stack = Terraria.WorldGen.genRand.Next(1, 3);
            }

            // --- PHASE 6: AMBIENT DECALS & HANGING ICICLES ---
            for (int x = originX - radiusX; x <= originX + radiusX; x++)
            {
                for (int y = originY - radiusY; y <= originY + radiusY; y++)
                {
                    if (x < 10 || x >= Main.maxTilesX - 10 || y < 10 || y >= Main.maxTilesY - 10)
                        continue;

                    Tile tile = Main.tile[x, y];
                    Tile tileBelow = Main.tile[x, y + 1];

                    if (tile.HasTile && tile.TileType == TileID.IceBlock && !tileBelow.HasTile)
                    {
                        if (Terraria.WorldGen.genRand.NextBool(3))
                        {
                            Terraria.WorldGen.PlaceTile(x, y + 1, TileID.Stalactite, true, false, -1, 2);
                        }
                    }

                    if (x > pondRight && !tile.HasTile && tileBelow.HasTile && tileBelow.TileType == TileID.IceBlock)
                    {
                        if (Terraria.WorldGen.genRand.NextBool(8))
                        {
                            Terraria.WorldGen.PlaceTile(x, y, TileID.SmallPiles, true, false, -1, 18);
                        }
                    }
                }
            }

            // --- PHASE 6.5: PLACE SANCTUM TOMB ON EASTERN DRY ARENA ---
            PlaceEasternGlacierTomb(originX, floorBaseline);

            // --- PHASE 7: BATCH FRAMING ---
            for (int x = originX - searchBoundX; x <= originX + searchBoundX; x++)
            {
                for (int y = originY - searchBoundY; y <= originY + searchBoundY; y++)
                {
                    if (x >= 10 && x < Main.maxTilesX - 10 && y >= 10 && y < Main.maxTilesY - 10)
                    {
                        Terraria.WorldGen.SquareTileFrame(x, y, true);
                        Framing.WallFrame(x, y, true);
                    }
                }
            }
        }

        private void PlaceEasternGlacierTomb(int originX, int floorBaseline)
        {
            int targetX = originX + 65;

            int targetY = floorBaseline - 10;
            while (targetY < Main.maxTilesY - 20 && (!Main.tile[targetX, targetY].HasTile || !Main.tileSolid[Main.tile[targetX, targetY].TileType]))
            {
                targetY++;
            }

            int placeX = targetX;
            int placeY = targetY - 1;

            int tombWidth = 8;
            int tombHeight = 5;
            int originCol = 3;
            int originRow = 4;

            int leftX = placeX - originCol;
            int rightX = leftX + tombWidth - 1;
            int topY = placeY - originRow;
            int bottomY = placeY;

            for (int x = leftX; x <= rightX; x++)
            {
                for (int y = bottomY + 1; y <= bottomY + 2; y++)
                {
                    Tile foundationTile = Main.tile[x, y];
                    foundationTile.ResetToType(TileID.IceBlock);
                    foundationTile.Slope = SlopeType.Solid;
                    foundationTile.IsHalfBlock = false;
                }
            }

            for (int x = leftX - 1; x <= rightX + 1; x++)
            {
                for (int y = topY - 1; y <= bottomY; y++)
                {
                    Tile clearTile = Main.tile[x, y];
                    clearTile.HasTile = false;
                    clearTile.LiquidAmount = 0;
                    clearTile.WallType = WallID.None;
                }
            }

            ushort tombTileType = (ushort)ModContent.TileType<GlacierTombTile>();
            bool success = Terraria.WorldGen.PlaceObject(placeX, placeY, tombTileType, mute: true, style: 0);

            if (!success)
            {
                Mod.Logger.Warn($"[Glacier Tomb] WorldGen failed to place GlacierTombTile at ({placeX}, {placeY}).");
            }
            else
            {
                Mod.Logger.Info($"[Glacier Tomb] Successfully erected Sanctum Tomb at ({placeX}, {placeY})!");
                NetMessage.SendObjectPlacement(-1, placeX, placeY, tombTileType, 0, 0, -1, -1);
            }
        }
    }
}