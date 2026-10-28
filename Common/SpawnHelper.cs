using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common;

public static class SpawnHelper
{
    public static bool AnyPillar(this Player player)
    {
        return player.ZoneTowerSolar || player.ZoneTowerNebula || player.ZoneTowerStardust || player.ZoneTowerVortex;
    }
    public static bool NotInRestrictedArea(ref NPCSpawnInfo spawnInfo)
    {
        return !spawnInfo.Player.ZoneDungeon && !spawnInfo.Player.AnyPillar();
    }
    public static bool Surface(ref NPCSpawnInfo spawnInfo)
    {
        return spawnInfo.SpawnTileY <= Main.worldSurface && NotInRestrictedArea(ref spawnInfo);
    }
    public static bool Underground(ref NPCSpawnInfo spawnInfo)
    {
        return spawnInfo.SpawnTileY > Main.worldSurface && NotInRestrictedArea(ref spawnInfo);
    }
    public static bool RockLayer(ref NPCSpawnInfo spawnInfo)
    {
        return spawnInfo.SpawnTileY > Main.rockLayer && NotInRestrictedArea(ref spawnInfo);
    }
    public static bool NotNoWorms(ref NPCSpawnInfo spawnInfo)
    {
        return spawnInfo.Player.afkCounter < NPC.AFKTimeNeededForNoWorms || spawnInfo.Player.ZoneShadowCandle;
    }
    public static bool Crimson(ref NPCSpawnInfo spawnInfo)
    {
        return NotInRestrictedArea(ref spawnInfo) && ((spawnInfo.Player.ZoneCrimson && spawnInfo.SpawnTileType == TileID.Crimtane)
            || spawnInfo.SpawnTileType == TileID.CrimsonGrass
            || spawnInfo.SpawnTileType == TileID.CrimsonJungleGrass
            || spawnInfo.SpawnTileType == TileID.Crimstone
            || spawnInfo.SpawnTileType == TileID.Crimsand
            || spawnInfo.SpawnTileType == TileID.FleshIce);
    }
    public static bool Corruption(ref NPCSpawnInfo spawnInfo)
    {
        return NotInRestrictedArea(ref spawnInfo) && ((spawnInfo.Player.ZoneCorrupt && spawnInfo.SpawnTileType == TileID.Demonite)
            || spawnInfo.SpawnTileType == TileID.CorruptGrass
            || spawnInfo.SpawnTileType == TileID.CorruptJungleGrass
            || spawnInfo.SpawnTileType == TileID.Ebonstone
            || spawnInfo.SpawnTileType == TileID.Ebonsand
            || spawnInfo.SpawnTileType == TileID.CorruptIce);
    }
    public static bool Hallow(ref NPCSpawnInfo spawnInfo)
    {
        return NotInRestrictedArea(ref spawnInfo) && Main.hardMode &&
            (spawnInfo.SpawnTileType == TileID.HallowedGrass
            || spawnInfo.SpawnTileType == TileID.HallowedIce
            || spawnInfo.SpawnTileType == TileID.Pearlstone
            || spawnInfo.SpawnTileType == TileID.Pearlsand);
    }
    public static bool Ocean(ref NPCSpawnInfo spawnInfo)
    {
        return NotInRestrictedArea(ref spawnInfo) && spawnInfo.Water && (spawnInfo.SpawnTileX < 250 || spawnInfo.SpawnTileX > Main.maxTilesX - 250) && Main.tileSand[spawnInfo.SpawnTileType] && spawnInfo.SpawnTileY < Main.rockLayer;
    }
    public static bool Beach(ref NPCSpawnInfo spawnInfo)
    {
        return NotInRestrictedArea(ref spawnInfo) && (spawnInfo.SpawnTileX < 340 || spawnInfo.SpawnTileX > Main.maxTilesX - 340) && Main.tileSand[spawnInfo.SpawnTileType] && spawnInfo.SpawnTileY < Main.rockLayer;
    }
    public static bool Snow(ref NPCSpawnInfo spawnInfo)
    {
        return
            NotInRestrictedArea(ref spawnInfo) && (
            spawnInfo.SpawnTileType == TileID.SnowBlock ||
            TileID.Sets.Conversion.Ice[spawnInfo.SpawnTileType]);
    }
    public static bool Desert(ref NPCSpawnInfo spawnInfo)
    {
        return
            NotInRestrictedArea(ref spawnInfo) && (
            TileID.Sets.Conversion.Sand[spawnInfo.SpawnTileType] ||
            TileID.Sets.Conversion.HardenedSand[spawnInfo.SpawnTileType] ||
            TileID.Sets.Conversion.Sandstone[spawnInfo.SpawnTileType]);
    }
    public static bool Purity(ref NPCSpawnInfo spawnInfo)
    {
        if (spawnInfo.SpawnTileY > Main.UnderworldLayer)
            return false;
        if (TileLoader.TryGetConversionFallback(spawnInfo.SpawnTileType, BiomeConversionID.Purity, out int t))
            return t == spawnInfo.SpawnTileType;
        return !Desert(ref spawnInfo) && !Snow(ref spawnInfo);
    }
}
