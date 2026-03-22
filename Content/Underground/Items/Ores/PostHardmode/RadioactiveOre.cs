using CalamityVanilla.Content.Buffs.Debuffs;
using CalamityVanilla.Content.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underground.Items.Ores.PostHardmode;

public class UraniumOre : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<UraniumOreTile>(), 0);
        Item.value = 2050;
        Item.rare = ModContent.RarityType<CobaltRarity>();
    }
}
public class PlutoniumOre : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<PlutoniumOreTile>(), 0);
        Item.value = 2200;
        Item.rare = ModContent.RarityType<CobaltRarity>();
    }
}

public class UraniumOreTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLighted[Type] = true;
        MinPick = 225;
        MineResist = 5f;

        TileID.Sets.Ore[Type] = true;
        TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
        Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
        Main.tileOreFinderPriority[Type] = 850; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
        Main.tileShine2[Type] = true; // Modifies the draw color slightly.
        Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently


        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(226, 246, 87), name);

        DustType = DustID.Chlorophyte;
        VanillaFallbackOnModDeletion = TileID.LunarOre;
        HitSound = SoundID.Tink;
    }

    public override void NearbyEffects(int i, int j, bool closer)
    {
        float dist = Vector2.Distance(Main.LocalPlayer.Hitbox.ClosestPointInRect(new Vector2(i * 16f + 8f, j * 16f + 8f)), new Vector2(i * 16f + 8f, j * 16f + 8f));
        if (dist < 70f)
        {
            Main.LocalPlayer.AddBuff(ModContent.BuffType<IrradiatedDebuff>(), 5);
            Main.LocalPlayer.GetModPlayer<IrradiatedRegen>().damage = (int)(1/(dist) * 380);
        }
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0.6f/3f;
        g = 0.85f/3f;
        b = 0.05f/3f;
    }
}

public class PlutoniumOreTile : UraniumOreTile {
    public new void SetStaticDefaults()
    {
        Main.tileOreFinderPriority[Type] = 860; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(232, 186, 86), name);

        DustType = DustID.Palladium;
    }
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0.9f / 3f;
        g = 0.6f / 3f;
        b = 0.3f / 3f;
    }
}
