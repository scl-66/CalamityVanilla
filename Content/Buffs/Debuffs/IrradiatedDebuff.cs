using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Underground.Items.Ores.PostHardmode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Buffs.Debuffs;

// This class serves as an example of a debuff that causes constant loss of life
// See ExampleLifeRegenDebuffPlayer.UpdateBadLifeRegen at the end of the file for more information
public class IrradiatedDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;  // Is it a debuff?
        Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
        Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
        BuffID.Sets.LongerExpertDebuff[Type] = true; // If this buff is a debuff, setting this to true will make this buff last twice as long on players in expert mode
    }

    // Allows you to make this buff give certain effects to the given player
    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<IrradiatedRegen>().lifeRegenDebuff = true;
    }
}

public class IrradiatedRegen : ModPlayer
{
    // Flag checking when life regen debuff should be activated
    public bool lifeRegenDebuff;
    public int damage;
    public int dustChance;

    public override void ResetEffects()
    {
        lifeRegenDebuff = false;
    }

    public override void PreUpdate()
    {
        float closestDist = 10000f;
        for (int x = -3; x < 4; x++) {
            for (int y = -3; y < 4; y++)
            {
                Point tPos = new Point(Player.Center.ToTileCoordinates().X + x, Player.Center.ToTileCoordinates().Y + y);
                Vector2 tCenter = tPos.ToWorldCoordinates();
                Tile t = Framing.GetTileSafely(tPos);
                if (Main.tile[tPos].TileType == ModContent.TileType<UraniumOreTile>() || Main.tile[tPos].TileType == ModContent.TileType<PlutoniumOreTile>())
                {
                    float dist = Vector2.Distance(Player.Hitbox.ClosestPointInRect(tCenter), tCenter);
                    if (dist < 90f)
                    {
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                        }
                    }
                }
            }
        }
        if (closestDist < 90f)
        {
            Main.LocalPlayer.AddBuff(ModContent.BuffType<IrradiatedDebuff>(), 5);
            Main.LocalPlayer.GetModPlayer<IrradiatedRegen>().damage = (int)(1/closestDist * 150);
            Main.LocalPlayer.GetModPlayer<IrradiatedRegen>().dustChance = (int)Math.Clamp(closestDist/4, 2, 1000);
        }
    }

    // Allows you to give the player a negative life regeneration based on its state (for example, the "On Fire!" debuff makes the player take damage-over-time)
    // This is typically done by setting player.lifeRegen to 0 if it is positive, setting player.lifeRegenTime to 0, and subtracting a number from player.lifeRegen
    // The player will take damage at a rate of half the number you subtract per second
    public override void UpdateBadLifeRegen()
    {
        if (lifeRegenDebuff)
        {
            // These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;
            // Player.lifeRegenTime used to increase the speed at which the player reaches its maximum natural life regeneration
            // So we set it to 0, and while this debuff is active, it never reaches it
            Player.lifeRegenTime = 0;
            // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 8 life lost per second
            Player.lifeRegen -= damage;
        }
    }
    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        if (lifeRegenDebuff)
        {
            LocalizedText DeathText = Language.GetText($"Mods.CalamityVanilla.DeathMessage.RadiationPoisoned{Main.rand.Next(1, 11)}");
            damageSource = PlayerDeathReason.ByCustomReason(DeathText.ToNetworkText(Player.name));
        }
        return true;
    }
    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (Main.GameUpdateCount % (dustChance <= 0 ? dustChance = 2 : dustChance) == 0 && lifeRegenDebuff)
        {
            float speed = 0.1f;
            Dust.NewDustDirect(Player.position, Player.width, Player.height, ModContent.DustType<UraniumRadDust>(), Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed, speed));
        }
    }
}