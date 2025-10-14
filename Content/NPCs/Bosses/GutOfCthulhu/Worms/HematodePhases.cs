using CalamityVanilla.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.Bosses.GutOfCthulhu.Worms;

partial class Hematode : WormNPC
{
    private void Idle()
    {
        NPC.TargetClosest();
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.Center.DirectionTo(targetplayer.Center).ToRotation() + MathHelper.PiOver2, 0.05f);

        float distmult = Math.Clamp(targetplayer.velocity.Length() / 2f, 1f, 5f);

        if (NPC.Center.Distance(targetplayer.Center) > 450f ||
            CVUtils.AngleDifference(NPC.rotation - MathHelper.PiOver2, NPC.velocity.ToRotation()) > 0.5f) NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.Center.DirectionTo(targetplayer.Center) * 2f * distmult, 0.05f);
        else NPC.velocity *= 0.97f;
    }
}