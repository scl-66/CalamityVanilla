using CalamityVanilla.Common.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Crimson.Items.ParasiticBelt;

[AutoloadEquip(EquipType.Waist)]
public class ParasiticBelt : ModItem
{
    //public bool beltVisualHasSpawned = false;

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 26;
        Item.maxStack = 1;
        Item.value = Item.sellPrice(0, 5);
        Item.accessory = true;
        Item.rare = ItemRarityID.Pink;
        Item.expert = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.statLife < player.statLifeMax2 / 2)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.GetModPlayer<ParasiticBeltImbue>().beltIchorImbue = true;
            player.MeleeEnchantActive = true; // MeleeEnchantActive indicates to other mods that a weapon imbue is active.
            player.AddBuff(ModContent.BuffType<ParasiticBeltBuff>(), 5);

            if (player.GetModPlayer<ParasiticBeltImbue>().beltVisualHasSpawned == false) // spawn brain of confusion-styled visual indicator, dust and noise to indicate effect
            {
                SoundEngine.PlaySound(SoundID.Item74 with { Pitch = 0.5f, PitchVariance = 0.2f, Volume = 0.5f });
                SoundEngine.PlaySound(Main.rand.NextBool() == true ? SoundID.NPCDeath19 : SoundID.NPCDeath12);

                Projectile.NewProjectile(player.GetSource_Accessory(Item),
                    player.Center - new Vector2(Main.rand.NextFloat(-25f, 25f), player.height * 1.5f),
                    Vector2.Zero,
                    ModContent.ProjectileType<ParasiticBeltEffect>(),
                    0,
                    0f);
                player.GetModPlayer<ParasiticBeltImbue>().beltVisualHasSpawned = true;

                for (int i = 0; i < 30; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(5f, 5f);
                    Dust d = Dust.NewDustDirect(player.position, 30, 30, DustID.Ichor, speed.X + player.velocity.X, speed.Y + player.velocity.Y);
                    d.noGravity = false;
                    d.velocity *= 0.9f;
                    d.scale = 1.2f * Main.rand.NextFloat(0.5f, 1f);
                    //d.velocity.Y *= 1.1f;
                }
            }
        }
        else
        {
            player.GetModPlayer<ParasiticBeltImbue>().beltVisualHasSpawned = false;
        }
    }
}

public class ParasiticBeltEffect : ModProjectile
{
    public ref float Siner => ref Projectile.ai[1];
    public override void SetDefaults()
    {
        Projectile.arrow = true;
        Projectile.width = 22;
        Projectile.height = 22;
        Projectile.aiStyle = ProjAIStyleID.BrainofConfusion;
    }
    public override void AI()
    {
        Projectile.ai[1]++;
        Projectile.position.X += ((float)Math.Cos(Siner / 5f) * 2f / (Siner / 10f)) - 2f / (Siner / 5f);
        Projectile.rotation = (float)Math.Sin(Siner / 5f) * (1f - Siner / 60f);
    }
}

public class ParasiticBeltBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }
}