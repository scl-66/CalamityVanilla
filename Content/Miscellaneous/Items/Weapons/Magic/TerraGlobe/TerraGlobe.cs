using CalamityVanilla.Common.Particles;
using CalamityVanilla.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Magic.TerraGlobe;

public class TerraGlobe : ModItem
{
    public const int DEBUFF_DURATION = 60 * 5;
    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White with { A = 200 };
    }
    public override void SetDefaults()
    {
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<TerraGlobeProjectile>(), 30, 9, true);
        Item.mana = 24;
        Item.damage = 78;
        Item.knockBack = 5;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(0, 20, 0, 0);
        Item.holdStyle = ItemHoldStyleID.HoldFront;
        Item.UseSound = SoundID.Item67 with
        {
            Volume = 0.75f
        };
    }

    public override void HoldStyle(Player player, Rectangle heldItemFrame)
    {
        player.itemLocation.X -= 29f * player.direction;
        player.itemLocation.Y -= 28f;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        int rand = Main.rand.Next([-1, 1]);
        velocity = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(5, 12) * rand));
    }
    public override bool ModifyItemDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, ref DrawData? coloredDrawData, ref DrawData? glowMaskDrawData)
    {
        Texture2D tex = AssetReferences.Content.Miscellaneous.Items.Weapons.Magic.TerraGlobe.TerraGlobeHeld.Asset.Value;
        drawData.texture = tex;

        drawData.sourceRect = tex.Frame(1, 17, 0, (int)((Main.timeForVisualEffects / 3) % 17));
        drawData.origin = new Vector2(11, 12);
        drawData.position += Vector2.UnitX.RotatedBy(drawInfo.drawPlayer.itemRotation) * 30 * drawInfo.drawPlayer.direction;

        return base.ModifyItemDraw(ref drawInfo, ref drawData, ref coloredDrawData, ref glowMaskDrawData);
    }
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddTile(TileID.MythrilAnvil)
            .AddIngredient<BlacklightScepter.BlacklightScepter>()
            .AddIngredient<FrigidFlashBolt.FrigidflashBolt>()
            .AddIngredient(ItemID.VenomStaff)
            .AddIngredient(ItemID.SpectreBar, 12)
            .Register();
    }
}

public class TerraGlobeProjectile : ModProjectile
{
    public int maxTimeLeft = (int)(60 * 2.23f);

