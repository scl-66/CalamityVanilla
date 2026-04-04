using CalamityVanilla.Common;
using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Crimson.Items.IchorJavelin;

public class IchorJavelin : ModItem
{
    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }
    public override void SetDefaults()
    {
        Item.DefaultToThrownWeapon(ModContent.ProjectileType<IchorJavelinProjectile>(), 24, 18, true);
        Item.noUseGraphic = true;
        Item.damage = 30;
        Item.knockBack = 5;
        Item.rare = ItemRarityID.LightRed;
        Item.consumable = true;
        Item.maxStack = 9999;
        Item.UseSound = SoundID.Item1;
        Item.value = 12;
    }
    public override void AddRecipes()
    {
        CreateRecipe(66).AddTile(TileID.Anvils).AddIngredient(ItemID.Vertebrae, 1).AddIngredient(ItemID.Ichor, 1).Register();
    }
}

public class IchorJavelinProjectile : ModProjectile
{
    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }

    public int pierceAmt = 0;
    public int startDamage;
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 18);
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        Projectile.ai[0]++;

        if (Projectile.ai[0] > 25)
        {
            Projectile.velocity.Y += 0.8f;
            Projectile.velocity.X *= 0.97f;
        }

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        if (Projectile.velocity.Y > 16f)
        {
            Projectile.velocity.Y = 16f;
        }

        if (Main.rand.NextBool(2))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, 4, 4, DustID.Ichor);
            d.noGravity = true;
            d.velocity += Projectile.velocity;
            d.velocity *= 0.3f;
            //d.velocity.Y += 1.9f;
            d.scale = Main.rand.NextFloat(0.8f, 1.2f);
        }
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

        for (int i = 0; i < 15; i++)
        {
            Vector2 pos = Main.rand.NextVector2FromRectangle(new Rectangle(0, -6, 18, 58)).RotatedBy(Projectile.rotation) + Projectile.Center;
            Dust d = Dust.NewDustDirect(pos, 4, 4, DustID.Ichor);
            d.velocity = Projectile.velocity / 5f;
            d.scale = Main.rand.NextFloat(0.25f, 1f);
            //d.noGravity = !Main.rand.NextBool(3);
            d.noGravity = true;
        }

        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), DustID.Ichor, Main.rand.NextVector2Circular(1.5f, 1.5f));
            d.noGravity = true;
            d.velocity *= 0.95f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (pierceAmt < 1)
        {
            startDamage = Projectile.damage;
        }

        Projectile.damage = startDamage + (int)(Math.Sqrt(pierceAmt * 10f) * 3.5f);
        target.AddBuff(BuffID.Ichor, 60 + pierceAmt * (int)(150 / Math.Sqrt(pierceAmt + 1)));

        //if (target.CanBeChasedBy()) pierceAmt++;
        pierceAmt++;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return true;
    }
}