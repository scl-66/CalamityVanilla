using CalamityVanilla.Common.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Crimson.GutOfCthulhu.Drops.ParasiticBelt;

[AutoloadEquip(EquipType.Waist)]
public class ParasiticBelt : ModItem
{
    //public bool beltVisualHasSpawned = false;

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 26;
        Item.maxStack = 1;
        Item.value = Item.sellPrice(0, 5);
        Item.accessory = true;
        Item.rare = ItemRarityID.Pink;
        Item.expert = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.statLife < player.statLifeMax2 / 2)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.GetModPlayer<ParasiticBeltImbue>().beltIchorImbue = true;
            player.MeleeEnchantActive = true; // MeleeEnchantActive indicates to other mods that a weapon imbue is active.
            player.AddBuff(ModContent.BuffType<ParasiticBeltBuff>(), 5);

            if (player.GetModPlayer<ParasiticBeltImbue>().beltVisualHasSpawned == false) // spawn brain of confusion-styled visual indicator, dust and noise to indicate effect
            {
                SoundEngine.PlaySound(SoundID.Item74 with { Pitch = 0.5f, PitchVariance = 0.2f, Volume = 0.5f });
                SoundEngine.PlaySound(Main.rand.NextBool() == true ? SoundID.NPCDeath19 : SoundID.NPCDeath12);

                Projectile.NewProjectile(player.GetSource_Accessory(Item),
                    player.Center - new Vector2(Main.rand.NextFloat(-25f, 25f), player.height * 1.5f),
                    Vector2.Zero,
                    ModContent.ProjectileType<ParasiticBeltEffect>(),
                    0,
                    0f);
                player.GetModPlayer<ParasiticBeltImbue>().beltVisualHasSpawned = true;

                for (int i = 0; i < 30; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f);
                    Dust d = Dust.NewDustDirect(player.position, 30, 30, DustID.Ichor, speed.X + player.velocity.X, speed.Y + player.velocity.Y);
                    d.noGravity = false;
                    d.velocity *= 0.9f;
                    d.scale = 1.2f * Main.rand.NextFloat(0.5f, 1f);
                    //d.velocity.Y *= 1.1f;
                }
            }
        }
        else
        {
            player.GetModPlayer<ParasiticBeltImbue>().beltVisualHasSpawned = false;
        }
    }
}

public class ParasiticBeltEffect : ModProjectile
{
    public ref float Siner => ref Projectile.ai[1];
    public override void SetDefaults()
    {
        Projectile.arrow = true;
        Projectile.width = 22;
        Projectile.height = 22;
        Projectile.aiStyle = ProjAIStyleID.BrainofConfusion;
    }
    public override void AI()
    {
        Projectile.ai[1]++;
        Projectile.position.X += (float)Math.Cos(Siner / 5f) * 2f / (Siner / 10f) - 2f / (Siner / 5f);
        Projectile.rotation = (float)Math.Sin(Siner / 5f) * (1f - Siner / 60f);
    }
}

public class ParasiticBeltBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }
}
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