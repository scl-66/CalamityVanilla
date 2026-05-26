using CalamityVanilla.Content.NPCs.TownNPCs.Priest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.Localization.NetworkText;

namespace CalamityVanilla.Common.Blessings;

public class BlessingSystem : ModSystem
{
    public static PriestBlessing Frictionless { get; private set; }
    public static PriestBlessing Moleman { get; private set; }
    public static PriestBlessing NoWings { get; private set; }
    public static void InitializeBlessings()
    {
        Frictionless = new PriestBlessing(ModContent.GetInstance<FrictionlessBlessingPlayer>(), "Frictionless", [
            new Tribute(ItemID.IceBlock, 5)
            ]);
        Moleman = new PriestBlessing(ModContent.GetInstance<MolemanBlessingPlayer>(), "Moleman", [
            new Tribute(ItemID.Torch, 50)
            ]);
        Moleman = new PriestBlessing(ModContent.GetInstance<NoWingsBlessingPlayer>(), "NoWings", [
            new Tribute(ItemID.SoulofFlight, 1)
            ]);
    }
    public override void PostSetupContent()
    {
        InitializeBlessings();
    }
}
public record struct Tribute(int ItemType, int Stack, int Money = 0);

public sealed class PriestBlessing
{
    public Asset<Texture2D> Icon { get; }
    public LocalizedText DisplayName { get; }
    public LocalizedText Description { get; }
    public LocalizedText Stats { get; }
    public BlessingPlayer BlessedPlayer { get; }
    public Tribute[] TributeList { get; }
        
    public PriestBlessing(BlessingPlayer player, string name, Tribute[] tributes)
    {
        DisplayName = Language.GetOrRegister($"Mods.CalamityVanilla.Blessings.{name}.DisplayName");
        Description = Language.GetOrRegister($"Mods.CalamityVanilla.Blessings.{name}.Description");
        Stats = Language.GetOrRegister($"Mods.CalamityVanilla.Blessings.{name}.Stats");
        Icon = ModContent.Request<Texture2D>($"CalamityVanilla/Common/Blessings/{name}Icon");

        BlessedPlayer = player;
        player.Blessing = this;
        TributeList = tributes;

        PriestUIState.AddBlessing(this);
    }

    public void Toggle()
    {
        BlessedPlayer.Active = !BlessedPlayer.Active;
    }
}

public abstract class BlessingPlayer : ModPlayer
{
    public PriestBlessing Blessing { get; set; }
    public bool Active { get; set; }

    public abstract int BuffType { get; }

    // Inherited players would check Active to make effects work
    public override void PreUpdateBuffs()
    {
        if (Active)
        {
            Player.AddBuff(BuffType, 5);
        }
    }
}

public class BlessingBuff<T> : ModBuff where T : BlessingPlayer
{
    public override void SetStaticDefaults()
    {
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        Main.debuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        if (player.GetModPlayer<T>().Active)
        {
            player.buffTime[buffIndex] = 5;
        } else
        {
            player.ClearBuff(Type);
        }
    }
}
public sealed class FrictionlessBlessingBuff : BlessingBuff<FrictionlessBlessingPlayer> { }
public sealed class FrictionlessBlessingPlayer : BlessingPlayer
{
    public override int BuffType => ModContent.BuffType<FrictionlessBlessingBuff>();
    public override void PostUpdateRunSpeeds()
    {
        bool tilesBelow = false;
        int playerX = Player.Bottom.ToTileCoordinates().X;
        int playerY = Player.Bottom.ToTileCoordinates().Y;
        for (int i = -1; i < 2; i++)
        {
            if (Main.tile[playerX + i, playerY].HasTile)
            {
                tilesBelow = true;
                break;
            }
        }
        if (Active)
        {
            if (tilesBelow)
            {
                //Player.slippy2 = true;
                Player.runSlowdown = 0;
                Player.runAcceleration *= 0.6f;
            }
            else
            {
                //Player.slippy2 = true;
                Player.runSlowdown = 0.1f;
                Player.runAcceleration *= 0.6f;
            }
        }
    }
}
public sealed class MolemanBlessingBuff : BlessingBuff<MolemanBlessingPlayer> { }
public sealed class MolemanBlessingPlayer : BlessingPlayer
{
    public bool isGettingMoled;
    public override int BuffType => ModContent.BuffType<MolemanBlessingBuff>();
    public override void PostUpdateMiscEffects()
    {
        int playerX = Player.Center.ToTileCoordinates().X;
        int playerY = Player.Center.ToTileCoordinates().Y;
        bool onSurface = playerY - 1 < Main.worldSurface;
        
        // tile wall check
        int tileCheckAmt = 8;
        if (onSurface && Active)
        {
            isGettingMoled = false;
            while (tileCheckAmt > 0)
            {
                if (!WorldGen.InWorld(playerX, playerY))
                {
                    break;
                }

                Tile tile = Main.tile[playerX, playerY];

                if (tile.WallType == WallID.None || tile.WallType == WallID.Glass || tile.WallType == WallID.EchoWall || (!Main.ShouldShowInvisibleWalls() && tile.IsWallInvisible))
                {
                    isGettingMoled = true;
                    break;
                }

                tileCheckAmt--;
                playerY--;
                if (WorldGen.SolidTile3(playerX, playerY) && tile.TileType != TileID.Glass && (!tile.invisibleBlock() || Main.ShouldShowInvisibleWalls()) && (tile.TileType != TileID.EchoBlock || Main.ShouldShowInvisibleWalls()))
                {
                    isGettingMoled = false;
                    break;
                }
            }
        }
        else
        {
            isGettingMoled = false;
        }
        if (Active)
        {
            if (isGettingMoled)
            {
                Player.AddBuff(BuffID.Obstructed, 5);
            }
            else
            {
                Player.moveSpeed += 0.15f;
                Player.pickSpeed -= 0.25f;
            }
        }
    }
    public override void PostUpdateRunSpeeds()
    {
        if (!isGettingMoled && Active)
        {
            Player.runAcceleration *= 1.75f;
            if (!Player.mount.Active)
            {
                Player.maxRunSpeed *= 1.55f;
            }
        }
    }
}
public sealed class NoWingsBlessingBuff : BlessingBuff<NoWingsBlessingPlayer> { }
public sealed class NoWingsBlessingPlayer : BlessingPlayer
{
    public override int BuffType => ModContent.BuffType<NoWingsBlessingBuff>();
    public override void PostUpdateRunSpeeds()
    {
        if (Active)
        {
            Player.velocity.Y *= 1.2f;
        }
    }
}