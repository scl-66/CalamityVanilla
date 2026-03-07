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
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hit.Crit && filthyGripEquipped)
        {
            float randWidth = Main.rand.NextFloat(-target.width, target.width) / 8;
            float randHeight = Main.rand.NextFloat(-target.height, target.height) / 8;
            Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center + new Vector2(randWidth, randHeight), Vector2.Zero, ModContent.ProjectileType<FilthyGripSlash>(), 30, 0f, Main.myPlayer);
        }
    }
}