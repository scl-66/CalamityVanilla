using CalamityVanilla.Common.Interfaces;
using CalamityVanilla.Common.Players;
using CalamityVanilla.Content.Underground.Items.GraniteTome;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla;

public partial class CalamityVanilla : Mod
{
    public enum PacketType : byte
    {
        SpawnGraniteTomeBoltSparks,
        SyncedOnHitNPC,
        SyncPlayerStats,
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        PacketType packetType = (PacketType)reader.ReadByte();

        switch (packetType)
        {
            case PacketType.SpawnGraniteTomeBoltSparks:
                Vector2 position = reader.ReadVector2();
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = GetPacket();
                    packet.Write((byte)PacketType.SpawnGraniteTomeBoltSparks);
                    packet.WriteVector2(position);
                    packet.Send(-1, whoAmI);
                    break;
                }
                GraniteTomeBolt.SpawnParticles(position);
                break;
            case PacketType.SyncedOnHitNPC:
                ISyncedOnHitEffect.HandlePacket(reader, whoAmI);
                break;
            case PacketType.SyncPlayerStats:
                byte playerNumber = reader.ReadByte();
                PlayerStats playerStatsPlayer = Main.player[playerNumber].GetModPlayer<PlayerStats>();
                playerStatsPlayer.HandlePacket(reader);
                if (Main.netMode == NetmodeID.Server)
                {
                    playerStatsPlayer.SyncPlayer(-1, whoAmI, false);
                }
                break;
            default:
                break;
        }
    }
}