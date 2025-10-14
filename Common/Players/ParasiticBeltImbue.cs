using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Players;

public class ParasiticBeltImbue : ModPlayer
{
    public bool beltIchorImbue = false;
    public bool beltVisualHasSpawned = false;

    public override void ResetEffects()
    {
        beltIchorImbue = false;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (beltIchorImbue)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Ichor);
                d.scale = 2f * (Math.Abs(d.position.Y - Player.Center.Y) / Player.height);
                d.noGravity = Main.rand.NextBool();
                //d.velocity.Y -= 2f;
                //d.velocity.Y *= 1.1f;
                d.velocity.X = 0;
            }
        }
    }

    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (beltIchorImbue && item.CountsAsClass<MeleeDamageClass>())
        {
            target.AddBuff(BuffID.Ichor, (int)(60 * Main.rand.NextFloat(1, 2.5f)));
        }
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (beltIchorImbue && (proj.DamageType.CountsAsClass<MeleeDamageClass>() || (proj.DamageType.CountsAsClass<RangedDamageClass>() || proj.DamageType.CountsAsClass<MagicDamageClass>() || ProjectileID.Sets.IsAWhip[proj.type]) && !proj.noEnchantments))
        {
            target.AddBuff(BuffID.Ichor, (int)(60 * Main.rand.NextFloat(1, 2.5f)));
        }
    }


    public override void MeleeEffects(Item item, Rectangle hitbox)
    {
        if (beltIchorImbue && item.DamageType.CountsAsClass<MeleeDamageClass>() && !item.noMelee && !item.noUseGraphic)
        {
            Lighting.AddLight(new Vector2(hitbox.X, hitbox.Y), new Vector3(1f, 1f, 0.5f) * 1.5f); // R G B values from 0 to 1f.
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Ichor, Scale: 1f);
                dust.velocity *= 0.4f;
                dust.velocity.X += 2f * Player.direction;
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(1))
            {
                Dust dust = Dust.NewDustDirect(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Ichor, Scale: 0.5f);
                dust.velocity *= 0.4f;
                dust.velocity.X += 2f * Player.direction;
                dust.noGravity = true;
            }
        }
    }

    public override void EmitEnchantmentVisualsAt(Projectile projectile, Vector2 boxPosition, int boxWidth, int boxHeight)
    {
        if (projectile.friendly == true && projectile.owner == Main.myPlayer && beltIchorImbue && (projectile.DamageType.CountsAsClass<MeleeDamageClass>() || (projectile.DamageType.CountsAsClass<RangedDamageClass>() || projectile.DamageType.CountsAsClass<MagicDamageClass>() || ProjectileID.Sets.IsAWhip[projectile.type]) && !projectile.noEnchantments))
        {
            if (Main.rand.NextBool(1))
            {
                Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.Ichor, Scale: 0.8f);
                dust.velocity *= 0.4f;
                dust.velocity += projectile.velocity.SafeNormalize(Vector2.UnitX) * 2f;
                dust.noGravity = true;
            }
        }
    }
}