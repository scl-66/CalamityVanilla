using CalamityVanilla.Common.Dusts;
using CalamityVanilla.Common.Particles;
using CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.TheGothic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underworld.Items.FlareBolt;

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
        Item.shootSpeed = 8f;
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
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 35;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = true;
        Projectile.timeLeft = 120;
        Projectile.extraUpdates = 14;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.ignoreWater = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
        {
            float percent = i / (float)Projectile.oldPos.Length;
            Color c = Color.Lerp(new Color(1f, 1f, 1f, 0.4f), new Color(0.5f, 0f, 0f, 0f), Math.Clamp(percent * 3, 0, 1));
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, null, c, Projectile.oldRot[i], new Vector2(7, 14), Projectile.scale * (1f - (percent * 0.75f)), SpriteEffects.None);
        }
        return false;
    }
    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        if (Projectile.timeLeft > 110)
            return;
        if (Main.rand.NextBool(4))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            d.velocity += Projectile.velocity * 0.25f;
            if (!Main.rand.NextBool(3))
            {
                d.scale = Main.rand.NextFloat(1, 2);
                d.noGravity = true;
            }
        }
        if (Main.rand.NextBool(8))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            d.velocity += Projectile.velocity * 0.25f;
            d.noGravity = true;
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

        for (int i = 0; i < 2; i++)
        {
            Gore g = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2Circular(2, 1), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
            g.velocity += Projectile.velocity * 0.25f;
            g.scale = Main.rand.NextFloat(0.3f, 0.75f);
        }

        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(3, 3));
            d.scale += Main.rand.NextFloat();
            if (Main.rand.NextBool())
            {
                d.velocity *= 3;
                d.noGravity = true;
                d.fadeIn = Main.rand.NextFloat(2);
            }
            else
            {
                d.customData = 1;
            }
            d.velocity += Projectile.velocity * 0.25f;
        }

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        int type = ModContent.DustType<SimpleColorableGlowyDust>();
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            float percent = i / (float)Projectile.oldPos.Length;
            Dust d = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width, Projectile.height, type);
            d.color = Color.Lerp(new Color(1f, 1f, 0f, 0f), new Color(0.5f, 0f, 0f, 0f), Math.Clamp(percent * 2, 0, 1));
            d.noLight = true;
            //d.velocity = Projectile.oldPos[i].DirectionTo(Projectile.oldPos[i - 1]) * Projectile.oldPos[i].Distance(Projectile.oldPos[i - 1]);
            d.scale *= 1.3f;
            d.noGravity = true;
        }
        for (int i = 0; i < 7; i++)
        {
            Gore g = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2Circular(3, 2), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
            g.velocity += Projectile.velocity * 0.25f;
            g.scale = Main.rand.NextFloat(0.5f, 1f);
        }

        for (int i = 0; i < 25; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(3, 3));
            d.scale += Main.rand.NextFloat();
            if (Main.rand.NextBool())
            {
                d.velocity *= 3;
                d.noGravity = true;
                d.fadeIn = Main.rand.NextFloat(2);
            }
            else
            {
                d.customData = 1;
            }
            d.velocity += Projectile.velocity * 0.25f;
        }

        var p = AnimatedParticle.RequestAnimatedParticle();
        p.SetTypeInfo(5, Main.rand.Next(10, 15), TextureAssets.Projectile[ProjectileID.Volcano], Color.White);
        p.LocalPosition = Projectile.Center;
        p.ScaleVelocity = Vector2.One * Main.rand.NextFloat(-0.01f, 0.01f);
        p.Scale = Vector2.One * Main.rand.NextFloat(1f, 1.3f);
        Main.ParticleSystem_World_OverPlayers.Add(p);

        SoundEngine.PlaySound(SoundID.Item62 with
        {
            PitchVariance = 0.2f,

            MaxInstances = 10,
        }, Projectile.Center);
    }
}