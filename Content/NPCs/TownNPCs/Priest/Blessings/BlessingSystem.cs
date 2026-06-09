using CalamityVanilla.Content.NPCs.TownNPCs.Priest;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest.Blessings;

public class BlessingSystem : ModSystem
{
    public static PriestBlessing Frictionless { get; private set; }
    public static PriestBlessing Moleman { get; private set; }
    public static PriestBlessing NoWings { get; private set; }
    public static PriestBlessing Greedy { get; private set; }
    public static PriestBlessing Adrenaline { get; private set; }
    public static PriestBlessing SpawnrateUp { get; private set; }
    public static void InitializeBlessings()
    {
        Frictionless = new PriestBlessing<FrictionlessBlessingPlayer>("Frictionless", new Tribute(ItemID.IceBlock, 5));
        Moleman = new PriestBlessing<MolemanBlessingPlayer>("Moleman", new Tribute(ItemID.Torch, 50));
        NoWings = new PriestBlessing<NoWingsBlessingPlayer>("NoWings", new Tribute(ItemID.SoulofFlight, 2));
        Greedy = new PriestBlessing<GreedyBlessingPlayer>("Greedy", new Tribute(ItemID.GoldDust, 3));
        Adrenaline = new PriestBlessing<AdrenalineBlessingPlayer>("Adrenaline", new Tribute(ItemID.LifeFruit, 1));
        SpawnrateUp = new PriestBlessing<SpawnrateUpBlessingPlayer>("SpawnrateUp", new Tribute(ItemID.Ectoplasm, 1));
    }
    public override void PostSetupContent()
    {
        InitializeBlessings();
    }
}
public record struct Tribute(int ItemType, int Stack);

public abstract class PriestBlessing
{
    public Asset<Texture2D> Icon { get; protected set; }
    public LocalizedText DisplayName { get; protected set; }
    public LocalizedText Description { get; protected set; }
    public LocalizedText Stats { get; protected set; }
    //public Tribute[] TributeList { get; protected set; }
    public Tribute Tribute { get; protected set; }
    public abstract bool Enable(Player player);
    public abstract bool CheckEnable(Player player);
    public abstract void Disable(Player player);
    public abstract bool GetState(Player player);
}

public sealed class PriestBlessing<T> : PriestBlessing where T : BlessingPlayer
{
    public PriestBlessing(string name, Tribute tribute)
    {
        DisplayName = Language.GetOrRegister($"Mods.CalamityVanilla.Blessings.{name}.DisplayName");
        Description = Language.GetOrRegister($"Mods.CalamityVanilla.Blessings.{name}.Description");
        Stats = Language.GetOrRegister($"Mods.CalamityVanilla.Blessings.{name}.Stats");
        Icon = ModContent.Request<Texture2D>($"CalamityVanilla/Content/NPCs/TownNPCs/Priest/Blessings/{name}Icon");

        Tribute = tribute;

        PriestUIState.AddBlessing(this);
    }

    public override bool GetState(Player player)
    {
        //return player.GetModPlayer<T>()?.Active ?? false;
        if (player.TryGetModPlayer<T>(out var modPlayer))
        {
            return modPlayer.Active;
        }
        return false;
    }

    public override bool CheckEnable(Player player)
    {
        if (player.CountItem(Tribute.ItemType, Tribute.Stack) < Tribute.Stack)
        {
            return false;
        }
        return true;
    }
    public override bool Enable(Player player)
    {
        if (!CheckEnable(player))
            return false;

        for (int i = 0; i < Tribute.Stack; i++)
        {
            player.ConsumeItem(Tribute.ItemType);
        }
        player.GetModPlayer<T>().Active = true;
        return true;
    }
    public override void Disable(Player player)
    {
        player.GetModPlayer<T>().Active &= false;
    }
}

