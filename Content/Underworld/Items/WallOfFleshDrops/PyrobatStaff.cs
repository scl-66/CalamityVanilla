using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underworld.Items.WallOfFleshDrops;

// ExampleStaff is a typical staff. Staffs and other shooting weapons are very similar, this example serves mainly to show what makes staffs unique from other items.
// Staff sprites, by convention, are angled to point up and to the right. "Item.staff[Type] = true;" is essential for correctly drawing staffs.
// Staffs use mana and shoot a specific projectile instead of using ammo. Item.DefaultToStaff takes care of that.
public class PyrobatStaff : ModItem
{
    public override string Texture => Assets.Textures.Underworld.Items.WallOfFleshDrops.PyrobatStaff.PyrobatStaffItem.KEY;

    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true; // This makes the useStyle animate as a staff instead of as a gun.
    }

    public override void SetDefaults()
    {
        // DefaultToStaff handles setting various Item values that magic staff weapons use.
        // Hover over DefaultToStaff in Visual Studio to read the documentation!
        //Item.DefaultToStaff(ModContent.ProjectileType<Pyrobat>(), 16, 50, 25);
        Item.mana = 25;
        Item.width = 48;
        Item.height = 48;
        Item.useTime = 25;
        Item.useAnimation = 25;
        Item.SetWeaponValues(85, 8);
        Item.SetShopValues(ItemRarityColor.LightRed4, 60000);
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Magic;
        Item.shoot = ModContent.ProjectileType<PyrobatStaffHeldProjectile>();
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.channel = true;
    }
    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] == 0;
    }
}

public class PyrobatStaffHeldProjectile : ModProjectile
{
    public override string Texture => Assets.Textures.Underworld.Items.WallOfFleshDrops.PyrobatStaff.PyrobatStaffHeldProjectile.KEY;

    public static Asset<Texture2D> TextureGlow => Assets.Textures.Underworld.Items.WallOfFleshDrops.PyrobatStaff.PyrobatStaffHeldProjectile_Glow.Asset;

    public ref float ShootTimer => ref Projectile.ai[0];
    public int ShootCount = 0;

