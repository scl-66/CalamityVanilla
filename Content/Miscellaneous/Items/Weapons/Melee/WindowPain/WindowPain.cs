using CalamityVanilla.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Melee.WindowPain;

// Jank Central oh my lord
public class WindowPainAnimation : PlayerDrawLayer
{
    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<WindowPain>() && drawInfo.drawPlayer.ItemAnimationActive;
    }
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        int frame = drawInfo.drawPlayer.GetModPlayer<WindowPainPlayer>().Shattered? 1 : 0;
        Asset<Texture2D> texture = TextureAssets.Item[drawInfo.heldItem.type];
        Vector2 basePosition = drawInfo.drawPlayer.itemLocation - Main.screenPosition;
        basePosition = new Vector2((int)basePosition.X, (int)basePosition.Y) + (drawInfo.drawPlayer.RotatedRelativePoint(drawInfo.drawPlayer.Center) - drawInfo.drawPlayer.Center);
        Item heldItem = drawInfo.drawPlayer.HeldItem;

        DrawData swingDraw = new DrawData(
        texture.Value, // texture
        basePosition, // position
        new Rectangle(0, texture.Height() / 2 * frame, texture.Width(), texture.Height() / 2), // texture coords
        new Color(Lighting.GetSubLight(drawInfo.drawPlayer.Center)), // color (wow really!?)
        drawInfo.drawPlayer.itemRotation,  // rotation
        new Vector2(drawInfo.drawPlayer.direction == -1 ? texture.Value.Width : 0, // origin X
        drawInfo.drawPlayer.gravDir == 1 ? texture.Value.Height / 2 : 0), // origin Y
        drawInfo.drawPlayer.GetAdjustedItemScale(heldItem), // scale
        drawInfo.itemEffect // sprite effects
        );
        drawInfo.DrawDataCache.Add(swingDraw);
        drawInfo.ItemLocation = Vector2.Zero;
    }
}
public class WindowPainPlayer : ModPlayer
{
    public bool Shattered = false;
}
public class WindowPain : ModItem, ISyncedOnHitEffect
{
    public override void SetDefaults()
    {
        Item.DefaultToSword(20, 28, 6);
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(0, 0, 75);
    }
    public override void SetStaticDefaults()
    {
        DrawAnimationVertical animation = new DrawAnimationVertical(-1, 2, false);
        animation.NotActuallyAnimating = true;
        Main.RegisterItemAnimation(Type, animation);
    }
    public override bool? UseItem(Player player)
    {
        if (player.ItemAnimationJustStarted)
            player.GetModPlayer<WindowPainPlayer>().Shattered = false;
        return base.UseItem(player);
    }
    public override bool? CanHitNPC(Player player, NPC target)
    {
        if (player.ItemAnimationJustStarted)
            return false;
        return base.CanHitNPC(player, target);
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.Anvils).AddIngredient(ItemID.GoldBroadsword).AddIngredient(ItemID.Glass, 25).AddIngredient(ItemID.SunplateBlock, 10).Register();
        CreateRecipe().AddTile(TileID.Anvils).AddIngredient(ItemID.PlatinumBroadsword).AddIngredient(ItemID.Glass, 25).AddIngredient(ItemID.SunplateBlock, 10).Register();
    }

    public void SyncedOnHitNPC(Player player, NPC target, bool crit, int hitDirection)
    {
        WindowPainPlayer p = player.GetModPlayer<WindowPainPlayer>();
        if (!p.Shattered)
        {
            p.Shattered = true;
            SoundEngine.PlaySound(SoundID.Shatter with { PitchVariance = 0.3f }, player.position);
            for (int i = 0; i < 5; i++)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, new Vector2(player.direction * Main.rand.NextFloat(3, 9), Main.rand.NextFloat(-6, -2)), ModContent.ProjectileType<WindowPainShard>(), Item.damage / 3, Item.knockBack / 3, player.whoAmI);
            }
            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustPerfect(player.Center + new Vector2(player.direction * 17, 0) + Main.rand.NextVector2Circular(24, 12), DustID.Glass, new Vector2(player.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-3, -1)));
                d.noGravity = Main.rand.NextBool();
            }
        }
    }
}

public class WindowPainShard : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.QuickDefaults();
        Projectile.penetrate = 2;
        Projectile.DamageType = DamageClass.Melee;
    }
    public override void AI()
    {
        Projectile.rotation += Projectile.velocity.X * 0.1f;
        Projectile.velocity.Y += 0.3f;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}