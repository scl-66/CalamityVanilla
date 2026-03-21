using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityVanilla.Common;

public class BossDownedSystem : ModSystem
{
    private static bool _downedCryogen;
    private static bool _downedGutOfCthulhu;
    private static bool _downedHiveMind;
    private static bool[] downedBoss = [];
    public static bool[] DownedBoss { get => downedBoss; set => downedBoss = value; }
    public override void Unload() => DownedBoss = null;

    public static bool DownedCryogen
    {
        get => _downedCryogen;
        set
        {
            if (!value)
                _downedCryogen = false;
            else
                NPC.SetEventFlagCleared(ref _downedCryogen, -1);
        }
    }
    public static bool DownedGutOfCthulhu
    {
        get => _downedGutOfCthulhu;
        set
        {
            if (!value)
                _downedGutOfCthulhu = false;
            else
                NPC.SetEventFlagCleared(ref _downedGutOfCthulhu, -1);
        }
    }
    public static bool DownedHiveMind
    {
        get => _downedHiveMind;
        set
        {
            if (!value)
                _downedHiveMind = false;
            else
                NPC.SetEventFlagCleared(ref _downedHiveMind, -1);
        }
    }
    private static void ResetFlags()
    {
        DownedCryogen = false;
    }
    public override void OnWorldLoad() => ResetFlags();
    public override void OnWorldUnload() => ResetFlags();

    public override void SaveWorldData(TagCompound tag)
    {
        List<string> downed = [];
        if (DownedGutOfCthulhu)
            downed.Add("gutofcthulhu");
        if (DownedHiveMind)
            downed.Add("hivemind");
        if (DownedCryogen)
            downed.Add("cryogen");

        tag.Add("downed", downed);
    }

    public override void LoadWorldData(TagCompound tag)
    {
        IList<string> downed = tag.GetList<string>("downed");
        DownedGutOfCthulhu = downed.Contains("gutofcthulhu");
        DownedHiveMind = downed.Contains("hivemind");
        DownedCryogen = downed.Contains("cryogen");
    }

    public override void NetReceive(BinaryReader reader)
    {
        BitsByte flags = reader.ReadByte();
        DownedGutOfCthulhu = flags[0];
        DownedHiveMind = flags[1];
        DownedCryogen = flags[2];

    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(new BitsByte()
        {
            [0] = DownedGutOfCthulhu,
            [1] = DownedHiveMind,
            [2] = DownedCryogen,
        });
    }
}