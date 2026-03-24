using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Rarities;
using CalamityVanilla.Content.Underground.Items.Ores.PostHardmode.Uranium;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underground.Items.Ores.PostHardmode.Plutonium;

public class PlutoniumBar : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<PlutoniumBarTile>(), 0);
        Item.value = 8800;
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

public class PlutoniumBarTile : PostHardmodeBarTile
{
    private static float val;
    public override int dustType => ModContent.DustType<PlutoniumDust>();
    public override bool glows => true;
    public override int sparkleChance => 12000;
    public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        if (Main.rand.Next(0, 300) == 0)
        {
            float speed = 0f;
            Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<PlutoniumRadDust>(), Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed, speed), 1, Color.Orange);
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
        r = 0.9f / 3f * val;
        g = 0.6f / 3f * val;
        b = 0.3f / 3f * val;
    }
}
