using CalamityVanilla.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Melee.WindowPain;

public class WindowPainAnimation : DrawAnimation
{
    public static bool Active = false;
    public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1)
    {
        return texture.Frame(1, 2, 0, Active ? 1 : 0);
    }
    private class WindowPainLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.GetModPlayer<WindowPainPlayer>().Shattered)
                Active = true;
        }
    }
    private class WindowPainLayer2 : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Active = false;
        }
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
        Main.RegisterItemAnimation(Type, new WindowPainAnimation());
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

    public void SyncedOnHitNPC(Player player, NPC target, int damage, float knockback, bool crit, int hitDirection)
    {
        if (target.type == NPCID.TargetDummy)
            return;
        WindowPainPlayer p = player.GetModPlayer<WindowPainPlayer>();
        if (!p.Shattered)
        {
            p.Shattered = true;
            SoundEngine.PlaySound(SoundID.Shatter with { PitchVariance = 0.3f }, player.position);
            if (Main.myPlayer == player.whoAmI)
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, new Vector2(player.direction * Main.rand.NextFloat(3, 9), Main.rand.NextFloat(-6, -2)), ModContent.ProjectileType<WindowPainShard>(), damage / 3, knockback / 3, player.whoAmI);
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