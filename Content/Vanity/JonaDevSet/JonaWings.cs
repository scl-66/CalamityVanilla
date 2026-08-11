using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Vanity.JonaDevSet;

[AutoloadEquip(EquipType.Wings)]
public class JonaWings : ModItem
{
    public override void SetStaticDefaults()
    {
        ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = ArmorIDs.Wing.Sets.Stats[ArmorIDs.Wing.RedsWings];
    }
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 8;
        Item.accessory = true;
        Item.rare = ItemRarityID.Cyan;
        Item.value = 400000;
    }
    public override bool WingUpdate(Player player, bool inUse)
    {
        if (!player.sleeping.isSleeping)
        {
            if (inUse || Main.rand.NextBool(15))
            {
                Dust d = Dust.NewDustPerfect(player.Center + new Vector2((Main.rand.NextBool() ? Main.rand.Next(-35, -14) : Main.rand.Next(14, 20)) * player.direction, Main.rand.Next(-12, 4)).RotatedBy(player.fullRotation, player.fullRotationOrigin), DustID.FoodPiece);
                d.color = new Color(166, 25, 67);
                d.velocity = player.velocity * 0.4f;
                d.position = player.RotatedRelativePoint(d.position);
                d.shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }

            if (player.velocity.Y == 0)
            {
                player.wingFrame = 2;
                return true;
            }
        }
        return base.WingUpdate(player, inUse);
    }
}