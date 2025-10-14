using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Items.Weapons.Melee;

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
    public override string Texture => "CalamityVanilla/Content/Items/Weapons/Melee/Icebreaker";
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
        Projectile.frame = Projectile.whoAmI % 3;
        Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        Projectile.alpha += 255 / 30;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Projectile.velocity *= 0.95f;
    }
}