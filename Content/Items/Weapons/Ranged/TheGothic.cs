using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityVanilla.Content.Projectiles.Ranged;
using Terraria.DataStructures;

namespace CalamityVanilla.Content.Items.Weapons.Ranged
{
    public class TheGothic : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToBow(14, 7, true);
            Item.damage = 35;
            Item.knockBack = 1;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 10, 0, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //float iterations = 0;
            //int rand = Main.rand.Next(3);
            //switch (rand)
            //{
            //    case 0:
            //        iterations = 0;
            //        break;
            //    case 1:
            //        iterations = 0.5f;
            //        break;
            //    case 2:
            //        iterations = 1;
            //        break;
            //}
            float iterations = Main.rand.NextBool() ? 0.5f : 1f;
            for (float i = -iterations; i <= iterations; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(i * Main.rand.NextFloat(0.05f,0.1f)), ModContent.ProjectileType<GothicTooth>(), damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2,0);
        }
    }
}
