using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Rarities;
using CalamityVanilla.Content.Underground.Items.Ores.PostHardmode.Uranium;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underground.Items.Ores.PostHardmode.Plutonium;

public class PlutoniumOre : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<PlutoniumOreTile>(), 0);
        Item.value = 2200;
        Item.rare = ModContent.RarityType<CobaltRarity>();
    }
    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, 0.9f / 3, 0.6f / 3, 0.3f / 3);
    }
    public override void Update(ref float gravity, ref float maxFallSpeed)
    {
        if (Main.rand.Next(0, 60) == 0)
        {
            float speed = 0.01f;
            Dust d = Dust.NewDustDirect(Item.position, Item.width, Item.height, ModContent.DustType<PlutoniumRadDust>(), Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed, speed));
        }
    }
}

public class PlutoniumOreTile : UraniumOreTile {
    private static float val;
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
        Main.tileOreFinderPriority[Type] = 860; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
        Main.tileShine2[Type] = true; // Modifies the draw color slightly.
        Main.tileShine[Type] = 10000; // How often tiny dust appear off this tile. Larger is less frequently
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(232, 186, 86), name);

        DustType = ModContent.DustType<PlutoniumDust>();
        VanillaFallbackOnModDeletion = TileID.LunarOre;
        HitSound = SoundID.Tink;
    }
    public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        if (Main.rand.Next(0, 500) == 0)
        {
            float speed = 0f;
            Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<PlutoniumRadDust>(), Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed, speed), 1, Color.Orange);
            d.velocity *= 0.5f;
        }
    }
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        if (Main.rand.Next(0, 300) == 0)
        {
            val += Main.rand.NextFloat(-0.1f, 0.1f);
        }
        val = Math.Clamp(val, 0.75f, 1.25f);
        r = 0.9f / 3f * val;
        g = 0.6f / 3f * val;
        b = 0.3f / 3f * val;
    }
}
