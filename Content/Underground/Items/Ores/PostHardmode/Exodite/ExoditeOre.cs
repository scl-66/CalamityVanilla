using CalamityVanilla.Content.Buffs.Debuffs;
using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Particles;
using CalamityVanilla.Content.Rarities;
using CalamityVanilla.Content.Underground.Items.Ores.PostHardmode.Plutonium;
using Daybreak.Common.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underground.Items.Ores.PostHardmode.Exodite;

public class ExoditeOre : ModItem
{
    private float auraTimer = 0;
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<ExoditeOreTile>(), 0);
        Item.value = 2400;
        Item.rare = ModContent.RarityType<CobaltRarity>();
    }

    public override void Update(ref float gravity, ref float maxFallSpeed)
    {
        auraTimer += 0.025f;
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
        // afterimages
        for (float i = 0f; i < 1f; i += 0.34f)
        {
            spriteBatch.Draw
            (
                itemTexture, drawPosition + new Vector2(0f, 4f).RotatedBy(-(i + counter) * ((float)Math.PI * 2f)) * offsetScale,
                itemFrame, new Color(0, 0, 0, 0.2f), rotation, drawOrigin, scale, SpriteEffects.None, 0f
            );
            spriteBatch.Draw
            (
                itemTexture, drawPosition + new Vector2(0f, 3f).RotatedBy((i + counter) * ((float)Math.PI * 2f)) * offsetScale,
                itemFrame, new Color(0, 0, 0, 0.3f), rotation, drawOrigin, scale, SpriteEffects.None, 0f
            );
        }
        return true;
    }
}

