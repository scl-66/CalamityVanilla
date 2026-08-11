using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Space.Items;

public class Cloudfall : ModItem
{
    public override string Texture => Assets.Textures.Space.Items.Cloudfall.CloudfallItem.KEY;

    public override void SetDefaults()
    {
        Item.DefaultToBow(16, 7, true);
        Item.damage = 20;
        Item.knockBack = 1;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(0, 0, 50, 0);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (type == ProjectileID.WoodenArrowFriendly)
            type = ModContent.ProjectileType<CloudArrow>();
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.Anvils).AddIngredient(ItemID.GoldBow).AddIngredient(ItemID.Feather, 15).AddIngredient(ItemID.Cloud, 35).Register();
        CreateRecipe().AddTile(TileID.Anvils).AddIngredient(ItemID.PlatinumBow).AddIngredient(ItemID.Feather, 15).AddIngredient(ItemID.Cloud, 35).Register();
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2, 0);
    }
}

public class CloudArrow : ModProjectile
{
    public override string Texture => Assets.Textures.Space.Items.Cloudfall.CloudArrow.KEY;

    public override void SetDefaults()
    {
        Projectile.QuickDefaults();
        Projectile.arrow = true;
        Projectile.DamageType = DamageClass.Ranged;
    }
    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud);
        d.velocity *= 0.1f;
        d.alpha = 128;
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud);
            d.velocity *= 0.5f;
            d.noGravity = !Main.rand.NextBool(3);
        }
    }
}