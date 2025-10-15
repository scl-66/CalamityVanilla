using CalamityVanilla.Content.Bosses.Cryogen;
using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Items.Icebreaker;

public class Icebreaker : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToSword(80, 8, 5);
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 3, 0, 0);
        Item.shoot = ModContent.ProjectileType<IcebreakerProjectile>();
        Item.shootSpeed = 11;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }
    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] < 1;
    }
}

public class IcebreakerProjectile : ModProjectile
{
    public override string Texture => ModContent.GetModItem(ModContent.ItemType<Icebreaker>()).Texture;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        for (int i = 0; i < 8; i++)
        {
            float percent = 1f - (i / 8f);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Cryogen.GetAuroraColor((int)Main.timeForVisualEffects + (i * 15)) with { A = 0 } * percent * 0.5f, Projectile.oldRot[i], new Vector2(21, 17), Projectile.scale *  (0.8f + (percent * 0.3f)), SpriteEffects.None);
        }
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value,Projectile.Center - Main.screenPosition,null,lightColor,Projectile.rotation,new Vector2(21,17),Projectile.scale,SpriteEffects.None);
        return false;
    }
    public override void AI()
    {
        if (Main.rand.NextBool(3))
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2), ModContent.DustType<SimpleColorableGlowyDust>(), Projectile.velocity.RotatedByRandom(0.1f));
            d.color = Cryogen.GetAuroraColor((int)Main.timeForVisualEffects) with { A = 0 };
            d.noGravity = true;
        }
        if (Main.rand.NextBool())
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Snow, Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(1f, 4f));
            d.noGravity = true;
        }
    }
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.ThornChakram);
        Projectile.extraUpdates = 1;
        Projectile.penetrate = 100;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.tileCollide = false;
    }
    private void OnHitAnything()
    {
        SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
        if (Main.myPlayer == Projectile.owner)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 rotation = Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 6 * i).RotatedByRandom(0.2f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + rotation * 24, rotation * Main.rand.NextFloat(4, 5), ModContent.ProjectileType<IcebreakerIcicles>(), Projectile.damage / 4, 2, Projectile.owner);
            }
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Projectile.penetrate != 100)
            return;
        OnHitAnything();
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        if (Projectile.penetrate != 100)
            return;
        OnHitAnything();
    }
}

public class IcebreakerIcicles : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults();
        Projectile.penetrate = -1;
        Projectile.timeLeft = 30;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.DamageType = DamageClass.Melee;
    }
    public override void AI()
    {
        if(Projectile.alpha == 0)
        {
            for(int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SimpleColorableGlowyDust>(), Projectile.velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(1f,2f));
                d.color = Cryogen.GetAuroraColor((int)Main.timeForVisualEffects) with { A = 0 };
                d.noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Snow, Projectile.velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(1f, 4f));
                d.noGravity = true;
            }
        }
        Projectile.frame = Projectile.whoAmI % 3;
        Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        Projectile.alpha += 255 / 30;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Projectile.velocity *= 0.95f;
    }
}