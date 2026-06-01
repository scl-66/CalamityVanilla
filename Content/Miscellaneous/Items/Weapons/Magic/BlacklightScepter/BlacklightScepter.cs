using CalamityVanilla.Content.Corruption.Items.DarkPrismStaff;
using CalamityVanilla.Content.Crimson.Items.DentataWand;
using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Particles;
using CalamityVanilla.Content.Underground.Items.GraniteTome;
using CalamityVanilla.Content.Underground.Items.MarbleTome;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Biomes;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using UtfUnknown.Core.Probers.MultiByte.Korean;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Magic.BlacklightScepter;

public class BlacklightScepter : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }
    public override void SetDefaults()
    {
        Item.DefaultToStaff(ModContent.ProjectileType<BlacklightEnergy>(), 15, 20, 13);
        Item.damage = 35;
        Item.knockBack = 5;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 5);
        Item.UseSound = SoundID.Item43 with
        {
            Pitch = 1f,
            Volume = 0.75f,
            PitchVariance = 0.8f,
            MaxInstances = 0,
        };
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int rand = Main.rand.Next(0, 2);
        for (int i = -1; i < rand; i++)
        {
            float randomWidth = MathHelper.Pi / 55;
            float spread = MathHelper.Pi / 25;
            float randRotation = Main.rand.NextFloat(-randomWidth, randomWidth);
            float randVel = 0.75f + Main.rand.NextFloat(-randomWidth, randomWidth) * 1.5f;
            int randType = Main.rand.Next(0, 3);
            if (rand == 2)
            {
                randRotation = spread + (i * spread*2);
            }
            if (randType == 2)
            {
                randRotation += MathHelper.Pi/16 * -player.direction;
                velocity *= 1.5f;
            }
            Projectile.NewProjectile(source, position + new Vector2(35, 0).RotatedBy(position.DirectionTo(Main.MouseWorld).ToRotation()), velocity.RotatedBy(randRotation) * randVel, type, (int)(damage / 1.5f), knockback, player.whoAmI, randType, 0, Item.shootSpeed);
        }
        return false;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddTile(TileID.DemonAltar)
            .AddIngredient<DarkPrismStaff>()
            .AddIngredient<GraniteTome>()
            .AddIngredient<MarbleTome>()
            .AddIngredient(ItemID.FlowerofFire)
            .Register();
        CreateRecipe()
            .AddTile(TileID.DemonAltar)
            .AddIngredient<DentataWand>()
            .AddIngredient<GraniteTome>()
            .AddIngredient<MarbleTome>()
            .AddIngredient(ItemID.FlowerofFire)
            .Register();
    }
}

