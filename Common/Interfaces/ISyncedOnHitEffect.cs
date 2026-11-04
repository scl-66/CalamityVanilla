using Microsoft.Xna.Framework;
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
    bool SyncedOnHitNPC(Player player, NPC target, Rectangle attackHitbox, int damage, float knockback, bool crit, int hitDirection, Projectile? projectile);

    public static void SendPacket(bool item, int damageDealer, Player player, NPC target, Rectangle targetHitbox, int damage, float knockback, bool crit, int hitDirection)
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
        packet.Write(targetHitbox.X);
        packet.Write(targetHitbox.Y);
        packet.Write(targetHitbox.Width);
        packet.Write(targetHitbox.Height);
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
        Rectangle hitbox = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

        if (Main.netMode == NetmodeID.Server)
            SendPacket(item, damagedealer, Main.player[player], Main.npc[target], hitbox, damage, knockback, crit, hitDir ? 1 : -1);

        if (item)
        {
            Item damageItem = ContentSamples.ItemsByType[damagedealer];
            if (damageItem.ModItem is ISyncedOnHitEffect i)
                i.SyncedOnHitNPC(Main.player[player], Main.npc[target], hitbox, damage, knockback, crit, hitDir ? 1 : -1, null);
            foreach (GlobalItem gi in damageItem.Globals)
            {
                if (gi is ISyncedOnHitEffect i2)
                    i2.SyncedOnHitNPC(Main.player[player], Main.npc[target], hitbox, damage, knockback, crit, hitDir ? 1 : -1, null);
            }
        }
        else
        {
            Projectile damageProj = Main.projectile.FirstOrDefault(x => x.identity == damagedealer && x.owner == player);
            if (damageProj != null)
            {
                if (damageProj.ModProjectile is ISyncedOnHitEffect i)
                    i.SyncedOnHitNPC(Main.player[player], Main.npc[target], hitbox, damage, knockback, crit, hitDir ? 1 : -1, damageProj);
                foreach (GlobalProjectile gp in damageProj.Globals)
                {
                    if (gp is ISyncedOnHitEffect i2)
                        i2.SyncedOnHitNPC(Main.player[player], Main.npc[target], hitbox, damage, knockback, crit, hitDir ? 1 : -1, damageProj);
                }
            }
        }
    }
}
public class SyncedOnHitGlobalItem : GlobalItem
{
    public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        bool sync = false;
        player.ItemCheck_GetMeleeHitbox(item, Item.GetDrawHitbox(item.type, player), out var dontAttack, out var itemRectangle);
        if (item.ModItem is ISyncedOnHitEffect i)
        {
            if (i.SyncedOnHitNPC(player, target, itemRectangle, hit.Damage, hit.Knockback, hit.Crit, hit.HitDirection, null))
                sync = true;
        }
        foreach (GlobalItem gi in item.Globals)
        {
            if (gi is ISyncedOnHitEffect i2)
                if (i2.SyncedOnHitNPC(player, target, itemRectangle, hit.Damage, hit.Knockback, hit.Crit, hit.HitDirection, null))
                    sync = true;
        }
        if (sync)
        {
            ISyncedOnHitEffect.SendPacket(true, item.type, player, target, itemRectangle, hit.Damage, hit.Knockback, hit.Crit, hit.HitDirection);
        }
    }
}
public class SyncedOnHitGlobalProjectile : GlobalProjectile
{
    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        bool sync = false;
        if (projectile.ModProjectile is ISyncedOnHitEffect i)
        {
            if (i.SyncedOnHitNPC(Main.player[projectile.owner], target, projectile.Hitbox, hit.Damage, hit.Knockback, hit.Crit, hit.HitDirection, projectile))
                sync = true;
        }
        foreach (GlobalProjectile gp in projectile.Globals)
        {
            if (gp is ISyncedOnHitEffect i2)
                if (i2.SyncedOnHitNPC(Main.player[projectile.owner], target, projectile.Hitbox, hit.Damage, hit.Knockback, hit.Crit, hit.HitDirection, projectile))
                    sync = true;
        }
        if (sync)
        {
            ISyncedOnHitEffect.SendPacket(false, projectile.identity, Main.player[projectile.owner], target, projectile.Hitbox, hit.Damage, hit.Knockback, hit.Crit, hit.HitDirection);
        }
    }
}