using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.BouncingEyeball;

public class BouncingEyeball : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }
    public override void SetDefaults()
    {
        Item.DefaultToThrownWeapon(ModContent.ProjectileType<BouncingEyeballCluster>(), 30, 13, true);
        Item.noUseGraphic = true;
        Item.damage = 16;
        Item.knockBack = 2;
        Item.rare = ItemRarityID.White;
        Item.consumable = true;
        Item.maxStack = 9999;
        Item.UseSound = SoundID.Item1;
        Item.value = 15;
    }
}

public class BouncingEyeballCluster : ModProjectile
{
    public override string Texture => "CalamityVanilla/Content/Miscellaneous/Items/Weapons/Ranger/BouncingEyeball/BouncingEyeball";
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 20);
        Projectile.timeLeft = 4 * 60;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.rotation = MathHelper.ToRadians(Main.rand.Next(360));
    }

    public override void AI()
    {
        Projectile.velocity.Y += 0.5f;
        Projectile.velocity.X *= 0.99f;

        Projectile.rotation += Projectile.velocity.X / 60f;
        Projectile.rotation += Projectile.velocity.Length() * Projectile.direction / 55f;
        //Projectile.rotation += (Projectile.velocity.X + Math.Abs(Projectile.velocity.Y) * Projectile.direction) / 60f;

        if (Main.rand.NextBool(5))
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<EyeballBloodDust>());
            d.velocity = Main.rand.NextVector2Circular(2f, 2f);
        }

        if (Projectile.velocity.Y > 16f)
        {
            Projectile.velocity.Y = 16f;
        }
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath11, Projectile.position);

        int goreMax = Main.rand.Next(3) + 1;
        for (int i = 0; i < goreMax; i++)
        {
            int g = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(6, 6), Mod.Find<ModGore>("BouncingEyeball" + $"{i + 1}").Type);
            Main.gore[g].timeLeft = 2;
        }

        for (int i = 0; i < 20; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<EyeballBloodDust>());
            d.velocity = Main.rand.NextVector2Circular(6, 6);
            d.scale = Main.rand.NextFloat(1, 2);
            //d.noGravity = !Main.rand.NextBool(3);
        }

        int eyeCount = Main.rand.Next(2, 5);
        for (int j = 0; j < eyeCount; j++)
        {
            float rand = Main.rand.NextFloat(MathHelper.TwoPi);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.velocity / 1.12f).RotatedBy(rand), ModContent.ProjectileType<BouncingEyeProj>(), 10, 0, ai1: j % 3);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.penetrate = 0;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        //SoundEngine.PlaySound(SoundID.NPCHit19, Projectile.position);
        Projectile.penetrate = 0;
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = Projectile.oldVelocity.Y * -0.92f;
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = Projectile.oldVelocity.X * -0.7f;
        return false;
    }
}

public class BouncingEyeProj : ModProjectile
{
    public int penetrateTime = 8;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 16);
        Projectile.timeLeft = 60 * 5;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.friendly = true;
        Projectile.rotation = MathHelper.ToRadians(Main.rand.Next(360));
        Projectile.penetrate = -1;
    }

    public ref float EyeSize => ref Projectile.ai[1];

    public override void AI()
    {

        //Main.NewText(Projectile.timeLeft);

        int widthAndHeight;

        switch (EyeSize)
        {
            case 0: widthAndHeight = 16; Projectile.frame = 0; break;
            case 1: widthAndHeight = 12; Projectile.frame = 1; break;
            default: case 2: widthAndHeight = 8; Projectile.frame = 2; break;
        }

        Projectile.width = widthAndHeight;
        Projectile.height = widthAndHeight;

        Projectile.velocity.Y += 0.5f;
        Projectile.velocity.X *= 0.985f;

        if (Projectile.velocity.Y > 12f)
        {
            Projectile.velocity.Y = 12f;
        }

        Projectile.rotation += Projectile.velocity.X / 45f;
        //Projectile.rotation += (Projectile.velocity.X + Math.Abs(Projectile.velocity.Y) * Projectile.direction) / 60f;

        Projectile.ai[0]++;

        if (Projectile.ai[0] < penetrateTime)
        {
            Projectile.damage = 0;
            Projectile.penetrate = -1;
        }
        else if (Projectile.ai[0] == penetrateTime)
        {
            Projectile.damage = 20;
            Projectile.penetrate = 2;
        }

        if (Main.rand.NextBool(5))
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<EyeballBloodDust>());
            d.velocity = Main.rand.NextVector2Circular(1f, 1f);
            d.scale = 0.75f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Projectile.ai[0] > penetrateTime)
        {
            Projectile.penetrate = 0;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.penetrate--;
        SoundEngine.PlaySound(SoundID.NPCHit1 with { Volume = 0.5f, Pitch = 0.1f, PitchVariance = 0.3f }, Projectile.position);
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = Projectile.oldVelocity.Y * -0.8f;
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = Projectile.oldVelocity.X * -0.95f;
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = 0.5f, Pitch = 0.1f }, Projectile.position);

        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<EyeballBloodDust>());
            d.velocity = Main.rand.NextVector2Circular(2f, 2f);
            d.scale = Main.rand.NextFloat(0.5f, 1.5f);
            //d.noGravity = !Main.rand.NextBool(3);
        }
    }
}