    SlotId hissSound;
    const int FULL_CHARGE_TIME = 60;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 2;
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.timeLeft = 60;
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.hide = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.ignoreWater = true;
    }
    public override bool? CanDamage() => false;

    public override void AI()
    {
        SoundStyle hissSoundStyle = new SoundStyle("Terraria/Sounds/Custom/sizzle") with { IsLooped = true };
        Player player = Main.player[Projectile.owner];
        player.heldProj = Projectile.whoAmI;
        if (player.channel || Projectile.ai[0] < 10)
        {

            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Square(-8, 8), DustID.Smoke);
                d.noGravity = true;
                d.velocity.Y -= 2;
                d.velocity.X *= 0.2f;
                d.scale = Main.rand.NextFloat(0.5f, 1f);
                d.alpha = 128;
            }

            if (!hissSound.IsValid || !hissSound.IsActive)
            {
                hissSound = SoundEngine.PlaySound(hissSoundStyle, Projectile.Center);
            }
            if (SoundEngine.TryGetActiveSound(hissSound, out var result))
            {
                result.Position = player.Center;
                result.Pitch = Utils.Remap(Projectile.ai[0], 0, FULL_CHARGE_TIME, -2, 0);
                result.Volume = Utils.Remap(Projectile.ai[0], 0, FULL_CHARGE_TIME, 0.5f, 1);
            }
            Projectile.ai[0]++;

            if (Projectile.ai[0] == FULL_CHARGE_TIME)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch);
                    d.noGravity = true;
                    d.scale = 2;
                    d.velocity *= 3;
                }
                SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, player.position);
            }

            Projectile.timeLeft = 25;
            player.SetDummyItemTime(Projectile.timeLeft);
            Projectile.spriteDirection = Math.Sign(Main.MouseWorld.X - player.Top.X);
        }
        else
        {
            Projectile.frame = 1;
            Projectile.ai[1]++;
            Vector2 shootDirection = player.Center.DirectionTo(Main.MouseWorld);
            if (Projectile.ai[1] == 1)
            {
                if (SoundEngine.TryGetActiveSound(hissSound, out var result))
                {
                    result.Stop();
                }
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, player.position);
                if (Projectile.ai[0] >= FULL_CHARGE_TIME)
                {
                    //shootDirection = Vector2.UnitX.RotatedBy(Utils.AngleLerp(shootDirection.ToRotation(),player.direction == 1? 0 : MathHelper.Pi, 0.65f));
                    //Vector2 shootDirection = Utils.rotateTowards(player.Center,new Vector2(player.direction,0),Main.MouseWorld,MathHelper.TwoPi);
                    for (int i = 0; i < 15; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch);
                        d.noGravity = true;
                        d.scale = 2;
                        d.velocity *= 2;
                        d.velocity += shootDirection.RotatedByRandom(1) * Main.rand.NextFloat(15);
                    }
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(player.direction * 5, shootDirection.Y * 5), ModContent.ProjectileType<Pyrobat>(), Projectile.damage, Projectile.knockBack);
                }
            }
            float chargePercent = Projectile.ai[0] / FULL_CHARGE_TIME;
            if (Projectile.ai[0] < FULL_CHARGE_TIME && (Projectile.ai[1] == 1 || (Projectile.ai[1] == 6 && chargePercent > 0.33f) || (Projectile.ai[1] == 13 && chargePercent > 0.66f)))
            {
                for (int i = 0; i < 5; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch);
                    d.noGravity = true;
                    d.scale = 2;
                    d.velocity *= 2;
                    d.velocity += shootDirection * Main.rand.NextFloat(3);
                }
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, player.position);
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootDirection.RotatedByRandom(0.2f) * Main.rand.NextFloat(6, 8), ModContent.ProjectileType<PyrobatSmall>(), Projectile.damage / 5, Projectile.knockBack);
            }
        }
        player.direction = Projectile.spriteDirection;
        Projectile.Center = player.RotatedRelativePoint(player.MountedCenter + new Vector2(25 * player.direction, -16)).Floor();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Vector2 origin = new Vector2(33, 15);
        if (Projectile.spriteDirection == -1)
        {
            origin.X -= 18;
        }
        Rectangle frame = tex.Frame(1, 2, 0, Projectile.frame);
        SpriteEffects effect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, effect);
        if (Projectile.frame == 1)
            return false;

        Main.EntitySpriteDraw(TextureGlow.Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, effect);
        float percent = Utils.Remap(Projectile.ai[0], 0, FULL_CHARGE_TIME, 0, 1);

        Main.EntitySpriteDraw(TextureGlow.Value, Projectile.Center - Main.screenPosition, frame, Color.White with { A = 0 } * percent, Projectile.rotation, origin, Projectile.scale + MathF.Pow(1f - percent, 2) * 2, effect);

        if (Projectile.ai[0] < FULL_CHARGE_TIME)
            return false;
        float sin = Utils.Remap((float)Math.Sin(Main.timeForVisualEffects * 0.1f), -1, 1, 0.25f, 0.5f);
        ulong seed4 = Main.TileFrameSeed;
        for (int i = 0; i < 4; i++)
        {
            Vector2 rand = new Vector2(Utils.RandomInt(ref seed4, -2, 3), Utils.RandomInt(ref seed4, -2, 3));
            Main.EntitySpriteDraw(TextureGlow.Value, Projectile.Center - Main.screenPosition + new Vector2(0, 2).RotatedBy(i * MathHelper.PiOver2) + rand, frame, Color.White with { A = 0 } * percent * sin, Projectile.rotation, origin, Projectile.scale, effect);
        }
        return false;
    }
}
public class PyrobatSmall : ModProjectile
{
    public override string Texture => Assets.Textures.Underworld.Items.WallOfFleshDrops.PyrobatStaff.PyrobatSmall.KEY;

