using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Underworld.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Items.Weapons.Magic;

public class FlareBolt : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 30;
        Item.damage = 65;
        Item.DamageType = DamageClass.Magic;
        Item.mana = 35;
        Item.useTime = 35;
        Item.useAnimation = 35;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 3.5f;
        Item.UseSound = SoundID.Item73 with
        {
            Pitch = 0.1f,
            PitchVariance = 0.1f,
            MaxInstances = 0,
        };
        Item.autoReuse = true;
        Item.shootSpeed = 4f;
        Item.shoot = ModContent.ProjectileType<FlareBoltProjectile>();

        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(0, 4);
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.Bookcases)
            .AddIngredient(ItemID.SpellTome, 1)
            .AddIngredient(ItemID.LivingFireBlock, 20)
            .AddIngredient<HavocSoul>(15)
            .Register();
    }
}


public class FlareBoltProjectile : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = true;
        Projectile.timeLeft = 240;
        Projectile.extraUpdates = 29;
        Projectile.alpha = 255;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.ignoreWater = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        if (Projectile.timeLeft > 230)
            return;
        for (int i = 0; i < 4; i++)
        {
            Dust dust = Dust.NewDustDirect(
                Projectile.position,
                Projectile.width, Projectile.height,
                ModContent.DustType<MagicFireDust>(),
                0, 0,
                0,
                default,
                Main.rand.NextFloat(0.6f, 1.2f));
            dust.velocity = dust.velocity * 0.2f + Projectile.velocity.RotatedByRandom(0.5) * Main.rand.NextFloat(0.1f, 0.3f);
            dust.noGravity = true;
        }
        if (Main.rand.NextBool(4))
        {
            Dust dust = Dust.NewDustPerfect(
                Projectile.Center,
                DustID.InfernoFork,
                -Projectile.velocity.RotatedByRandom(0.2) * 0.5f,
                0,
                Color.White,
                Main.rand.NextFloat(1.25f, 1.75f)
            );
            dust.noGravity = true;
        }

        if (Projectile.wet && !Projectile.lavaWet)
        {
            Projectile.Kill();
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 360);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.OnFire3, 360, false, false);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
        {
            Projectile.velocity.X = -oldVelocity.X;
        }
        if (Projectile.velocity.Y != oldVelocity.Y)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
        }
        SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item88 with
        {
            Pitch = 0.5f,
            PitchVariance = 0.1f,

            MaxInstances = 0,
        }, Projectile.Center);
        for (int k = 0; k < 64; k++)
        {
            Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 6f);
            Dust dust = Dust.NewDustPerfect(
                Projectile.Center,
                ModContent.DustType<MagicFireDust>(),
                velocity,
                0,
                Color.White,
                Main.rand.NextFloat(0.6f, 1.2f)
            );
        }
        for (int k = 0; k < 32; k++)
        {
            Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 8f);
            Dust dust = Dust.NewDustPerfect(
                Projectile.Center,
                DustID.InfernoFork,
                velocity,
                0,
                Color.White,
                Main.rand.NextFloat(1.25f, 1.75f)
            );
            dust.noGravity = true;
        }
    }
}