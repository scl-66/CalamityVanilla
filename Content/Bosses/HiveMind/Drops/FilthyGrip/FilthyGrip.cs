using CalamityVanilla.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Drops.FilthyGrip;

[AutoloadEquip(EquipType.HandsOn)]
public class FilthyGrip : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 1;
        Item.value = 65000;
        Item.rare = ItemRarityID.Pink;
        Item.accessory = true;
        // Set other Item.X values here
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<FilthyGripCrit>().filthyGripEquipped = true;
    }
}

public class FilthyGripSlash : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 7;
    }
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 11;
        Projectile.friendly = true;
        Projectile.timeLeft = 21;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.ScalingArmorPenetration += 1f;
    }
    public override void AI()
    {
        if (++Projectile.frameCounter >= 3)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }
    }
}