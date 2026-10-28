using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.DataSets;

[ReinitializeDuringResizeArrays]
public class NPCSets
{
    public static bool[] SlowResistant = NPCID.Sets.Factory.CreateNamedSet("SlowResistant").Description("Set for multi-part npcs primarily, or anything that really needs to be immune to any slows. Doesn't include bosses (except EOW) because the only things that use this don't go on them either.").RegisterBoolSet(
        [
        NPCID.SolarCrawltipedeBody,NPCID.SolarCrawltipedeHead,NPCID.SolarCrawltipedeTail,
        NPCID.DevourerHead,NPCID.DevourerBody,NPCID.DevourerTail,
        NPCID.DiggerHead,NPCID.DiggerBody,NPCID.DiggerTail,
        NPCID.DuneSplicerHead,NPCID.DuneSplicerBody,NPCID.DuneSplicerTail,
        NPCID.EaterofWorldsHead,NPCID.EaterofWorldsBody,NPCID.EaterofWorldsTail,
        NPCID.GiantWormHead,NPCID.GiantWormBody,NPCID.GiantWormTail,
        NPCID.LeechHead,NPCID.LeechBody,NPCID.LeechTail,
        NPCID.StardustWormHead,NPCID.StardustWormBody,NPCID.StardustWormTail,
        NPCID.TombCrawlerHead,NPCID.TombCrawlerBody,NPCID.TombCrawlerTail,
        NPCID.SeekerHead,NPCID.SeekerBody,NPCID.SeekerTail,
        NPCID.BoneSerpentHead,NPCID.BoneSerpentBody,NPCID.BoneSerpentTail,
        NPCID.WyvernHead,NPCID.WyvernBody,NPCID.WyvernTail,NPCID.WyvernBody2,NPCID.WyvernBody3,NPCID.WyvernLegs,
        NPCID.BloodEelHead,NPCID.BloodEelBody,NPCID.BloodEelTail,
        NPCID.CultistDragonHead,NPCID.CultistDragonTail,NPCID.CultistDragonBody1,NPCID.CultistDragonBody2,NPCID.CultistDragonBody3,NPCID.CultistDragonBody4,

        NPCID.Pumpking, NPCID.PumpkingBlade,
        NPCID.PirateShip, NPCID.PirateShipCannon
        ]);
}
