using CalamityVanilla.Content.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Items.Weapons.Magic
{
    // ExampleStaff is a typical staff. Staffs and other shooting weapons are very similar, this example serves mainly to show what makes staffs unique from other items.
    // Staff sprites, by convention, are angled to point up and to the right. "Item.staff[Type] = true;" is essential for correctly drawing staffs.
    // Staffs use mana and shoot a specific projectile instead of using ammo. Item.DefaultToStaff takes care of that.
    public class MyceliumStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true; // This makes the useStyle animate as a staff instead of as a gun.
        }

        public override void SetDefaults()
        {
            // DefaultToStaff handles setting various Item values that magic staff weapons use.
            // Hover over DefaultToStaff in Visual Studio to read the documentation!
            Item.DefaultToStaff(ModContent.ProjectileType<MyceliumShroom>(), 16, 25, 12);

            // Customize the UseSound. DefaultToStaff sets UseSound to SoundID.Item43, but we want SoundID.Item20
            Item.UseSound = SoundID.Item20;

            // Set damage and knockBack
            Item.SetWeaponValues(40, 5);

            // Set rarity and value
            Item.SetShopValues(ItemRarityColor.Pink5, 10000);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity = Vector2.Zero;
            if (player == Main.LocalPlayer)
            {
                int WorldX;
                int WorldY;
                int PushUpY;
                Main.LocalPlayer.FindSentryRestingSpot(type, out WorldX, out WorldY, out PushUpY);

                Projectile mushroomLeft = Projectile.NewProjectileDirect(source, new Vector2(WorldX - 16, WorldY - PushUpY + 12), Vector2.Zero, type, (int)(damage/1.5), 0, player.whoAmI, -3);
                mushroomLeft.frame = Main.rand.Next(1, 3);
                mushroomLeft.rotation = Main.rand.NextFloat(-0.2f, 0f);
                mushroomLeft.timeLeft = 55;
                Projectile mushroomRight = Projectile.NewProjectileDirect(source, new Vector2(WorldX + 16, WorldY - PushUpY + 12), Vector2.Zero, type, (int)(damage / 1.5), 0, player.whoAmI, -8);
                mushroomRight.frame = Main.rand.Next(1, 3);
                mushroomRight.rotation = Main.rand.NextFloat(0f, 0.2f);
                mushroomRight.timeLeft = 50;
                Projectile mushroomCenter = Projectile.NewProjectileDirect(source, new Vector2(WorldX, WorldY - PushUpY + 12), Vector2.Zero, type, (int)(damage / 1.5), 0, player.whoAmI);
                mushroomCenter.frame = 0;
            }
            return false;
        }
    }
}
