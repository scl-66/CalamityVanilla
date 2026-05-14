using System;
using System.Collections.Generic;
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
    private void TryToPlaceRockTomeObjects(int x, int y)
    {
        if (Main.tile[x, y + 1].TileType == TileID.Granite)
        {
            if (Main.rand.NextBool(23))
            {
                WorldGen.PlaceTile(x, y, ModContent.TileType<GraniteTomeObject>(), true, false);
            }
        }
        else if (Main.tile[x, y + 1].TileType == TileID.Marble)
        {
            if (Main.rand.NextBool(23))
            {
                WorldGen.PlaceTile(x, y, ModContent.TileType<MarbleTomeObject>(), true, false);
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
        int index = tasks.FindIndex(GenPass => GenPass.Name.Equals("Piles"));
        tasks.Insert(index + 1, new RockTomeGeneration("RockTomeObjects", 100f));
    }
    public static LocalizedText RockTomeMessage { get; private set; }
    public override void SetStaticDefaults()
    {
        RockTomeMessage = Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(RockTomeMessage)}"));
    }
}
