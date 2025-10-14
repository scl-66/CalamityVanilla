using CalamityVanilla.Content.Miscellaneous.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.MoneyShot;

public class MoneyShot : ModItem
{
    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.TinkerersWorkbench).AddIngredient(ItemID.Boomstick).AddIngredient(ModContent.ItemType<GoblinScrap>(), 10).AddIngredient(ItemID.Wire, 30).Register();
    }
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Coin, 40, 8f, true);
        Item.width = 16;
        Item.height = 16;
        Item.knockBack = 2f;
        Item.UseSound = SoundID.Item36;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Orange;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        for (int i = 0; i < 3; i++)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.8f, 1.2f), type, (int)(damage * 0.75f), knockback, player.whoAmI, 0, 0, 200);
        }
        return false;
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-14, -4);
    }
}
public class MoneyShotCoinShortener : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return entity.type is ProjectileID.CopperCoin or ProjectileID.SilverCoin or ProjectileID.GoldCoin or ProjectileID.PlatinumCoin;
    }
    public override bool PreDraw(Projectile projectile, ref Color lightColor)
    {
        if (projectile.ai[2] == 200)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[projectile.type].Value, projectile.Center - Main.screenPosition, new Rectangle(0, 0, 6, 6), lightColor, projectile.rotation, new Vector2(3, 6), projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(TextureAssets.Projectile[projectile.type].Value, projectile.Center - Main.screenPosition, new Rectangle(0, 12, 6, 4), lightColor, projectile.rotation, new Vector2(3, 0), projectile.scale, SpriteEffects.None);
            return false;
        }
        return base.PreDraw(projectile, ref lightColor);
    }
}