public class BlacklightEnergy : ModProjectile
{
    public ref float energyType => ref Projectile.ai[0];
    public ref float timer => ref Projectile.ai[1];
    public Vector2 startVel;
    public Vector2 spawnPos;
    public int playerDirection;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
    }

    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 20);
        Projectile.timeLeft = 120;
        Projectile.extraUpdates = 1;
        Projectile.alpha = 255;
        Projectile.penetrate = 3;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        DrawOffsetX = -10;
    }

    private Color GetDustColor()
    {
        Color color = Color.White;

        switch (Projectile.ai[0])
        {
            case 0:
                color = new Color(75, 255, 115);
                break;
            case 1:
                color = new Color(255, 169, 114);
                break;
            case 2:
                color = new Color(255, 142, 192);
                break;
        }
        color.A = 64;
        return color;
    }

    public override void OnSpawn(IEntitySource source)
    {
        startVel = Projectile.velocity;
        spawnPos = Projectile.position;
        playerDirection = Main.player[Projectile.owner].direction;
    }

    public override void AI()
    {
        float mult = Utils.Remap(Projectile.velocity.Length(), 0, 20, 0.5f, 3f);
        Projectile.scale = 1 + (MathF.Sin(Projectile.timeLeft * 0.45f) * mult/10);
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.alpha > 25)
        {
            Projectile.alpha -= 13;
        }

        Projectile.ai[1]++;
        if (Projectile.ai[1] > 30 && Projectile.ai[0] != 2)
        {
            Projectile.velocity *= 0.95f;
        }

        if (Projectile.ai[0] == 0)
        {
            spawnPos += Projectile.velocity;
            float amp = 10 * (playerDirection == 1 ? 1 : -1);
            if (Projectile.ai[1] % 1 == 0)
            {
                float sine = MathHelper.Lerp(-amp, amp, (float)Math.Sin((Projectile.ai[1] / 20f * Projectile.velocity.Length()) + 1f) / 2f);
                Vector2 offset = new Vector2(0, sine).RotatedBy(Projectile.velocity.ToRotation());
                //offset = Vector2.Zero;
                Projectile.position = spawnPos + offset;
                Projectile.rotation = (Projectile.position - Projectile.oldPosition).ToRotation();
            }
        } else if (Projectile.ai[0] == 2)
        {
            Projectile.velocity.Y += 0.5f;
            Projectile.velocity.X *= 0.99f;
        }


        Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SimpleColorableGlowyDust>());
        d.noLight = true;
        d.velocity *= 0.3f;
        d.noGravity = true;
        d.velocity += Projectile.velocity * 0.4f;
        d.color = Color.Lerp(GetDustColor(), Color.White, Main.rand.NextFloat(0f, 0.6f)) with { A = 0 };
        d.scale = Projectile.scale;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        if (Projectile.ai[0] == 1)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
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
        }
        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override void OnKill(int timeLeft)
    {
        if (timeLeft > 20)
        {
            int length = ProjectileID.Sets.TrailCacheLength[Type];
            int t = ModContent.DustType<SimpleColorableGlowyDust>();
            for (int i = 1; i < length; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, t);
                d.color = GetDustColor() * Projectile.Opacity * (1f - i / (float)length);
                d.noGravity = d.noLight = true;
                d.scale = 1.5f;
                d.velocity += Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * 3;
                d.velocity *= 0.4f;
            }
        }
        PrettySparkleParticle sparkle = VanillaParticles.RequestPrettySparkleParticle();
        sparkle.LocalPosition = Projectile.Center;
        sparkle.Scale = new Vector2(Main.rand.NextFloat(2.7f, 3.3f), Main.rand.NextFloat(0.9f, 1.1f));
        sparkle.Rotation = MathHelper.PiOver2 + Main.rand.NextFloat(-0.1f, 0.1f);
        sparkle.DrawVerticalAxis = true;
        sparkle.ColorTint = GetDustColor();
        sparkle.FadeInEnd = 5;
        sparkle.FadeOutStart = 5;
        sparkle.FadeOutEnd = 40;
        Main.ParticleSystem_World_OverPlayers.Add(sparkle);

        for (int i = 0; i < 10; i++)
        {
            int t = ModContent.DustType<SimpleColorableGlowyDust>();
            Dust d = Dust.NewDustPerfect(Projectile.Center, t, new Vector2(0, Main.rand.NextFloat(-5, 5)).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.1f, 0.1f)));
            d.noGravity = true;
            d.scale = Main.rand.NextFloat(0.8f, 1.2f);
            d.color = GetDustColor();
            Dust d2 = Dust.NewDustPerfect(Projectile.Center, t, new Vector2(Main.rand.NextFloat(-7, 7), 0).RotatedBy(sparkle.Rotation + Main.rand.NextFloat(-0.1f, 0.1f)));
            d2.scale = Main.rand.NextFloat(0.8f, 1.2f);
            d2.noGravity = true;
            d2.color = GetDustColor();

            Dust d3 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, t);
            d3.color = GetDustColor();
            d3.velocity *= 2;
            d3.noGravity = true;
            d3.fadeIn = Main.rand.NextFloat(1.3f);
        }

        if (Projectile.ai[0] == 2)
        {
            for (int i = 0; i < Main.rand.Next(2,4); i++)
            {
                Vector2 speed = (Main.rand.NextVector2Unit(-MathHelper.PiOver4, MathHelper.PiOver2) * 5 * Main.rand.NextFloat(0, 2)).RotatedBy(Projectile.velocity.ToRotation()).RotatedBy(0);
                speed = Main.rand.NextVector2Circular(8, 8);
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.position, speed, ProjectileID.CrystalPulse2, Projectile.damage / 2, Projectile.knockBack / 2);
            }
            for (int i = 0; i < 20; i++)
            {
                int t = ModContent.DustType<SimpleColorableGlowyDust>();
                Vector2 speed = (Main.rand.NextVector2Unit(-MathHelper.PiOver4, MathHelper.PiOver2) * 5 * Main.rand.NextFloat(0, 2)).RotatedBy(Projectile.velocity.ToRotation()).RotatedBy(0);
                Dust d = Dust.NewDustPerfect(Projectile.Center, t, speed);
                d.noGravity = false;
                d.velocity.Y = -Math.Abs(d.velocity.Y);
                d.velocity *= 0.5f;
                d.scale = Main.rand.NextFloat(0.62f, 1f);
                d.scale *= 0.5f;
                d.color = GetDustColor();
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        Asset<Texture2D> glowTex = TextureAssets.Extra[ExtrasID.ThePerfectGlow];
        Rectangle frame = tex.Frame(3, 1, (int)Projectile.ai[0], 0);
        int length = ProjectileID.Sets.TrailCacheLength[Type];
        int num = 4;
        Color col = GetDustColor() * Utils.Remap(Projectile.velocity.Length(), 0, startVel.Length(), 0.1f, 1);
        for (int k = Projectile.oldPos.Length - (num - 2); k > 0; k--)
        {
            for (int i = 0; i < num - 1; i++)
            {
                float percent = 1f - (k / (float)Projectile.oldPos.Length);
                Vector2 drawPos = (Vector2.Lerp(Projectile.oldPos[k], Projectile.oldPos[k + 1], i / (float)length) - Main.screenPosition);
                Main.EntitySpriteDraw(glowTex.Value, drawPos + Projectile.Size / 2, null, col * Projectile.Opacity * (1f - k / (float)length), Projectile.oldRot[k] + MathHelper.PiOver2, glowTex.Size() / 2, new Vector2(0.5f, (length - k) / 5), SpriteEffects.None);
            }
        }
        Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition, null, col * Projectile.Opacity * 2f, Projectile.rotation + MathHelper.PiOver2, glowTex.Size() / 2, 0.65f * Projectile.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation + MathHelper.PiOver2, tex.Size() / 2 - new Vector2(tex.Width()/3, 0), Projectile.scale, SpriteEffects.None);
        //Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        //Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, 
        //    tex.Frame(3, Main.projFrames[Type], (int)Projectile.ai[0], Projectile.frame), 
        //    new Color(1f, 1f, 1f, 0.8f) * Projectile.Opacity, Projectile.rotation, 
        //    new Vector2(9, 11), Projectile.scale, SpriteEffects.None);
        return false;
    }

}
