using CalamityVanilla.Common.Dusts;
using CalamityVanilla.Common.Items;
using CalamityVanilla.Common.Templates;
using CalamityVanilla.Content.Tundra.Buffs;
using CalamityVanilla.Content.Tundra.Cryonic.Ore;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Cryonic.FlaskOfCold;

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
    public override void ItemEffects(Player player, Vector2 position, int width, int height)
    {
        if (Main.rand.NextBool(7))
        {
            Dust d = Dust.NewDustDirect(position, width, height, ModContent.DustType<CryonicSmokeDust>());
            d.fadeIn = 10;
            d.noGravity = true;
            d.customData = 0.03f;
            d.velocity *= 0.4f;
            d.velocity += new Vector2(player.direction, -player.gravDir * 0.5f);
        }
        if (Main.rand.NextBool(5))
        {
            Dust d = Dust.NewDustDirect(position, width, height, DustID.Snow);
            d.noGravity = true;
            d.velocity *= 0.4f;
            d.velocity += new Vector2(player.direction, -player.gravDir * 0.5f);
            d.fadeIn += 0.5f;
        }
    }
    public override void ProjectileEffects(Projectile projectile, Vector2 position, int width, int height)
    {
        if (Main.rand.NextBool(7))
        {
            Dust d = Dust.NewDustDirect(position, width, height, ModContent.DustType<CryonicSmokeDust>());
            d.fadeIn = 10;
            d.noGravity = true;
            d.customData = 0.03f;
        }
        if (Main.rand.NextBool(5))
        {
            Dust d = Dust.NewDustDirect(position, width, height, DustID.Snow);
            d.noGravity = true;
            d.fadeIn += 0.5f;
        }
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.ImbuingStation).AddIngredient(ItemID.BottledWater).AddIngredient(AutoContent.ModItem<CryonicOre>()).Register();
    }
}
