using Daybreak.Common.Features.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CalamityVanilla.Common.Rarities;

internal sealed class BlackAndGoldRarity : ModRarity, IRarityTextRenderer
{
    public override Color RarityColor => new Color(0, 0, 0);

    public void RenderText(SpriteBatch sb, DynamicSpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, RarityDrawContext drawContext, float maxWidth = -1, float spread = 2)
    {
        float outlineOffset = 1f;

        var offsets = new Vector2[] {
            new Vector2(-outlineOffset, 0),
            new Vector2(outlineOffset, 0),
            new Vector2(0, -outlineOffset),
            new Vector2(0, outlineOffset),
            new Vector2(-outlineOffset, -outlineOffset),
            new Vector2(outlineOffset, -outlineOffset),
            new Vector2(-outlineOffset, outlineOffset),
            new Vector2(outlineOffset, outlineOffset)
        };

        var gold = new Color(225, 221, 82);

        var size = ChatManager.GetStringSize(font, text, scale);

        foreach (var offset in offsets)
        {
            ChatManager.DrawColorCodedString(sb, font, text, position + offset + size / 2, gold, 0f, size / 2, scale);
        }

        ChatManager.DrawColorCodedString(sb, font, text, position + size / 2, Color.Black, 0f, size / 2, scale);
    }
}