    public override void Load()
    {
        MiscShaderData shader = new MiscShaderData(Main.Assets.Request<Effect>("PixelShader"), "MagicMissile").UseProjectionMatrix(doUse: true);
        shader.UseImage2(TextureAssets.MagicPixel);
        shader.UseImage1(TextureAssets.MagicPixel);
        shader.UseImage0(TextureAssets.MagicPixel);
        GameShaders.Misc.Add("TerraGlobeOrb", shader);
    }
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
    }
    public override void SetDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        Projectile.QuickDefaults(false, 24);
        Projectile.timeLeft = maxTimeLeft;
        Projectile.penetrate = 5;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.ignoreWater = true;
        Projectile.Opacity = 0;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(ModContent.BuffType<Terraburn>(), TerraGlobe.DEBUFF_DURATION);
    }
    public override void AI()
    {
        Projectile.Opacity += 0.1f;
        Projectile.ai[1]++;
        if (Projectile.ai[1] == 1 && Main.myPlayer == Projectile.owner) // summon orbiting orbs
        {
            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                Vector2 rand = Main.rand.NextVector2CircularEdge(Projectile.width * 2, Projectile.height * 2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + rand, rand.SafeNormalize(Vector2.UnitX) * 5f,
                ModContent.ProjectileType<TerraGlobeOrb>(), 0, 0.5f, Projectile.owner, Projectile.identity, Projectile.damage / 3);
            }
        }
        Projectile.rotation += 0.1f;
        if (Projectile.ai[1] == 3) // spawn dust when first fired (this code is messy i knowww)
        {
            for (int i = 0; i < 30; i++)
            {
                float projRotation = Projectile.rotation - (Projectile.direction == 1 ? 0 : MathHelper.Pi);
                Vector2 arc = Main.rand.NextVector2Unit(MathHelper.Pi / 4, MathHelper.Pi).RotatedBy(-MathHelper.PiOver4 * 3);
                Vector2 rotatedArc = arc.RotatedBy(projRotation);
                float rand = Main.rand.NextFloat(0.5f, 1f);
                Vector2 speed = rotatedArc * 2f * rand + Main.player[Projectile.owner].velocity;
                Dust d2 = Dust.NewDustDirect(Projectile.position, 8, 8, DustID.Terra, speed.X, speed.Y);
                d2.noGravity = !Main.rand.NextBool(3);
                d2.scale = rand + 0.25f + Main.rand.NextFloat(-0.15f, 0.15f);
            }
        }

        if (Projectile.timeLeft > 40 && Projectile.penetrate > 1)
        {
            Vector2 dir = Projectile.Center.DirectionTo(Main.MouseWorld);
            Projectile.velocity += (dir);
            Projectile.velocity = Projectile.velocity.LengthClamp(15, 2);
        }
        else
        {
            Projectile.velocity *= 0.93f;
        }

        if (Projectile.penetrate <= 1 && Projectile.timeLeft > 41)
        {
            Projectile.timeLeft = 40;
        }

        if (Main.rand.NextBool(4))
        {
            Dust d2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terra);
            d2.velocity *= 0.3f;
            d2.velocity += Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f);
            d2.noGravity = !Main.rand.NextBool(8);
            d2.scale = 0.25f;
            d2.fadeIn = 0.3f;
        }
    }
    //public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    //{
    //Projectile.damage = (int)(Projectile.damage * 0.8f);
    //}

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        Projectile.damage = (int)(Projectile.damage * 0.8f);
        Projectile.penetrate--;
        if (Projectile.penetrate > 0)
        {
            // If the projectile hits the left or right side of the tile, reverse the X velocity
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity *= 0.5f;
            return false;
        }
        return true;
    }

    public override void OnKill(int timeLeft)
    {
        if (Main.myPlayer == Projectile.owner)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TerraGlobeExplosion>(), (int)(Projectile.damage * 2.5f), Projectile.knockBack);
        }
    }
    private VertexStrip _vertexStrip = new();
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        Rectangle grayFrame = new Rectangle(0, tex.Height() / 2, tex.Width(), tex.Height() / 2);
        Rectangle frame = new Rectangle(0, 0, tex.Width(), tex.Height() / 2);
        float outerScale = Utils.Remap(Projectile.timeLeft, 0, 60, 0, 3) * Projectile.Opacity;
        for (int i = 0; i < 3; i++)
        {
            float amount = 1f - (int)((Main.timeForVisualEffects - 10 * i) % 30) / 30f;
            Color c = new Color(45, 123, 204, 0) * 0.5f;
            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, grayFrame, c * Projectile.Opacity * (1f - amount), Projectile.rotation, frame.Size() / 2, 1f + (amount) * outerScale, SpriteEffects.None);
        }

        MiscShaderData miscShaderData = GameShaders.Misc["TerraGlobeOrb"];
        miscShaderData.UseOpacity(Projectile.Opacity);
        miscShaderData.UseSaturation(-1);
        miscShaderData.Apply();
        _vertexStrip.PrepareStripWithProceduralPadding(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, -Main.screenPosition + Projectile.Size / 2f);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        //for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        //{
        //    float mult = 1f - (i / (float)Projectile.oldPos.Length);
        //    Color c = Color.Lerp(new Color(45, 123, 204, 200), new Color(147,216,45,100), mult * mult) * mult;
        //    Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, grayFrame, c * Projectile.Opacity, Projectile.oldRot[i], frame.Size() / 2, 0.5f + mult * 0.5f, SpriteEffects.None);
        //}

        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, frame, new Color(255, 255, 255, 200) * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, 1f, SpriteEffects.None);
        outerScale = Utils.Remap(Projectile.timeLeft, 0, 30, 0, 1) * Projectile.Opacity;
        if (outerScale > 0)
            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, grayFrame, Color.White with { A = 0 } * Projectile.Opacity * (1f - outerScale), Projectile.rotation, frame.Size() / 2, 1, SpriteEffects.None);
        return false;
    }
    private Color StripColors(float progressOnStrip)
    {
        return Color.Lerp(new Color(147, 216, 45, 100), new Color(45, 123, 204, 200), progressOnStrip * 2) * (1f - progressOnStrip);
    }
    private float StripWidth(float progressOnStrip)
    {
        return (20 - progressOnStrip * 20) * Utils.Remap(Projectile.timeLeft, 0, 60, 0, 1);
    }
}

