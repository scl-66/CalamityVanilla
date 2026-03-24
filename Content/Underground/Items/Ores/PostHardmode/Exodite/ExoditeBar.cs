using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Rarities;
using Daybreak.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underground.Items.Ores.PostHardmode.Exodite;

public class ExoditeBar : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<ExoditeBarTile>(), 0);
        Item.value = 8200;
        Item.rare = ModContent.RarityType<CobaltRarity>();
    }
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Main.GetItemDrawFrame(Item.type, out var itemTexture, out var itemFrame);
        Vector2 drawOrigin = itemFrame.Size() / 2f;
        Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, drawOrigin.Y);

        float counter = Item.timeSinceItemSpawned / 240f + Main.GlobalTimeWrappedHourly * 0.04f;
        float offsetScale = Main.GlobalTimeWrappedHourly * 2;
        offsetScale %= 4f;
        offsetScale /= 2f;
        if (offsetScale >= 1f)
        {
            offsetScale = 2f - offsetScale;
        }

        offsetScale = offsetScale * 0.5f + 0.5f;
        for (float i = 0f; i < 1f; i += 0.34f)
        {
            spriteBatch.Draw
            (
                itemTexture, drawPosition + new Vector2(0f, 3f).RotatedBy(-(i + counter) * ((float)Math.PI * 2f)) * offsetScale,
                itemFrame, new Color(0, 0, 0, 0.25f), rotation, drawOrigin, scale, SpriteEffects.None, 0f
            );
            spriteBatch.Draw
            (
                itemTexture, drawPosition + new Vector2(0f, 2.5f).RotatedBy((i + counter) * ((float)Math.PI * 2f)) * offsetScale,
                itemFrame, new Color(0, 0, 0, 0.5f), rotation, drawOrigin, scale, SpriteEffects.None, 0f
            );
        }
        return true;
    }
}

public class ExoditeBarTile : PostHardmodeBarTile
{
    public override int dustType => ModContent.DustType<ExoditeDust>();
    public override bool glows => false;
    public override int sparkleChance => 6000;

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Framing.GetTileSafely(i, j);
        int frameX = tile.TileFrameX;
        int frameY = tile.TileFrameY;
        Rectangle frame = new(frameX, frameY, 16, 16);

        Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

        float counter = Main.GlobalTimeWrappedHourly * 0.1f;
        float offsetScale = Main.GlobalTimeWrappedHourly * 2;
        offsetScale %= 4f;
        offsetScale /= 2f;
        if (offsetScale >= 1f)
        {
            offsetScale = 2f - offsetScale;
        }

        offsetScale = offsetScale * 0.5f + 0.5f;

        for (float k = 0; k < 1f; k += 0.34f)
        {
            spriteBatch.Draw
            (
                tex, new Vector2(i, j).ToWorldCoordinates(0, 0) - Main.screenPosition + drawOffset + new Vector2(0f, 3f).RotatedBy(-(k + counter) * ((float)Math.PI * 2f)) * offsetScale,
                frame, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.25f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
            );
            spriteBatch.Draw
            (
                tex, new Vector2(i, j).ToWorldCoordinates(0, 0) - Main.screenPosition + drawOffset + new Vector2(0f, 3.5f).RotatedBy((k + counter) * ((float)Math.PI * 2f)) * offsetScale,
                frame, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.025f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
            );
        }
        spriteBatch.End(out var sbSnapshot);
        spriteBatch.Begin(sbSnapshot);
        return true;
    }
}