public abstract class BlessingPlayer : ModPlayer
{
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
        }
        else
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
            Player.runAcceleration *= 0.6f;
            Player.runSlowdown = tilesBelow ? 0 : 0.1f;
            if (tilesBelow)
            {
                float velLength = Player.velocity.Length();
                if (velLength > 3)
                {
                    int num = (int)Utils.Remap(velLength, 0, 10, 15, 2, true);
                    float velX = Math.Clamp(Math.Abs(Player.velocity.X / 15), 0.3f, 10f) * Player.velocity.SafeNormalize(Vector2.UnitX).X - 0.1f;
                    int i = 1;
                    if (Math.Abs(velLength) > 7)
                    {
                        i = (int)Utils.Remap(velLength, 7, 10, 1, 2);
                    }
                    for (int j = 0; j < i; j++)
                    {
                        if (Main.rand.NextBool(num))
                        {
                            Dust d = Dust.NewDustDirect(Player.position + new Vector2(0, 30), Player.width, Player.width, DustID.Ice, velX/2, -Main.rand.NextFloat(1f, 2f));
                            d.scale = Main.rand.NextFloat(0.5f, 1.2f);
                            d.velocity *= 0.5f;
                            d.noGravity = false;
                        }
                    }
                }
            }
        }
    }

    public override void FrameEffects()
    {
        if (Active)
        {
            Player.slippy2 = true;
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
            Player.runAcceleration *= 1.25f;
            if (!Player.mount.Active)
            {
                Player.maxRunSpeed *= 1.25f;
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
        if (Active && Player.equippedWings != null)
        {
            Player.wingTime = 0;
            Player.wingTimeMax = 0;
            Player.wingsLogic = 0;
            Player.rocketTime = 0;
            Player.rocketTimeMax = 0;
            Player.runAcceleration *= 2.0f;
            if (!Player.mount.Active)
            {
                Player.maxRunSpeed *= 2.0f;
            }
        }
    }
}

public sealed class GreedyBlessingBuff : BlessingBuff<GreedyBlessingPlayer> { }
public sealed class GreedyBlessingPlayer : BlessingPlayer
{
    public override int BuffType => ModContent.BuffType<GreedyBlessingBuff>();
    public override void OnHurt(Player.HurtInfo info)
    {
        if (Active)
        {
            LocalizedText DeathText = Language.GetText($"Mods.CalamityVanilla.DeathMessage.GreedyDeath{Main.rand.Next(1, 16)}");
            info.DamageSource.TryGetCausingEntity(out Entity entity);
            PlayerDeathReason damageSource = PlayerDeathReason.ByCustomReason(DeathText.ToNetworkText(Player.name, Main.npc[entity.whoAmI].GivenOrTypeName));
            Player.KillMe(damageSource, 9999, 0);
        }
    }
}
public sealed class AdrenalineBlessingBuff : BlessingBuff<AdrenalineBlessingPlayer> { }
public sealed class AdrenalineBlessingPlayer : BlessingPlayer
{
    public override int BuffType => ModContent.BuffType<AdrenalineBlessingBuff>();
    public override void OnHurt(Player.HurtInfo info)
    {
        if (Active)
        {
            LocalizedText DeathText = Language.GetText($"Mods.CalamityVanilla.DeathMessage.GreedyDeath{Main.rand.Next(1, 16)}");
            info.DamageSource.TryGetCausingEntity(out Entity entity);
            PlayerDeathReason damageSource = PlayerDeathReason.ByCustomReason(DeathText.ToNetworkText(Player.name, Main.npc[entity.whoAmI].GivenOrTypeName));
            Player.KillMe(damageSource, 9999, 0);
        }
    }
}
public sealed class SpawnrateUpBlessingBuff : BlessingBuff<SpawnrateUpBlessingPlayer> { }
public sealed class SpawnrateUpBlessingPlayer : BlessingPlayer
{
    public override int BuffType => ModContent.BuffType<SpawnrateUpBlessingBuff>();
    public override void OnHurt(Player.HurtInfo info)
    {
        if (Active)
        {
            LocalizedText DeathText = Language.GetText($"Mods.CalamityVanilla.DeathMessage.GreedyDeath{Main.rand.Next(1, 16)}");
            info.DamageSource.TryGetCausingEntity(out Entity entity);
            PlayerDeathReason damageSource = PlayerDeathReason.ByCustomReason(DeathText.ToNetworkText(Player.name, Main.npc[entity.whoAmI].GivenOrTypeName));
            Player.KillMe(damageSource, 9999, 0);
        }
    }
}