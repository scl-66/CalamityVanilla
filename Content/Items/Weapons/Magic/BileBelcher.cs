using CalamityVanilla.Content.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Items.Weapons.Magic
{
    public class BileBelcher : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToStaff(ModContent.ProjectileType<IchorBlob>(), 16, 12, 6);
            Item.UseSound = SoundID.NPCHit1;
            Item.damage = 50;
        }
        public override float UseSpeedMultiplier(Player player)
        {
            return 0.5f + (player.statMana / (float)player.statManaMax2);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.9f,1.1f);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.rand.NextBool(4))
            {
                SoundEngine.PlaySound(SoundID.NPCDeath13, player.position);
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.8f, 1.2f), ModContent.ProjectileType<BileBone>(), damage / 3 * 5, knockback, player.whoAmI);
            }
            return true;
        }
    }
}
