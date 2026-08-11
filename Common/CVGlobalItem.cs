using CalamityVanilla.Content.Underworld.Items.WallOfFleshDrops;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CalamityVanilla.Common;

public class CVGlobalItem : GlobalItem
{
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
    {
        switch (item.type)
        {
            case ItemID.WallOfFleshBossBag:
                itemLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<FleshLauncher>(), ModContent.ItemType<PyrobatStaff>(), ModContent.ItemType<FighterJetRemote>()));
                break;
        }
    }
    public static Color Color1 => new Color(72, 139, 154);
    public static Color Color2 => new Color(169, 219, 233);
    public static float Lerp;
    public static float Style = 1;

    public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
    {
        //Replaces instances of ENNWAY! in tooltip lines with custom drawing to have fancy color swapping.
        if (line.Mod == "Terraria" && line.Name == "Tooltip0")
        {
            if (line.Text.Contains("ENNWAY!"))
            {
                if (Style == 1)
                {
                    Lerp += 0.010f;
                    if (Lerp >= 2)
                    {
                        Style = 2;
                    }
                }
                if (Style == 2)
                {
                    Lerp -= 0.012f;
                    if (Lerp <= 1.15)
                    {
                        Style = 1;
                    }
                }

                string text = line.Text.Replace("ENNWAY!", "");
                Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One);

                Utils.DrawBorderString(Main.spriteBatch, text, new Vector2(line.X, line.Y), Main.MouseTextColorReal, 1);
                Utils.DrawBorderString(Main.spriteBatch, "ENNWAY!", new Vector2(line.X + stringSize.X, line.Y), Color1 * Lerp, 1);
                return false;
            }
        }
        return true;
    }
}