using CalamityVanilla.Common.Dusts;
using Daybreak.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Crimson.Items.DentataWand;

public class DentataWand : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToStaff(ModContent.ProjectileType<Dentata>(), 6f, 24, 6);

        // Set damage and knockBack
        Item.SetWeaponValues(30, 4);
        Item.UseSound = SoundID.NPCHit13;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.channel = true;

        // Set rarity and value
        Item.SetShopValues(ItemRarityColor.Blue1, 2000);
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.Anvils)
            .AddIngredient(ItemID.CrimtaneBar, 8)
            .AddIngredient(ItemID.TissueSample, 6)
            .Register();
    }

    public override bool CanUseItem(Player player)
    {
        if (player.ownedProjectileCounts[ModContent.ProjectileType<Dentata>()] < 1)
            return true;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type == ModContent.ProjectileType<Dentata>() && proj.owner == player.whoAmI && proj.ai[0] == 0)
            {
                return false;
            }
        }
        return true;
    }
}

public class Dentata : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 5;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
    }
    public override void SetDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        Projectile.QuickDefaults(false, 20);
        Projectile.DamageType = DamageClass.Magic;
        Projectile.tileCollide = true;
        DrawOffsetX = -4;
        DrawOriginOffsetY = -4;
    }

    public float xScale = 1f;
    public float yScale = 1f;
    public override void AI()
    {
        Projectile.ai[1]++;
        if (++Projectile.frameCounter >= 5)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }

        if (Main.rand.NextBool(2))
        {
            Vector2 dustPosition = Projectile.position + new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, 5));
            Vector2 vel = Main.rand.NextVector2Circular(Projectile.width / 4, Projectile.height / 8) + new Vector2(0, 0.3f) + (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 3;
            Dust dust = Dust.NewDustDirect(dustPosition, Projectile.width, Projectile.height, DustID.Blood, vel.X, vel.Y);
            dust.velocity.X *= 0.3f;
            dust.noGravity = false;
            dust.scale = Main.rand.NextFloat(0.85f, 1.2f);
        }

        if (Main.rand.NextBool(2))
        {
            Vector2 dustPosition = Projectile.position + new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, 5));
            Vector2 vel = Main.rand.NextVector2Circular(Projectile.width / 4, Projectile.height / 8) * 0.2f + (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 3;
            Dust dust = Dust.NewDustDirect(dustPosition, Projectile.width, Projectile.height, ModContent.DustType<DentataFleshChunk>(), vel.X, vel.Y);
            _ = dust.color;
            dust.velocity *= 0.75f;
            dust.noGravity = true;
            dust.fadeIn = 0.8f;
            dust.scale = Main.rand.NextFloat(0.8f, 1f);
        }

        // In Multi Player (MP) This code only runs on the client of the projectile's owner, this is because it relies on mouse position, which isn't the same across all clients.
        if (Main.myPlayer == Projectile.owner)
        {
            float maxDistance = 12f; // This also sets the maximun speed the projectile can reach while following the cursor.
            Player player = Main.player[Projectile.owner];
            // If the player channels the weapon, do something. This check only works if item.channel is true for the weapon.
            if (player.channel && player.HeldItem.shoot == Type && Projectile.ai[0] == 0f && (player.Center - Projectile.Center).Length() < 500f)
            {
                Vector2 mousePos = Main.MouseWorld;
                float trueDistToCursor = (Main.MouseWorld - Projectile.Center).Length();
                Vector2 vectorToCursor = mousePos - Projectile.Center;
                bool distCheck = trueDistToCursor < 15;

                //if (distCheck)
                //{
                //    vectorToCursor = Projectile.oldVelocity;
                //}

                float distanceToCursor = trueDistToCursor;

                // Here we can see that the speed of the projectile depends on the distance to the cursor.
                if (distanceToCursor > maxDistance)
                {
                    distanceToCursor = maxDistance / distanceToCursor;
                    vectorToCursor *= distanceToCursor;
                }

                int velocityXBy1000 = (int)(vectorToCursor.X * 1000f);
                int oldVelocityXBy1000 = (int)(Projectile.velocity.X * 1000f);
                int velocityYBy1000 = (int)(vectorToCursor.Y * 1000f);
                int oldVelocityYBy1000 = (int)(Projectile.velocity.Y * 1000f);

                // This code checks if the precious velocity of the projectile is different enough from its new velocity, and if it is, syncs it with the server and the other clients in MP.
                // We previously multiplied the speed by 1000, then casted it to int, this is to reduce its precision and prevent the speed from being synced too much.
                if (velocityXBy1000 != oldVelocityXBy1000 || velocityYBy1000 != oldVelocityYBy1000)
                {
                    Projectile.netUpdate = true;
                }

                if (!distCheck)
                {
                    Projectile.velocity = vectorToCursor;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity.RotatedByRandom(MathHelper.Pi / 2), 0.2f);
                }
                else
                {
                    xScale = Main.rand.NextFloat(0.9f, 1.35f);
                    yScale = Main.rand.NextFloat(0.9f, 1.35f);
                    maxDistance = 5;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 0.15f + Main.rand.NextVector2Circular(6, 6), 0.65f);
                    Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.05f);
                }

                // Set the rotation so the projectile points towards where it's going.
                if (Projectile.velocity != Vector2.Zero && !distCheck)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
            // If the player stops channeling, do something else.
            else if (Projectile.ai[0] == 0f)
            {
                // This code block is very similar to the previous one, but only runs once after the player stops channeling their weapon.
                Projectile.netUpdate = true;
                Vector2 vectorToCursor = Main.MouseWorld - Projectile.Center;
                float distanceToCursor = vectorToCursor.Length();

                //If the projectile was at the cursor's position, set it to move in the oposite direction from the player.
                if (distanceToCursor < 100)
                {
                    vectorToCursor = Projectile.Center - player.Center;
                    distanceToCursor = vectorToCursor.Length();
                }

                distanceToCursor = maxDistance / distanceToCursor;
                vectorToCursor *= distanceToCursor;
                Projectile.velocity = vectorToCursor;

                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.Kill();
                }

                Projectile.ai[0] = 1f;
            }

            if (Projectile.ai[0] == 1f)
            {
                Projectile.scale = 1f;
                xScale = 1f;
                yScale = 1f;
                Projectile.velocity.Y += 0.5f;
                Projectile.velocity.X *= 0.99f;

                if (Projectile.velocity.Y > 16f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
                {
                    Projectile.velocity.Y = 16f;
                }

                // Set the rotation so the projectile points towards where it's going.
                if (Projectile.velocity != Vector2.Zero)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
        }
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
        SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.position);
        for (int i = 0; i < 10; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 0, Color.White, 1f);
            dust.noGravity = false;
            dust.velocity *= 2f;
            dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 0, Color.White, 1.4f);
        }

        for (int i = 0; i < 20; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.Crimson);
            dust.noGravity = false;
            dust.velocity *= 2f;
            dust.scale = Main.rand.NextFloat(0.75f, 1.1f);
        }

        for (int i = 0; i < 10; i++)
        {
            Vector2 dustPosition = Projectile.Center + new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, 5));
            Vector2 vel = Main.rand.NextVector2Circular(Projectile.width / 4, Projectile.height / 4) - Projectile.velocity;
            Dust dust = Dust.NewDustDirect(dustPosition, Projectile.width, Projectile.height, ModContent.DustType<DentataFleshChunk>(), vel.X, vel.Y);
            dust.velocity *= 0.5f;
            dust.noGravity = true;
            dust.fadeIn = .8f;
            dust.scale = Main.rand.NextFloat(0.3f, 0.8f);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Rectangle frame = tex.Frame(1, 5, 0, Projectile.frame);
        Vector2 origin = frame.Size() / 2 + new Vector2(0, 2);
        //if (Main._multiplyBlendState == null)
        //{
        //    Main._multiplyBlendState = new BlendState
        //    {
        //        ColorBlendFunction = BlendFunction.ReverseSubtract,
        //        ColorDestinationBlend = Blend.One,
        //        ColorSourceBlend = Blend.SourceColor,
        //        AlphaBlendFunction = BlendFunction.ReverseSubtract,
        //        AlphaDestinationBlend = Blend.One,
        //        AlphaSourceBlend = Blend.SourceColor
        //    };
        //}
        //Main.spriteBatch.End(out var ss);
        //Main.spriteBatch.Begin(ss with
        //{
        //    BlendState = Main._multiplyBlendState
        //});

        //for (int k = 0; k < Projectile.oldPos.Length - 2; k++)
        //{
        //    int length = 4;
        //    for (int i = 0; i < 3; i++)
        //    {
        //        Rectangle drawFrame = tex.Frame(1, 5, 0, Math.Abs((Projectile.frame - k) % 4));
        //        Vector2 drawPos = (Vector2.Lerp(Projectile.oldPos[k], Projectile.oldPos[k + 1], i / (float)length) + Projectile.Size / 2 - Main.screenPosition);
        //        float drawRotation = (Utils.AngleLerp(Projectile.oldRot[k], Projectile.oldRot[k + 1], i / (float)length));
        //        Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
        //        Main.EntitySpriteDraw(tex, drawPos, drawFrame, (color * 1f).MultiplyRGBA(Color.Cyan), drawRotation, origin, Projectile.scale, SpriteEffects.None);
        //    }
        //}
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(ss);

        MiscShaderData miscShaderData = GameShaders.Misc["Dentata"];
        miscShaderData.UseSaturation(-2);
        miscShaderData.UseOpacity(Projectile.Opacity);
        miscShaderData.Apply();
        _vertexStrip.PrepareStripWithProceduralPadding(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, -Main.screenPosition + Projectile.Size / 2f);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, new Vector2(xScale, yScale) * Projectile.scale, SpriteEffects.None);

        return false;
    }
    private static VertexStrip _vertexStrip = new VertexStrip();
    private Color StripColors(float progressOnStrip)
    {
        if (progressOnStrip is float.NaN)
            return Color.Transparent;
        return new Color(Lighting.GetSubLight(Projectile.oldPos[(int)Utils.Remap(progressOnStrip, 0, 1, 0, Projectile.oldPos.Length - 1)] + Projectile.Size / 2));
    }
    private static float StripWidth(float progressOnStrip)
    {
        return 14 - (progressOnStrip * 14);
    }
    public override void Load()
    {
        MiscShaderData shader = new MiscShaderData(Main.Assets.Request<Effect>("PixelShader"), "MagicMissile").UseProjectionMatrix(doUse: true);
        shader.UseImage2(ModContent.Request<Texture2D>(Texture + "Erosion"));
        shader.UseImage1(ModContent.Request<Texture2D>(Texture + "Shape"));
        shader.UseImage0(ModContent.Request<Texture2D>(Texture + "Gradient"));
        GameShaders.Misc.Add("Dentata", shader);
    }
}