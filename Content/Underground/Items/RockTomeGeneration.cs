using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityVanilla.Content.Underground.Items;

public class RockTomeGeneration : GenPass
{
    public RockTomeGeneration(string name, double loadWeight) : base(name, loadWeight)
    {

    }
    static List<Point> tiles = new List<Point>();
    const int limitDist = 50;
    private void TryToPlaceRockTomeObjects(int x, int y)
    {
        if (Main.tile[x, y + 1].TileType is TileID.Granite or TileID.GraniteBlock && !WorldGen.SolidOrSlopedTile(Main.tile[x, y]))
        {
            if (Main.rand.NextBool(30) && !tiles.Any(t => new Vector2(t.X - x, t.Y - y).LengthSquared() < limitDist * limitDist))
            {
                WorldGen.PlaceTile(x, y, ModContent.TileType<GraniteTomeObject>(), true, false);
                tiles.Add(new Point(x, y));
            }
        }
        else if (Main.tile[x, y + 1].TileType is TileID.Marble or TileID.MarbleBlock && WorldGen.SolidOrSlopedTile(Main.tile[x, y + 1]))
        {
            if (Main.rand.NextBool(30) && !tiles.Any(t => new Vector2(t.X - x, t.Y - y).LengthSquared() < limitDist * limitDist))
            {
                for (int j = -1; j < 2; j++)
                {
                    for (int k = -2; k < 1; k++)
                    {
                        if (WorldGen.SolidOrSlopedTile(Main.tile[x + j, y + k]))
                        {
                            return;
                        }
                    }
                }
                for (int j = -1; j < 2; j++)
                {
                    WorldGen.PlaceTile(x + j, y + 1, TileID.Marble, true, true);
                }
                WorldGen.PlaceTile(x, y, ModContent.TileType<MarbleTomeObject>(), true, false);
                tiles.Add(new Point(x, y));
            }
        }
    }
    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = RockTomeObjectSystem.RockTomeMessage.Value;
        for (int x = 20; x < Main.maxTilesX - 20; x++)
        {
            for (int y = 100; y < Main.maxTilesY - 20; y++)
            {
                TryToPlaceRockTomeObjects(x, y);
            }
        }
    }
}

public class RockTomeObjectSystem : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        int index = tasks.FindIndex(GenPass => GenPass.Name.Equals("Traps"));
        tasks.Insert(index + 1, new RockTomeGeneration("RockTomeObjects", 100f));
    }
    public static LocalizedText RockTomeMessage { get; private set; }
    public override void SetStaticDefaults()
    {
        RockTomeMessage = Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(RockTomeMessage)}"));
    }
}