public class TerraGlobeOrb : ModProjectile
{
    private Projectile ParentProjectile
    {
        get => Main.projectile.FirstOrDefault(x => x.identity == (int)Projectile.ai[0] && x.owner == Projectile.owner);
        set => Projectile.ai[0] = value.whoAmI;
    }
    public ref float BaseDamage => ref Projectile.ai[1];
    public Vector2 startOffset;
    public Vector2 offsetVel;
    public bool canFollow = true;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.QuickDefaults();
        Projectile.timeLeft = (int)(60 * 3f);
        Projectile.penetrate = 3;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15;
        Projectile.ignoreWater = true;
        Projectile.Opacity = 0;
        //Projectile.extraUpdates = 1;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(startOffset);
        writer.WriteVector2(offsetVel);
        writer.Write(canFollow);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        startOffset = reader.ReadVector2();
        offsetVel = reader.ReadVector2();
        canFollow = reader.ReadBoolean();
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(ModContent.BuffType<Terraburn>(), TerraGlobe.DEBUFF_DURATION);
    }
    public override void OnSpawn(IEntitySource source)
    {
        startOffset = new(Projectile.Center.X - ParentProjectile.Center.X, Projectile.Center.Y - ParentProjectile.Center.Y);
    }
    public override void AI()
    {
        Projectile.Opacity += 0.1f;
        if (canFollow && ParentProjectile != null)
        {
            //Vector2 dir = Projectile.Center.DirectionTo(ParentProjectile.Center);
            //Projectile.velocity += (dir);
            //Projectile.velocity = Projectile.velocity.LengthClamp(15, 2);
            Projectile.Center = ParentProjectile.Center + startOffset;

            Vector2 dir = (ParentProjectile.Center + startOffset).DirectionTo(ParentProjectile.Center);
            offsetVel += dir;
            offsetVel = offsetVel.RotatedBy(MathHelper.ToRadians(2));
            offsetVel = offsetVel.LengthClamp(8, 2);
            startOffset += offsetVel;

            Projectile.timeLeft = (int)(60 * 3f);

            if (ParentProjectile.timeLeft <= 0)
            {
                canFollow = false;
                Projectile.velocity = (Projectile.Center - ParentProjectile.Center) / 8;
                Projectile.velocity = Projectile.velocity.LengthClamp(8, 4);
            }
        }
        else
        {
            Projectile.damage = (int)BaseDamage;
            int target = Projectile.FindTargetWithLineOfSight(500);

            if (target != -1)
            {
                Projectile.velocity += Projectile.DirectionTo(Main.npc[target].Center) * 1f;
                Projectile.velocity = Projectile.velocity.LengthClamp(15, 2);
            }
            else
            {
                NPC targetAttempt2 = Projectile.FindTargetWithinRange(250);
                if (targetAttempt2 != null)
                {
                    Projectile.velocity += Projectile.DirectionTo(targetAttempt2.Center) * 1f;
                    Projectile.velocity = Projectile.velocity.LengthClamp(6, 2);
                }
                else
                {
                    Projectile.velocity *= 0.96f;
                }
            }
        }

        //dust
        if (Main.rand.NextBool(2))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terra);
            d.noLight = true;
            d.velocity *= 0.2f;
            d.noGravity = true;
            //d.velocity += Projectile.velocity * 0.4f;
            d.scale = Main.rand.NextFloat(0.5f, 1f);
        }
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 5; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terra);
            d.velocity += Projectile.velocity * 0.1f;
        }
    }
    private static VertexStrip _vertexStrip = new VertexStrip();
    public override bool PreDraw(ref Color lightColor)
    {
        MiscShaderData miscShaderData = GameShaders.Misc["TerraGlobeOrb"];
        miscShaderData.UseOpacity(Projectile.Opacity);
        miscShaderData.UseSaturation(-1);
        miscShaderData.Apply();
        _vertexStrip.PrepareStripWithProceduralPadding(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, -Main.screenPosition + Projectile.Size / 2f);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        //Rectangle grayFrame = new Rectangle(0, tex.Height() / 2, tex.Width(), tex.Height() / 2);
        Rectangle frame = new Rectangle(0, 0, tex.Width(), tex.Height());
        //for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        //{
        //    float mult = 1f - (i / (float)Projectile.oldPos.Length);
        //    Color c = Color.Lerp(new Color(45, 123, 204, 128), new Color(147, 216, 45, 32), mult * mult) * mult;
        //    Main.EntitySpriteDraw(tex.Value, Vector2.Lerp(Projectile.oldPos[i],Projectile.position,0.5f) - Main.screenPosition + Projectile.Size / 2, grayFrame, c * Projectile.Opacity, Projectile.oldRot[i], frame.Size() / 2, 0.5f + mult * 0.5f, SpriteEffects.None);
        //}
        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, frame, new Color(255, 255, 255, 255) * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, 1f, SpriteEffects.None);

        Asset<Texture2D> glow = TextureAssets.Extra[ExtrasID.ThePerfectGlow];
        float glowFade = Utils.Remap(Projectile.timeLeft, 0, 60, 0, 1);
        float glowScale = ((float)Math.Sin((Main.timeForVisualEffects + Projectile.identity * 10) * 0.15f) * 0.25f + 1f) * glowFade;
        Color c2 = Color.Lerp(new Color(45, 123, 204, 64), new Color(113, 186, 98, 128), (float)Math.Sin((Main.timeForVisualEffects + Projectile.identity * 10) * 0.05f) * 0.5f + 0.5f) * glowFade;
        for (int i = 0; i < 2; i++)
        {
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, c2 * Projectile.Opacity * 0.75f, MathHelper.PiOver2 * i, glow.Size() / 2, new Vector2(1.2f, 1f) * glowScale, SpriteEffects.None);
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f) * 0.5f * Projectile.Opacity * 0.5f, MathHelper.PiOver2 * i, glow.Size() / 2, new Vector2(1f, 0.8f) * glowScale, SpriteEffects.None);
        }

        return false;
    }
    private Color StripColors(float progressOnStrip)
    {
        return Color.Lerp(new Color(45, 123, 204, 128), new Color(113, 186, 98, 128), (float)Math.Sin((Main.timeForVisualEffects + Projectile.identity * 10) * 0.05f) * 0.5f + 0.5f) * (1f - progressOnStrip);
    }
    private float StripWidth(float progressOnStrip)
    {
        return (10 - progressOnStrip * 10) * Utils.Remap(Projectile.velocity.Length(), 1, 3, 0, 1) * Utils.Remap(Projectile.timeLeft, 0, 60, 0, 1); ;
    }
}