    public static Asset<Texture2D> TextureGlow => Assets.Textures.Underworld.Items.WallOfFleshDrops.PyrobatStaff.PyrobatSmall_Glow.Asset;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 5;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 16);
    }
    public override void AI()
    {
        Projectile.spriteDirection = Projectile.direction;
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.spriteDirection == -1)
            Projectile.rotation += MathHelper.Pi;
        Projectile.frameCounter++;
        if (Projectile.frameCounter > 3)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame > 4)
                Projectile.frame = 0;
        }

        if (Main.rand.NextBool(4))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            d.noGravity = true;
            d.velocity += Projectile.velocity;
            d.scale = Main.rand.NextFloat(1f, 1.5f);
        }

        if (Main.rand.NextBool(2))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            d.noGravity = true;
            d.velocity *= 0.9f;
            d.scale = Main.rand.NextFloat(0.5f, 1f);
            d.alpha = 128;
        }
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            d.noGravity = true;
            d.velocity += Projectile.velocity * Main.rand.NextFloat(0.75f);
            d.scale = Main.rand.NextFloat(1.5f, 2);
        }
        for (int i = 0; i < 5; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MeteorHead);
            d.noGravity = Main.rand.NextBool();
            d.velocity += Projectile.velocity * Main.rand.NextFloat(0.75f);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Rectangle frame = tex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
        SpriteEffects effect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2, Projectile.scale, effect);
        Main.EntitySpriteDraw(TextureGlow.Value, Projectile.Center - Main.screenPosition, frame, Color.White with { A = 128 }, Projectile.rotation, frame.Size() / 2, Projectile.scale, effect);
        ulong seed4 = Main.TileFrameSeed;
        for (int i = 0; i < 5; i++)
        {
            Vector2 rand = new Vector2(Utils.RandomInt(ref seed4, -2 + (i * -2), 3 + (i * 2)), Utils.RandomInt(ref seed4, -2 + (i * -2), 3 + (i * 2)));
            //Main.EntitySpriteDraw(TextureGlow.Value, Projectile.Center - Main.screenPosition - Projectile.velocity * i, frame, Color.White with { A = 0 } * ((1f - (i / 5f)) * 0.5f), Projectile.rotation, frame.Size() / 2, Projectile.scale, effect);
            Main.EntitySpriteDraw(TextureGlow.Value, Projectile.Center - Main.screenPosition - Projectile.velocity * i + rand, frame, Color.Lerp(Color.White, Color.Red, i / 5f) with { A = 64 } * (1f - (i / 5f)), Projectile.rotation, frame.Size() / 2, Projectile.scale, effect);
        }

        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(2))
            target.AddBuff(BuffID.OnFire3, 60 * 2);
        else if (Main.rand.NextBool(4))
        {
            target.AddBuff(BuffID.OnFire3, 60 * 6);
        }
    }
}
public class Pyrobat : ModProjectile
{
    public override string Texture => Assets.Textures.Underworld.Items.WallOfFleshDrops.PyrobatStaff.Pyrobat.KEY;

    public virtual Asset<Texture2D> TextureGlow => Assets.Textures.Underworld.Items.WallOfFleshDrops.PyrobatStaff.Pyrobat_Glow.Asset;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 6;
        ProjectileID.Sets.TrailCacheLength[Type] = 5;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }
    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60 * 4;
        DrawOffsetX = -25;
        DrawOriginOffsetY = -20;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 60 * 6);
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
        SoundEngine.PlaySound(SoundID.Item110, Projectile.position);
        for (int i = 0; i < 20; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, 20, 20, DustID.Torch);
            d.velocity *= Main.rand.NextFloat(2.5f, 7.5f);
            d.scale = Main.rand.NextFloat(1f, 2f);
            d.noGravity = !Main.rand.NextBool(5);
        }
        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, 20, 20, DustID.Smoke);
            d.velocity *= Main.rand.NextFloat(1.5f, 3.5f);
            d.scale = Main.rand.NextFloat(1f, 2f);
            d.noGravity = true;
            d.alpha = 128;
        }
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height / 2, DustID.MeteorHead);
            d.noGravity = Main.rand.NextBool();
            d.velocity *= Main.rand.NextFloat(1, 2.5f);
            d.velocity.Y -= 2;
        }
    }
    public override void AI()
    {
        //Lighting.AddLight(Projectile.Center, 1f, 0.55f, 0.25f);

        if (Projectile.timeLeft > 14)
        {
            Projectile.ai[0]--;
            float gravity = 0.3f;
            int timeBetweenBounces = 24;
            Projectile.velocity.Y += gravity;

            if (Projectile.velocity.Y < gravity * -timeBetweenBounces / 2)
            {
                Projectile.velocity.Y += 0.2f;
            }
            else if (Projectile.velocity.Y > gravity * timeBetweenBounces / 2)
            {
                Projectile.velocity.Y -= 0.2f;
            }

            if (Projectile.ai[0] == -timeBetweenBounces / 2)
            {
                Projectile.ai[0] = timeBetweenBounces / 2;
                Projectile.velocity.Y += gravity * -timeBetweenBounces;
            }
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % (Main.projFrames[Projectile.type] - 2);
            }
        }
        else if (Projectile.timeLeft >= 7)
        {
            Projectile.frame = 4;
            Projectile.velocity.X *= 0.9f;
            Projectile.velocity.Y = 0;
            //Projectile.velocity.Y = 0f;
        }
        else if (Projectile.timeLeft < 7)
        {
            Projectile.frame = 5;
        }

        if (Projectile.ai[1]-- <= 0)
        {
            SoundEngine.PlaySound(SoundID.LiquidsWaterLava with { Volume = 0.75f }, Projectile.position);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity * new Vector2(Main.rand.NextFloat(-0.7f, 0.2f), 0.2f), ModContent.ProjectileType<PyrobatFlame>(), Projectile.damage / 2, Projectile.knockBack);
            Projectile.ai[1] = Main.rand.Next(15, 40);
        }

        if (Main.rand.NextBool(4))
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, 8) + Main.rand.NextVector2Circular(16, 16), DustID.Torch);
            d.noGravity = true;
            d.scale = Main.rand.NextFloat(1f, 1.5f);
            d.velocity.X += Projectile.velocity.X;
        }

        if (Main.rand.NextBool(2))
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, 8) + Main.rand.NextVector2Circular(16, 16), DustID.Smoke);
            d.noGravity = true;
            d.velocity *= 0.9f;
            d.scale = Main.rand.NextFloat(0.5f, 1f);
            d.alpha = 128;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = -oldVelocity.X;
        //if (Projectile.velocity.Y != oldVelocity.Y)
        //Projectile.velocity.Y = -oldVelocity.Y * 0.2f;
        return false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Rectangle frame = tex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
        SpriteEffects effect = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2, Projectile.scale, effect);
        Main.EntitySpriteDraw(TextureGlow.Value, Projectile.Center - Main.screenPosition, frame, Color.White with { A = 128 }, Projectile.rotation, frame.Size() / 2, Projectile.scale, effect);
        ulong seed4 = Main.TileFrameSeed;
        for (int i = 0; i < 5; i++)
        {
            Vector2 rand = new Vector2(Utils.RandomInt(ref seed4, -2, 3), Utils.RandomInt(ref seed4, -2, 3));
            Main.EntitySpriteDraw(TextureGlow.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2 + rand, frame, Color.Lerp(Color.White, Color.Red, i / 5f) with { A = 64 } * (1f - (i / 5f)), Projectile.rotation, frame.Size() / 2, Projectile.scale, effect);
            //Main.EntitySpriteDraw(TextureGlow.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, frame, Color.White with { A = 0 } * ((1f - (i / 5f)) * 0.5f), Projectile.rotation, frame.Size() / 2, Projectile.scale, effect);
        }
        return false;
    }
}

