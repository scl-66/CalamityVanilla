using CalamityVanilla.Content.Bosses.GutOfCthulhu.Drops.ParasiticBelt;
using CalamityVanilla.Content.Bosses.HiveMind.Drops.FilthyGrip;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Players;

public class FilthyGripCrit : ModPlayer
{
    public bool filthyGripEquipped = false;
    public override void ResetEffects()
    {
        filthyGripEquipped = false;
    }
    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hit.Crit && filthyGripEquipped)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float randWidth = Main.rand.NextFloat(-target.width, target.width) / 8;
                float randHeight = Main.rand.NextFloat(-target.height, target.height) / 8;
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Hitbox.ClosestPointInRect(Player.Center), Vector2.Zero, ModContent.ProjectileType<FilthyGripSlash>(), 30, 0f, Main.myPlayer);
            }
        }
    }
    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hit.Crit && filthyGripEquipped)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float randWidth = Main.rand.NextFloat(-target.width, target.width) / 8;
                float randHeight = Main.rand.NextFloat(-target.height, target.height) / 8;
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Hitbox.ClosestPointInRect(proj.Center), Vector2.Zero, ModContent.ProjectileType<FilthyGripSlash>(), 30, 0f, Main.myPlayer);
            }
        }
    }
}