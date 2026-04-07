using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Players;

public class PlayerStats : ModPlayer
{
    public int TimeInWorld;

    public override void PreUpdate()
    {
        TimeInWorld++;
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)CalamityVanilla.PacketType.SyncPlayerStats);
        packet.Write((byte)Player.whoAmI);
        packet.Write(TimeInWorld);
        packet.Send(toWho, fromWho);
    }

    public void HandlePacket(BinaryReader reader)
    {
        TimeInWorld = reader.ReadInt32();
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        PlayerStats clone = (PlayerStats)targetCopy;
        clone.TimeInWorld = TimeInWorld;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        PlayerStats clone = (PlayerStats)clientPlayer;

        if (TimeInWorld != clone.TimeInWorld && TimeInWorld % 600 == 0)
        {
            SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
    }
}