public class ExoditeOreTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLighted[Type] = false;
        MinPick = 225;
        MineResist = 5f;

        TileID.Sets.Ore[Type] = true;
        TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
        Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
        Main.tileOreFinderPriority[Type] = 870; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
        Main.tileShine2[Type] = false; // Modifies the draw color slightly.
        //Main.tileShine[Type] = 6000; // How often tiny dust appear off this tile. Larger is less frequently


        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(16, 9, 76), name);

        DustType = ModContent.DustType<ExoditeDust>();
        VanillaFallbackOnModDeletion = TileID.LunarOre;
        HitSound = SoundID.Tink;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) // slope drawcode adapted from spiritmod/tiles/gtile
    {
        Tile tile = Framing.GetTileSafely(i, j);
        int frameX = tile.TileFrameX;
        int frameY = tile.TileFrameY;
        Rectangle frame = new(frameX, frameY, 16, 16);

        Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 offsetZero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

        Vector2 location = new Vector2(i, j).ToWorldCoordinates(0, 0);
        Vector2 offsets = -Main.screenPosition + offsetZero;
        Vector2 drawCoords = location + offsets;


        float counter = Main.GlobalTimeWrappedHourly * 0.1f;
        float offsetScale = Main.GlobalTimeWrappedHourly * 2;
        offsetScale %= 4f;
        offsetScale /= 2f;
        if (offsetScale >= 1f)
        {   
            offsetScale = 2f - offsetScale;
        }

        offsetScale = offsetScale * 1f + 0.5f;
        float piScale = (float)Math.PI * 2f;

        if ((tile.Slope == 0 && !tile.IsHalfBlock) || (Main.tileSolid[tile.TileType] && Main.tileSolidTop[tile.TileType]))
        {
            for (float k = 0; k < 1f; k += 0.34f)
            {
                spriteBatch.Draw
                (
                    tex, drawCoords + new Vector2(0f, 4f).RotatedBy(-(k + counter) * piScale) * offsetScale,
                    frame, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.25f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                );
                spriteBatch.Draw
                (
                    tex, drawCoords + new Vector2(0f, 3f).RotatedBy((k + counter) * piScale) * offsetScale,
                    frame, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.5f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                );
            }
        }
        else if (tile.IsHalfBlock)
        {
            for (float k = 0; k < 1f; k += 0.34f)
            {
                spriteBatch.Draw
                (
                    tex, new Vector2(drawCoords.X, drawCoords.Y + 8) + new Vector2(0f, 4f).RotatedBy(-(k + counter) * piScale) * offsetScale,
                    new Rectangle(frameX, frameY, 16, 8), Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.25f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                );
                spriteBatch.Draw
                (
                    tex, new Vector2(drawCoords.X, drawCoords.Y + 8) + new Vector2(0f, 3f).RotatedBy((k + counter) * piScale) * offsetScale,
                    new Rectangle(frameX, frameY, 16, 8), Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.5f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                );
            }
        }
        // slope drawing
        else
        {
            byte b = (byte)tile.Slope;
            Rectangle frameSlope;
            Vector2 drawPos;

            if (b == 1 || b == 2)
            {
                int length;
                int height2;

                for (int a = 0; a < 8; ++a)
                {
                    if (b == 2)
                    {
                        length = 16 - a * 2 - 2;
                        height2 = 14 - a * 2;
                    }
                    else
                    {
                        length = a * 2;
                        height2 = 14 - length;
                    }

                    frameSlope = new Rectangle(frameX + length, frameY, 2, height2);
                    drawPos = new Vector2(i * 16 + length, j * 16 + a * 2) + offsets;

                    for (float k = 0; k < 1f; k += 0.34f)
                    {
                        Main.spriteBatch.Draw
                        (
                            tex, drawPos + new Vector2(0f, 4f).RotatedBy(-(k + counter) * piScale) * offsetScale,
                            frameSlope, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.25f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                        );
                        Main.spriteBatch.Draw
                        (
                            tex, drawPos + new Vector2(0f, 3f).RotatedBy((k + counter) * piScale) * offsetScale,
                            frameSlope, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.5f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                        );
                    }
                }

                frameSlope = new Rectangle(frameX, frameY + 14, 16, 2);
                drawPos = new Vector2(i * 16, j * 16 + 14) + offsets;
                for (float k = 0; k < 1f; k += 0.34f)
                {
                    Main.spriteBatch.Draw
                    (
                        tex, drawPos + new Vector2(0f, 4f).RotatedBy(-(k + counter) * piScale) * offsetScale,
                        frameSlope, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.25f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                    );
                    Main.spriteBatch.Draw
                    (
                        tex, drawPos + new Vector2(0f, 3f).RotatedBy((k + counter) * piScale) * offsetScale,
                        frameSlope, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.5f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                    );
                }
            }
            else
            {
                int length;
                int height2;

                for (int a = 0; a < 8; ++a)
                {
                    if (b == 3)
                    {
                        length = a * 2;
                        height2 = 16 - length;
                    }
                    else
                    {
                        length = 16 - a * 2 - 2;
                        height2 = 16 - a * 2;
                    }

                    frameSlope = new Rectangle(frameX + length, frameY + 16 - height2, 2, height2);
                    drawPos = new Vector2(i * 16 + length, j * 16) + offsets;
                    for (float k = 0; k < 1f; k += 0.34f)
                    {
                        Main.spriteBatch.Draw
                        (
                            tex, drawPos + new Vector2(0f, 4f).RotatedBy(-(k + counter) * piScale) * offsetScale,
                            frameSlope, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.25f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                        );
                        Main.spriteBatch.Draw
                        (
                            tex, drawPos + new Vector2(0f, 3f).RotatedBy((k + counter) * piScale) * offsetScale,
                            frameSlope, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.5f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                        );
                    }
                }

                drawPos = new Vector2(i * 16, j * 16) + offsets;
                frameSlope = new Rectangle(frameX, frameY, 16, 2);
                for (float k = 0; k < 1f; k += 0.34f)
                {
                    Main.spriteBatch.Draw
                    (
                        tex, drawPos + new Vector2(0f, 4f).RotatedBy(-(k + counter) * piScale) * offsetScale,
                        frameSlope, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.25f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                    );
                    Main.spriteBatch.Draw
                    (
                        tex, drawPos + new Vector2(0f, 3f).RotatedBy((k + counter) * piScale) * offsetScale,
                        frameSlope, Lighting.GetColor(i, j).MultiplyRGBA(new Color(0, 0, 0, 0.5f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
                    );
                }
            }
        }
        spriteBatch.End(out var sbSnapshot);
        spriteBatch.Begin(sbSnapshot);
        return true;
    }

    public override bool CanExplode(int i, int j)
    {
        return false;
    }
}