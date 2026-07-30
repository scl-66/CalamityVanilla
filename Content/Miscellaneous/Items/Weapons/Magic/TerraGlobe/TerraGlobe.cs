using CalamityVanilla.Content.Corruption.Items.DarkPrismStaff;
using CalamityVanilla.Content.Crimson.Items.DentataWand;
using CalamityVanilla.Content.Miscellaneous.Items.Weapons.Magic.BlacklightScepter;
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
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Magic.TerraGlobe;

public class TerraGlobe : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<TerraGlobeProjectile>(), 40, 8, true);
        Item.damage = 35;
        Item.knockBack = 5;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 5);
        Item.holdStyle = ItemHoldStyleID.HoldFront;
        Item.UseSound = SoundID.Item67;
    }

    public override void HoldStyle(Player player, Rectangle heldItemFrame)
    {
        player.itemLocation.X -= 29f * player.direction;
        player.itemLocation.Y -= 28f;
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
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 5;
    }
    public override void SetDefaults()
    {
        Projectile.width = 36;
        Projectile.height = 36;
        Projectile.QuickDefaults(false, 24);
        Projectile.timeLeft = (int)(60 * 6f);
        Projectile.penetrate = 5;
        Projectile.DamageType = DamageClass.Magic;
        //Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        if (Projectile.timeLeft > 70)
        {
            Vector2 dir = Projectile.Center.DirectionTo(Main.MouseWorld);
            Projectile.velocity += (dir);
            Projectile.velocity = Projectile.velocity.LengthClamp(15, 3);
        }
        else
        {
            Projectile.velocity *= 0.93f;
            if (Projectile.timeLeft <= 30)
            {
                Projectile.Opacity = 0.5f;
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        Rectangle frame = tex.Frame(1, 1, 0, 0);
        float glowOpacity = MathF.Pow(Projectile.Opacity, 8);
        for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
        {
            float multiply = 1 - i / (float)ProjectileID.Sets.TrailCacheLength[Type];
            Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, frame, Color.Lerp(new Color(30, 150, 255, 0), Color.Lerp(new Color(255, 210, 50, 64), new Color(180, 255, 55, 64), Projectile.ai[2] / 3), multiply * multiply) * multiply * glowOpacity, Projectile.oldRot[i], frame.Size() / 2, 1f + multiply * 0.2f, SpriteEffects.FlipVertically);
        }

        Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, frame, new Color(1f, 1f, 1f, 1f) * Projectile.Opacity, Projectile.rotation, frame.Size() / 2, 1f, SpriteEffects.FlipVertically);

        //Asset<Texture2D> glow = TextureAssets.Extra[ExtrasID.ThePerfectGlow];
        //Vector2 velocityNormal = Vector2.Normalize(Projectile.velocity);
        //Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition + velocityNormal * frame.Height / 2, null, new Color(0.2f, 0.85f, 0.4f, 0.5f) * glowOpacity, Projectile.rotation, glow.Size() / 2, new Vector2(0.4f, 1f), SpriteEffects.FlipVertically);
        //Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition + velocityNormal * frame.Height / 2, null, new Color(1f, 1f, 1f, 0f) * 0.5f * glowOpacity, Projectile.rotation, glow.Size() / 2, new Vector2(0.2f, 0.8f), SpriteEffects.FlipVertically);

        //float sin = Utils.Remap((float)Math.Sin(Main.timeForVisualEffects * 0.5f), -1, 1, 0, 1);

        //Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition + velocityNormal * frame.Height / 2, null, new Color(0.2f, 0.85f, 0.4f, 0.5f) * glowOpacity, Projectile.rotation + MathHelper.PiOver2, glow.Size() / 2, new Vector2(0.4f, 0.7f * sin), SpriteEffects.FlipVertically);
        //Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition + velocityNormal * frame.Height / 2, null, new Color(1f, 1f, 1f, 0f) * 0.5f * glowOpacity, Projectile.rotation + MathHelper.PiOver2, glow.Size() / 2, new Vector2(0.2f, 0.5f * sin), SpriteEffects.FlipVertically);
        return false;
    }
}
