using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Drops.MyceliumStaff;

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

            Projectile mushroomLeft = Projectile.NewProjectileDirect(source, new Vector2(WorldX - 16, WorldY - PushUpY + 12), Vector2.Zero, type, (int)(damage / 1.5), 0, player.whoAmI, -3);
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

public class MyceliumShroom : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 38);
        Projectile.penetrate = 1;
        Projectile.timeLeft = 60 * 1;
        Projectile.frame = Main.rand.Next(0, 3);
        Projectile.spriteDirection = Main.rand.Next(0, 2);
    }

    public override void AI()
    {
        Projectile.Opacity = (float)Math.Clamp(Projectile.ai[0], 0, 20) / 20f;
        Projectile.ai[0]++;
    }

    public override void OnSpawn(IEntitySource source)
    {
        for (int i = 0; i < 4; i++)
        {
            Dust dust = Dust.NewDustPerfect(Projectile.Bottom + new Vector2(0, Main.rand.NextFloat(0f, 3f)), DustID.Corruption, new Vector2((float)Main.rand.NextFloat(-0.8f, 0.8f), (float)Main.rand.NextFloat(-0.6f, -0.3f)));
        }
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Top, new Vector2(Main.rand.NextFloat(-2f, 2f), -10f), ModContent.ProjectileType<MyceliumSpore>(), (int)(Projectile.damage * 1.5), Projectile.knockBack, Projectile.owner, Main.rand.Next(-15, 15), Projectile.position.Y);

        for (int i = 0; i < 5; i++)
        {
            Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Corruption);
        }
        for (int i = 0; i < Main.rand.Next(1, 2); i++)
        {
            float goreSize = Main.rand.NextFloat(0.65f, 1f);
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(0.3f, 0.3f), Mod.Find<ModGore>("MyceliumShroom" + $"{i}").Type, goreSize);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("CalamityVanilla/Content/Items/Weapons/Magic/MyceliumShroom").Value;
        float stretch = Projectile.ai[0] < 40 ? (float)(Math.Sin(Projectile.ai[0] * 6) / Math.Pow(Projectile.ai[0] / 8, 3) + 0.95f) : 1;

        Main.EntitySpriteDraw(
            texture,
            Projectile.Center + new Vector2(0, 19) - Main.screenPosition,
            new Rectangle(0, 42 * Projectile.frame, 38, 42),
            lightColor * Projectile.Opacity,
            Projectile.rotation,
            new Vector2(texture.Width / 2, texture.Height / 3),
            new Vector2(Math.Clamp(stretch, 0f, 3f), stretch),
            Projectile.spriteDirection == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
            );

        return false;
    }
}

public class MyceliumSpore : ModProjectile
{
    NPC target = null;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 18);
        Projectile.penetrate = 5;
        Projectile.timeLeft = 60 * 10;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        float creatorY = Projectile.ai[1];

        Projectile.tileCollide = Projectile.position.Y > creatorY ? true : false;

        target = CVUtils.FindClosestNPC(700f, Projectile.Center);

        if (Projectile.velocity.Y > 0f && target == null) Projectile.velocity.X = (float)Math.Sin(Projectile.ai[0] / 15) * 1f;
        else if (Projectile.velocity.Y > 0f && target != null)
        {
            if (Math.Abs(target.Center.X - Projectile.Center.X) > 2f)
            {
                if ((target.Center.X - Projectile.Center.X) / Math.Abs(target.Center.X - Projectile.Center.X) == Projectile.velocity.X / Math.Abs(Projectile.velocity.X))
                {
                    Projectile.velocity.X += (target.Center.X - Projectile.Center.X) / Math.Abs(target.Center.X - Projectile.Center.X) * 0.08f;
                }
                else Projectile.velocity.X += (target.Center.X - Projectile.Center.X) / Math.Abs(target.Center.X - Projectile.Center.X) * 0.2f;
            }
        }
        else Projectile.velocity.X *= 0.98f;
        Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y + 0.1f, -10f, 2f);
        Projectile.ai[0]++;

        Projectile.rotation = Projectile.velocity.X / 5f;

        if (++Projectile.frameCounter >= 8)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Corruption, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-0.2f, 0.2f));
            Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Cloud, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-0.2f, 0.2f));
        }
    }
}