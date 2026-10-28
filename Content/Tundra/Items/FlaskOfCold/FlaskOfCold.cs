using CalamityVanilla.Common.Templates;
using CalamityVanilla.Content.Tundra.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Items.FlaskOfCold;

public class FlaskOfCold : Flask
{
    public override Color[] DrinkColors => [
            new Color(229, 238, 255),
            new Color(173, 210, 255),
            new Color(133, 166, 233)];
    public override void OnHit(NPC target)
    {
        target.AddBuff(ModContent.BuffType<ChilledCV>(), Main.rand.Next(300, 600));
    }
    public override void Effects(Vector2 position, int width, int height)
    {
        Dust d = Dust.NewDustDirect(position, width, height, Main.rand.Next([DustID.Snow, DustID.Snow, DustID.Ice]));
        d.noGravity = true;
        d.velocity *= 0.5f;
    }
}