public class TerraGlobeExplosion : ModProjectile
{
    public override string Texture => ModContent.GetModProjectile(ModContent.ProjectileType<TerraGlobeProjectile>()).Texture;
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 150);
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 9;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.Opacity = 0f;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(ModContent.BuffType<Terraburn>(), TerraGlobe.DEBUFF_DURATION);
    }
    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, new Vector3(0.9f, 1f, 0f) * Projectile.timeLeft / 14f); // R G B values from 0 to 1f.
        if (Projectile.timeLeft == 8)
        {
            for (int i = 0; i < 7; i++)
            {
                var p = VanillaParticles.RequestPrettySparkleParticle();
                p.ColorTint = new Color(0.2f, 0.85f, 0.4f, 0.5f);
                p.Velocity = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(5, 7);
                p.LocalPosition = Projectile.Center;
                p.Rotation = p.Velocity.ToRotation();
                p.Scale = new Vector2(6, 0.75f);
                p.FadeInNormalizedTime = 0.95f;
                p.FadeOutNormalizedTime = 0.95f;
                p.TimeToLive = Main.rand.Next(10, 20);
                p.AdditiveAmount = 0.35f;
                p.DrawVerticalAxis = false;
                Main.ParticleSystem_World_OverPlayers.Add(p);
            }
            for (int i = 0; i < 2; i++)
            {
                var p2 = VanillaParticles.RequestFadingParticle();
                p2.ColorTint = i == 0 ? new Color(0.2f, 0.85f, 0.4f, 0.5f) : new Color(45, 123, 204, 128);
                p2.SetBasicInfo(TextureAssets.Extra[ExtrasID.KeybrandRing], null, Vector2.Zero, Projectile.Center);
                int time = Main.rand.Next(10, 15);
                p2.SetTypeInfo(time);
                p2.Rotation = Main.rand.NextFloatDirection();
                p2.RotationVelocity = Main.rand.NextFloat(-0.1f, 0.1f);
                p2.ScaleVelocity = Vector2.One.RotatedByRandom(0.6f) / time;
                p2.Scale = p2.ScaleVelocity * time / 2f;
                p2.FadeInNormalizedTime = 0.2f;
                p2.FadeOutNormalizedTime = 0.5f;
                Main.ParticleSystem_World_OverPlayers.Add(p2);
            }

            SoundEngine.PlaySound(SoundID.Item14 with { Pitch = 0.3f, PitchVariance = 0.2f, Volume = 0.6f }, Projectile.position);
            for (int i = 0; i < 25; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(15f, 15f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Terra, speed);
                d.noGravity = !Main.rand.NextBool(10);
                d.scale = Main.rand.NextFloat(0.8f, 1.2f);
                d.fadeIn = Main.rand.NextFloat(1.4f);
            }
        }
    }
}