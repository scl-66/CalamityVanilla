using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityVanilla.Common.Interfaces;

/// <summary>
/// Only usable on Items and Projectiles
/// </summary>
public interface ISyncedOnHitEffect
{
    void SyncedOnHitNPC(Player player, NPC target, int damage, float knockback, bool crit, int hitDirection);

    public static void SendPacket(bool item, int damageDealer, Player player, NPC target, int damage, float knockback, bool crit, int hitDirection)
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;
        ModPacket packet = ModContent.GetInstance<CalamityVanilla>().GetPacket();
        packet.Write((byte)CalamityVanilla.PacketType.SyncedOnHitNPC);
        packet.WriteFlags(crit, hitDirection == 1, item);
        packet.Write(damage);
        packet.Write(knockback);
        packet.Write((short)damageDealer);
        packet.Write((byte)player.whoAmI);
        packet.Write((short)target.whoAmI);
        packet.Send(ignoreClient: player.whoAmI);
    }
    public static void HandlePacket(BinaryReader reader, int fromWho)
    {
        reader.ReadFlags(out bool crit, out bool hitDir, out bool item);
        int damage = reader.ReadInt32();
        float knockback = reader.ReadSingle();
        short damagedealer = reader.ReadInt16();
        byte player = reader.ReadByte();
        short target = reader.ReadInt16();
        if (Main.netMode == NetmodeID.Server)
            SendPacket(item, damagedealer, Main.player[player], Main.npc[target], damage, knockback, crit, hitDir ? 1 : -1);

        if (item && ContentSamples.ItemsByType[damagedealer].ModItem is ISyncedOnHitEffect i)
            i.SyncedOnHitNPC(Main.player[player], Main.npc[target], damage, knockback, crit, hitDir ? 1 : -1);

        else if (Main.projectile.FirstOrDefault(x => x.identity == damagedealer && x.owner == player).ModProjectile is ISyncedOnHitEffect i2)
            i2.SyncedOnHitNPC(Main.player[player], Main.npc[target], damage, knockback, crit, hitDir ? 1 : -1);
    }
}
public class SyncedOnHitGlobalItem : GlobalItem
{
    public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (item.ModItem is ISyncedOnHitEffect i)
        {
            i.SyncedOnHitNPC(player, target, hit.Damage, hit.Knockback, hit.Crit, hit.HitDirection);
            ISyncedOnHitEffect.SendPacket(true, item.type, player, target, hit.Damage, hit.Knockback, hit.Crit, hit.HitDirection);
        }
    }
}
public class SyncedOnHitGlobalProjectile : GlobalProjectile
{
    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (projectile.ModProjectile is ISyncedOnHitEffect i)
        {
            i.SyncedOnHitNPC(Main.player[projectile.owner], target, hit.Damage, hit.Knockback, hit.Crit, hit.HitDirection);
            ISyncedOnHitEffect.SendPacket(false, projectile.identity, Main.player[projectile.owner], target, hit.Damage, hit.Knockback, hit.Crit, hit.HitDirection);
        }
    }
}