using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Crimson.Items.TapewormWhip;

internal class TapewormWhipGiblet1 : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.IgnoresEncumberingStone[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 1;
    }

    public override bool ItemSpace(Player player) => true;

    public override bool OnPickup(Player player)
    {
        SoundEngine.PlaySound(SoundID.Item2);
        if (player.HasBuff(ModContent.BuffType<MinorBottomFeederBuff>()))
        {
            player.buffTime[player.FindBuffIndex(ModContent.BuffType<MinorBottomFeederBuff>())] += 300;
        }
        else
        {
            player.AddBuff(ModContent.BuffType<MinorBottomFeederBuff>(), 300);
        }
        return false;
    }
}

internal class TapewormWhipGiblet2 : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.IgnoresEncumberingStone[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 1;
    }

    public override bool ItemSpace(Player player) => true;

    public override bool OnPickup(Player player)
    {
        SoundEngine.PlaySound(SoundID.Item2);
        if (player.HasBuff(ModContent.BuffType<MediumBottomFeederBuff>()))
        {
            player.buffTime[player.FindBuffIndex(ModContent.BuffType<MediumBottomFeederBuff>())] += 300;
        }
        else
        {
            player.AddBuff(ModContent.BuffType<MediumBottomFeederBuff>(), 300);
        }
        return false;
    }
}

internal class TapewormWhipGiblet3 : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.IgnoresEncumberingStone[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 1;
    }

    public override bool ItemSpace(Player player) => true;

    public override bool OnPickup(Player player)
    {
        SoundEngine.PlaySound(SoundID.Item2);
        if (player.HasBuff(ModContent.BuffType<MajorBottomFeederBuff>()))
        {
            player.buffTime[player.FindBuffIndex(ModContent.BuffType<MajorBottomFeederBuff>())] += 300;
        }
        else
        {
            player.AddBuff(ModContent.BuffType<MajorBottomFeederBuff>(), 300);
        }
        return false;
    }
}

internal sealed class MinorBottomFeederBuff : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        player.statDefense += 1;
        player.moveSpeed += 0.05f;
        player.lifeRegen += 1;
        player.pickSpeed -= 0.05f;
        player.GetDamage(DamageClass.Generic) += 0.05f;
        player.GetCritChance(DamageClass.Generic) += 2;
    }
}

internal sealed class MediumBottomFeederBuff : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        player.statDefense += 2;
        player.moveSpeed += 0.15f;
        player.lifeRegen += 2;
        player.pickSpeed -= 0.15f;
        player.GetDamage(DamageClass.Generic) += 0.05f;
        player.GetCritChance(DamageClass.Generic) += 3;
    }
}

internal sealed class MajorBottomFeederBuff : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        player.statDefense += 4;
        player.moveSpeed += 0.25f;
        player.lifeRegen += 3;
        player.pickSpeed -= 0.25f;
        player.GetDamage(DamageClass.Generic) += 0.15f;
        player.GetCritChance(DamageClass.Generic) += 5;
    }
}