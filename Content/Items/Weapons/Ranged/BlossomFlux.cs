using CalamityVanilla.Content.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Items.Weapons.Ranged
{
    public class BlossomFlux : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToBow(7, 14, true);
            Item.useAnimation *= 5;
            Item.consumeAmmoOnFirstShotOnly = true;
            Item.damage = 28;
            Item.knockBack = 2;
            Item.UseSound = null;

            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 5);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddTile(TileID.MythrilAnvil)
                .AddIngredient(ItemID.BeesKnees)
                .AddIngredient(ItemID.HellwingBow)
                .AddIngredient(ItemID.BloodRainBow)
                .AddIngredient(ItemID.IceBow)
                .AddIngredient(ItemID.ChlorophyteBar,10)
                .Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item5,player.position);

            if (player.ItemAnimationJustStarted)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BlossomFluxBomb>(), damage * 2, knockback * 2, player.whoAmI);
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(0.1f);
            if(type == ProjectileID.WoodenArrowFriendly)
            {
                type = ModContent.ProjectileType<BlossomFluxLeaf>();
            }
        }
    }
}
