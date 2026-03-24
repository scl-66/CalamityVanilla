using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underground.Items.Ores.PostHardmode.Uranium;

public class UraniumBar : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<UraniumBarTile>(), 0);
        Item.value = 8000;
        Item.rare = ModContent.RarityType<CobaltRarity>();
    }
    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, 0.60f / 3, 0.85f / 3, 0.05f / 3);
    }
    public override void Update(ref float gravity, ref float maxFallSpeed)
    {
        if (Main.rand.Next(0, 60) == 0)
        {
            float speed = 0.01f;
            Dust d = Dust.NewDustDirect(Item.position, Item.width, Item.height, ModContent.DustType<UraniumRadDust>(), Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed, speed));
        }
    }
}

public class UraniumBarTile : PostHardmodeBarTile
{
    private static float val;
    public override int dustType => ModContent.DustType<UraniumDust>();
    public override bool glows => true;
    public override int sparkleChance => 12000;
    public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        if (Main.rand.Next(0, 300) == 0)
        {
            float speed = 0.1f;
            Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<UraniumRadDust>(), Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed, speed));
            d.velocity *= 0.5f;
        }
    }
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        if (Main.rand.Next(0, 200) == 0)
        {
            val += Main.rand.NextFloat(-0.1f, 0.1f);
        }
        val = Math.Clamp(val, 0.75f, 1.25f);
        r = 0.60f / 3f * val;
        g = 0.85f / 3f * val;
        b = 0.05f / 3f * val;
    }
}
