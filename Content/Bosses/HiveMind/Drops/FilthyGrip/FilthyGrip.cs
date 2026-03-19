using CalamityVanilla.Common.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
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
    int frameSpeed = 2;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 8;
    }
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 30;
        Projectile.friendly = true;
        Projectile.timeLeft = 7*frameSpeed;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 7;
        Projectile.penetrate = 3;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        DrawOffsetX = 7;
        DrawOriginOffsetY = -10;
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.ScalingArmorPenetration += 1f;
    }
    public override void AI()
    {
        if (++Projectile.frameCounter >= frameSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }

        Projectile.ai[0]++;

        if (Projectile.ai[0] <= 1)
        {
            SoundEngine.PlaySound(SoundID.Item71 with
            {
                Volume = 0.4f,
                Pitch = Main.rand.NextFloat(0.7f, 1.1f)
            });
            Projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            //Projectile.rotation = 0f;
        }
    }
}