public class Pyrobat2 : Pyrobat // functionally identical to Pyrobat, except for the sprite
{
    public override string Texture => Assets.Textures.Underworld.Items.WallOfFleshDrops.PyrobatStaff.Pyrobat2.KEY;

    public override Asset<Texture2D> TextureGlow => Assets.Textures.Underworld.Items.WallOfFleshDrops.PyrobatStaff.Pyrobat2_Glow.Asset;
}

public class PyrobatFlame : ModProjectile
{
    public override string Texture => Assets.Textures.Underworld.Items.WallOfFleshDrops.PyrobatStaff.PyrobatFlame.KEY;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 3;
    }
    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.aiStyle = -1; // or 14
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        //AIType = ProjectileID.GreekFire1;
        Projectile.timeLeft = 60 * 6;
        Projectile.penetrate = Main.rand.Next(2, 3);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 60 * 2);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity.Y = oldVelocity.Y * -0.24f;
        return false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Rectangle frame = tex.Frame(1, 3, 0, Projectile.frame);
        ulong seed4 = Main.TileFrameSeed;
        for (int i = 0; i < 4; i++)
        {
            Vector2 rand = new Vector2(Utils.RandomInt(ref seed4, -2, 3), Utils.RandomInt(ref seed4, -2, 3));
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + rand, frame, new Color(120, 120, 120, 0), Projectile.rotation, frame.Size() / 2, Projectile.scale - (float)i * 0.2f, SpriteEffects.None, 0);
        }
        return false;
    }
    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.X * -1f;
        Projectile.velocity.Y += 0.1f;
        Projectile.velocity.X *= 0.96f;
        if (++Projectile.frameCounter >= 4)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }

        Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
        d.scale = Main.rand.NextFloat(1f, 1.5f);
        d.noGravity = true;
        d.velocity.Y -= 1f;
        if (Main.rand.NextBool(3))
        {
            Dust d2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            d2.noGravity = false;
            d2.velocity.X *= 0.1f;
            d2.velocity.Y = Main.rand.NextFloat(-2, 0);
            d2.scale = Main.rand.NextFloat(0.3f, 1f);
            d2.alpha = 128;
        }

        if (Projectile.wet)
        {
            Projectile.Kill();
        }
    }
}