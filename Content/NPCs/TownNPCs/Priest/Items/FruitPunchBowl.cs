using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Items;


public class FruitPunchBowlTile : ModTile
{
    public static string[] potionNameContainsList = { "Bottled", "Potion", "Juice", "Brew", "Flask" };
    public static string[] potionNameBlacklist = { "Statue", "Decorative" };
    public Player player;
    public bool heldItemcontainsPotionName;
    public int potionType;
    public bool filled;

    public override void SetStaticDefaults()
    {
        HitSound = SoundID.Shatter;
        DustType = DustID.Glass;
        AddMapEntry(new Color(219, 226, 225), Language.GetText("Mods.CalamityVanilla.Items.FruitPunchBowl.DisplayName"));
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table, TileObjectData.Style2x1.Width, 0);
        TileObjectData.addTile(Type);
    }

    public override void MouseOver(int i, int j)
    {
        player = Main.LocalPlayer;
        heldItemcontainsPotionName = potionNameContainsList.Any(player.HeldItem.Name.Contains) && !potionNameBlacklist.Any(player.HeldItem.Name.Contains);
        potionType = player.HeldItem.netID;
        player.cursorItemIconEnabled = true;
        if (!heldItemcontainsPotionName)
        {
            player.cursorItemIconID = ItemID.BottledWater;
        } else
        {
            player.cursorItemIconID = potionType;
        }
    }

    public override bool RightClick(int i, int j)
    {
        if (heldItemcontainsPotionName)
        {
            //Main.NewText(player.HeldItem.Name);

            SoundEngine.PlaySound(SoundID.SplashWeak with { MaxInstances = 0 }, new Vector2(i, j).ToWorldCoordinates());
            return true;
        }
        else
        {
            //Main.NewText("does not has potion");
            return true;
        }
    }
}

public class FruitPunchBowl : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<FruitPunchBowlTile>());
        Item.value = Item.buyPrice(0, 0, 15, 0);
    }
}
