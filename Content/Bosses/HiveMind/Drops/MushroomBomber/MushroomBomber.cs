using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityVanilla.Content.Bosses.HiveMind.Drops.MushroomBomber;

public class MushroomBomber : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
    }
    public override void SetDefaults()
    {
        Item.channel = true;
        Item.DamageType = DamageClass.Ranged;
        Item.damage = 900;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useTime = Item.useAnimation = 60 * 5;
        Item.knockBack = 10;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 2, 10, 0);
        Item.width = 32;
        Item.height = 16;
        Item.shoot = ModContent.ProjectileType<MushroomBomberHeld>();
        Item.shootSpeed = 5;
        Item.noUseGraphic = true;
        Item.noMelee = true;
    }
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FirstOrDefault(tooltip => tooltip.Name == "Speed" && tooltip.Mod == "Terraria").Text = this.GetLocalization("SpecialTooltip", () => "").Value;
    }
}

public class MushroomBomberHeld : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 4;
    }
    public override bool? CanDamage() => false;

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.ignoreWater = true;
        Projectile.frame = 1;
    }
    private const int _mediumTime = 60 * 2;
    private const int _bigTime = 60 * 5;
    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        owner.heldProj = Projectile.whoAmI;

        if (owner.channel && !owner.noItems && !owner.CCed)
        {
            Projectile.timeLeft = 20;
            if (owner == Main.LocalPlayer)
            {
                Projectile.velocity = owner.Center.DirectionTo(Main.MouseWorld) * 8;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] == _mediumTime || Projectile.ai[0] == _bigTime)
            {
                if (Projectile.ai[0] == _mediumTime) {
                    SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);
                } else if (Projectile.ai[0] == _bigTime) {
                    SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite with { Pitch = 0.7f, Volume = 1.1f }, Projectile.position);
                }
                Projectile.frame++;
                Vector2 mouthPos = Projectile.Center + new Vector2(Projectile.spriteDirection * 13, -8).RotatedBy(Projectile.rotation);
                for (int i = 0; i < 15; i++)
                {
                    Dust d = Dust.NewDustPerfect(mouthPos + Main.rand.NextVector2Circular(10, 10), DustID.Corruption, Main.rand.NextVector2Circular(3, 3));
                    d.noGravity = true;
                }
            }
        }
        else if (Projectile.ai[2] == 0)
        {
            Vector2 mouthPos = Projectile.Center + new Vector2(Projectile.spriteDirection * 2, -10).RotatedBy(Projectile.rotation);
            Projectile.ai[2]++;
            float Power = Projectile.frame == 1? Utils.Remap(Projectile.ai[0], 0, _mediumTime, 0, 1, true) : Utils.Remap(Projectile.ai[0], _mediumTime, _bigTime, 0, 1, true);
            if (owner == Main.LocalPlayer)
            {
                StatModifier damageModifier = owner.GetTotalDamage(Projectile.DamageType);
                damageModifier = damageModifier.CombineWith(owner.specialistDamage);
                damageModifier = damageModifier.CombineWith(new StatModifier((float)owner.HeldItem.damage / owner.HeldItem.OriginalDamage, 1));
                CombinedHooks.ModifyWeaponDamage(owner, owner.HeldItem, ref damageModifier);
                StatModifier knockbackModifier = owner.GetTotalKnockback(Projectile.DamageType);
                knockbackModifier.CombineWith(new StatModifier(owner.HeldItem.knockBack / ContentSamples.ItemsByType[owner.HeldItem.type].knockBack,1));
                CombinedHooks.ModifyWeaponKnockback(owner, owner.HeldItem, ref knockbackModifier);
                switch (Projectile.frame)
                {
                    case 1:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                            mouthPos,
                            Projectile.velocity * Utils.Remap(Power, 0, 1, 0.7f, 1.5f),
                            ModContent.ProjectileType<MushroomBomberShotSmall>(),
                            (int)damageModifier.ApplyTo(Utils.Remap(Power, 0, 1, 140, 650)),
                            knockbackModifier.ApplyTo(Utils.Remap(Power, 0, 1, 3, 5)),
                            owner.whoAmI);
                        break;
                    case 2:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                            mouthPos,
                            Projectile.velocity * Utils.Remap(Power, 0, 1, 1.5f, 1.8f),
                            ModContent.ProjectileType<MushroomBomberShotMedium>(),
                            (int)damageModifier.ApplyTo(Utils.Remap(Power, 0, 1, 650, 1250)),
                            knockbackModifier.ApplyTo(Utils.Remap(Power, 0, 1, 5, 8)),
                            owner.whoAmI);
                        break;
                    case 3:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                            mouthPos,
                            Projectile.velocity * 1.8f,
                            ModContent.ProjectileType<MushroomBomberShotLarge>(),
                            (int)damageModifier.ApplyTo(1500),
                            knockbackModifier.ApplyTo(10),
                            owner.whoAmI);
                        break;
                }
            }
            for(int i = 0; i < 5; i++)
            {
                Dust d = Dust.NewDustPerfect(mouthPos, DustID.Corruption, Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.3f, 1f));
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item61, Projectile.position);
            Projectile.frame = 0;
        }
        Projectile.spriteDirection = Projectile.direction;
        owner.ChangeDir(Projectile.direction);
        owner.itemRotation = MathHelper.WrapAngle((float)Math.Atan2(Projectile.velocity.Y * (float)Projectile.direction, Projectile.velocity.X * (float)Projectile.direction));
        owner.SetDummyItemTime(Projectile.timeLeft);

        float Recoil = Utils.Remap(Projectile.timeLeft, 10, 20, MathHelper.Pi, 0, true);
        Projectile.Center = owner.RotatedRelativePoint(owner.MountedCenter + (Projectile.velocity * (1f - (MathF.Sin(Recoil) * 1.2f))), false, true);

        Projectile.rotation = owner.itemRotation;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpriteEffects effect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        //int xFrame = ((Projectile.frame == 1 && Projectile.ai[0] > _mediumTime - 60) || (Projectile.frame == 2 && Projectile.ai[0] > _bigTime - 60)) ? 1 : 0;
        Rectangle frame = TextureAssets.Projectile[Type].Frame(2, 4, 0, Projectile.frame);
        int xOrigin = Projectile.spriteDirection == 1 ? 38 : frame.Width - 38;
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value,Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, new Vector2(xOrigin, 28), Projectile.scale, effect);
        float glowRotation = (float)(Main.timeForVisualEffects * 0.04f);
        if (Projectile.frame == 3)
        {
            frame = TextureAssets.Projectile[Type].Frame(2, 4, 1, Projectile.frame);
            float percent = Utils.Remap(Projectile.ai[0], _bigTime, _bigTime + 30, 0, 1);
            Color glow = Color.Lerp(new Color(1f, 0.5f, 0.5f, 0f), new Color(0.5f, 0.25f, 1f, 0f), Main.masterColor) * percent;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, frame, glow * 0.5f, Projectile.rotation, new Vector2(xOrigin, 28), Projectile.scale, effect);
            for (int i = 0; i < 4; i++)
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition + new Vector2(MathF.Sin((float)Main.timeForVisualEffects * 0.03f) * 4, 0).RotatedBy(Projectile.rotation + i * MathHelper.PiOver2 + glowRotation), frame, glow * 0.15f, Projectile.rotation, new Vector2(xOrigin, 28), Projectile.scale, effect);
        }
        else
        {
            frame.Width -= 46;
            frame.X += 46;
            xOrigin = Projectile.spriteDirection == 1 ? 38 - 46 : frame.Width - 38 + 46;
            float percent = Projectile.frame == 2 ? Utils.Remap(Projectile.ai[0], _mediumTime, _bigTime, 0, 1) : Utils.Remap(Projectile.ai[0], 0, _mediumTime, 0, 1);
            float antiPercent = 1f - percent;
            Color c = lightColor with { A = 0 } * MathF.Pow(percent, 3) * antiPercent * 3;
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition + new Vector2(0, 2 + (antiPercent * 4 * Projectile.frame)).RotatedBy(i * MathHelper.PiOver2 + glowRotation), frame, c, Projectile.rotation, new Vector2(xOrigin, 28), Projectile.scale, effect);
            }
        }
        return false;
    }
}