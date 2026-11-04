using CalamityVanilla.Common.Interfaces;
using CalamityVanilla.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Space.Items.WindowPain;
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
    public override bool ModifyItemDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, ref DrawData? coloredDrawData, ref DrawData? glowMaskDrawData)
    {
        if(drawInfo.drawPlayer.GetModPlayer<WindowPainPlayer>().Shattered)
            drawData.texture = AssetReferences.Content.Space.Items.WindowPain.WindowPainShattered.Asset.Value;
        return true;
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

    public bool SyncedOnHitNPC(Player player, NPC target, Rectangle attackHitbox, int damage, float knockback, bool crit, int hitDirection, Projectile? projectile)
    {
        if (!target.CanBeChasedBy())
            return false;
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
            return true;
        }
        return false;
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