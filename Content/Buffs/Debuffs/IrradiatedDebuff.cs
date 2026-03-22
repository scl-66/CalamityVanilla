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

    public override void ResetEffects()
    {
        lifeRegenDebuff = false;
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
            LocalizedText DeathText = Language.GetText($"Mods.CalamityVanilla.DeathMessage.RadiationPoisoned{Main.rand.Next(1, 4)}");
            damageSource = PlayerDeathReason.ByCustomReason(DeathText.ToNetworkText(Player.name));
        }
        return true;
    }
    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
    }
}