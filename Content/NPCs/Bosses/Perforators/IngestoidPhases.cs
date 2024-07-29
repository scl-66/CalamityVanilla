using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityVanilla.Content.NPCs.Bosses.Perforators
{
    internal partial class Ingestoid
    {
        Vector2 targetspot = Vector2.Zero;
        private void Idle()
        {
            NPC.ai[2]++;
            if (NPC.ai[2] % 30 == 0) targetspot = targetplayer.Center + new Vector2(chaosnumber[0] / 64f, chaosnumber[1] / 64f);

            if (NPC.Center.Distance(targetspot) > 100 && targetspot != Vector2.Zero) NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetspot) * 6f, 0.01f);
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            if (NPC.ai[2] > 60)
            {
                phase = IngestoidPhases.Chase;
                NPC.ai[2] = 0;
            }
        }

        private void Chase()
        {
            if (NPC.Distance(targetplayer.Center) > 40f) NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetplayer.Center) * 8f, 0.05f);
            else NPC.velocity *= 0.95f;

            NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.8f);

            if (NPC.ai[2] > 240)
            {
                phase = IngestoidPhases.Idle; 
                NPC.ai[2] = 0;
            }
            NPC.ai[2]++;
        }

        private void Wall()
        {
            NPC.ai[2]++;
        }
    }
}
