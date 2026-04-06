using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen.Drops.FrostGuardStaff;

public class FrostGuardStaff : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        ItemID.Sets.LockOnIgnoresCollision[Type] = true;

        ItemID.Sets.StaffMinionSlotsRequired[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.Size = new Vector2(24, 24);
        Item.damage = 10;
        Item.DamageType = DamageClass.Summon;
        Item.mana = 10;
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.noMelee = true;
        Item.knockBack = 2f;

        Item.shoot = ModContent.ProjectileType<FrostShieldCounter>();
        Item.buffType = ModContent.BuffType<FrostShieldBuff>();
        Item.shootSpeed = 10f;

        Item.UseSound = SoundID.Item66;
        Item.autoReuse = true;
        Item.reuseDelay = 2;

        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 2, 70, 0);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        position = Main.MouseWorld;
        player.LimitPointToPlayerReachableArea(ref position);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        player.AddBuff(Item.buffType, 2);

        return true;
    }
}

public class FrostShieldBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (player.ownedProjectileCounts[ModContent.ProjectileType<FrostShieldCounter>()] > 0)
        {
            player.buffTime[buffIndex] = 18000;
        }
        else
        {
            player.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}

public class FrostShieldCounter : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 6;

        ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        ProjectileID.Sets.MinionSacrificable[Type] = true;

        Main.projPet[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(10, 10);

        Projectile.timeLeft = 60;
        Projectile.minionSlots = 1f;
        Projectile.netImportant = true;

        Projectile.DamageType = DamageClass.Summon;
        Projectile.friendly = true;
        Projectile.penetrate = -1;

        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;

        Projectile.hide = true;
    }

    public override void AI()
    {
        
    }

    public override bool? CanDamage() => false;
    public override bool? CanCutTiles() => false;
}