using CalamityVanilla.Common.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Cryonic.Ore;

public class FrozenTool : ModBuff
{
    public static int MaxTime = 60 * 6;
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        MaxTime = 60 * 6;
        if (player.HeldItem.pick >= 100)
            return;

        player.pickSpeed += Utils.Remap(player.buffTime[buffIndex],0, MaxTime, 0,2);
    }
}
public class FrozenToolItem : GlobalItem
{
    public override void PostModifyItemDraw(Item item, ref PlayerDrawSet drawInfo, DrawData drawData, DrawData? coloredDrawData, DrawData? glowmaskDrawData)
    {
        if (item.pick >= 100)
            return;
        int index = drawInfo.drawPlayer.FindBuffIndex(ModContent.BuffType<FrozenTool>());
        if(index != -1)
        {
            float colorMult = Utils.Remap(drawInfo.drawPlayer.buffTime[index], 0, FrozenTool.MaxTime, 0, 1);
            drawInfo.DrawDataCache.Add(drawData with { color = drawData.color.MultiplyRGBA(new Color(0, 0.7f, 1, 0.25f)) * colorMult, shader = ContentSamples.ItemsByType[ItemID.SkyBlueDye].dye });
        }
    }
    public override void MeleeEffects(Item item, Player player, Rectangle hitbox)
    {
        if (item.pick >= 100)
            return;
        int index = player.FindBuffIndex(ModContent.BuffType<FrozenTool>());
        if (index != -1)
        {
            Vector2 position = hitbox.TopLeft();
            if (Main.rand.NextBool((int)Utils.Remap(player.buffTime[index], 0, FrozenTool.MaxTime, 100, 4)))
            {
                CVUtils.GetPointOnSwungItemPath(TextureAssets.Item[item.type].Width(), TextureAssets.Item[item.type].Height(), Main.rand.NextFloat(),item.scale,out Vector2 pos, out Vector2 dir, player);
                Dust d = Dust.NewDustPerfect(pos, DustID.Snow, dir.RotatedBy(player.direction * player.gravDir * MathHelper.PiOver2));
                d.noGravity = true;
            }
        }
    }
    public override float UseSpeedMultiplier(Item item, Player player)
    {
        if (item.pick >= 100)
            return 1f;
        int index = player.FindBuffIndex(ModContent.BuffType<FrozenTool>());
        if (index != -1)
        {
            return Utils.Remap(player.buffTime[index], 0, FrozenTool.MaxTime, 1, 0.33f);
        }
        return 1f;
    }
}
