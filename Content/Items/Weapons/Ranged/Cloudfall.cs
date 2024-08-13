using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityVanilla.Content.Projectiles.Ranged;

namespace CalamityVanilla.Content.Items.Weapons.Ranged
{
    public class Cloudfall : ModItem
    {
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
            return new Vector2(-2,0);
        }
    }
}
