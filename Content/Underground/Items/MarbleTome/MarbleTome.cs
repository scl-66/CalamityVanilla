using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underground.Items.MarbleTome;

public class MarbleTome : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 30;
        Item.damage = 4;
        Item.DamageType = DamageClass.Magic;
        Item.mana = 5;
        Item.useTime = 15;
        Item.useAnimation = 15;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 5f;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<MarbleRock>();
        Item.shootSpeed = 14f;
        Item.channel = true;

        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(0, 1);
    }
}

public class MarbleRock : ModProjectile
{
    private ref float FrameCount => ref Projectile.ai[0];

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 34;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.ignoreWater = true;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        Projectile.rotation += Projectile.velocity.X * 0.05f;

        FrameCount++;
        if (FrameCount > 15)
        {
            Projectile.velocity.X *= 0.97f;
            Projectile.velocity.Y += 0.2f;
        }


        if (Projectile.velocity.Y > 16f)
            Projectile.velocity.Y = 16f;
    }
}