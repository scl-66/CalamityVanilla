using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.WorldBuilding;

namespace CalamityVanilla.Content.NPCs.Bosses.GutOfCthulhu.Worms;

internal partial class Ingestoid
{
    Vector2 targetspot = Vector2.Zero;

    //Idle Action - base idling
    private void Idle()
    {
        NPC.TargetClosest();
        if (NPC.ai[2] % 30 == 0) targetspot = targetplayer.Center + new Vector2(chaosnumber[0] / 64f, chaosnumber[1] / 64f);

        if (NPC.Center.Distance(targetspot) > 100 && targetspot != Vector2.Zero) NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetspot) * 6f, 0.01f);

        if (NPC.ai[2] > 30)
        {
            if (chaosnumber[0] > 150) phase = IngestoidPhases.Wall;
            else phase = IngestoidPhases.Chase;
            NPC.ai[2] = 0;
        }
        NPC.ai[2]++;
    }

    //Chase Action - base chase
    private void Chase()
    {
        float distmult = Math.Clamp(targetplayer.velocity.Length() / 4f, 1f, 2f);
        targetspot = targetplayer.Center + new Vector2(0, (float)Math.Sin(NPC.ai[2] / 20f) * 60f);
        if (NPC.Distance(targetspot) > 40f) NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetspot) * 10f, 0.05f);
        else NPC.velocity *= 0.95f;

        if (NPC.ai[2] > 180)
        {
            phase = IngestoidPhases.Idle;
            NPC.ai[2] = 0;
        }
        NPC.ai[2]++;
    }

    bool wallswitch = false;
    float olddirection = 1;

    //Wall attack - Ingestoid will try and and go around the player and intercept them
    private void Wall()
    {
        if (olddirection != targetplayer.direction)
        {
            NPC.ai[3] = 40;
            olddirection = targetplayer.direction;
        }

        Vector2 disttonpc = targetplayer.Center - NPC.Center;

        if (NPC.ai[3] <= 0) targetspot = targetplayer.Center + new Vector2(400, (float)Math.Sin((disttonpc.X / 180f) - MathHelper.PiOver2) * 300f) * targetplayer.direction;

        if (NPC.Center.Distance(targetspot) > 120f && (!wallswitch || NPC.ai[3] <= 0))
        {
            float distmult = Math.Clamp(targetplayer.velocity.Length() / 20f, 1f, 2f);
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetspot) * 12f, 0.05f);
        }
        else if (NPC.Center.Distance(targetspot) <= 120f)
        {
            NPC.velocity *= 0.95f;
        }
        else if (NPC.Center.Distance(targetspot) <= 120f &&
            (NPC.Center.X - targetplayer.Center.X * targetplayer.direction) > 100f &&
            Math.Abs(NPC.Center.Y - targetplayer.Center.Y) < 80f)
        {
            wallswitch = true;
        }
        else
        {
            NPC.velocity *= 0.95f;
        }

        NPC.ai[3]--;

        if (NPC.ai[2] > 240)
        {
            phase = IngestoidPhases.Idle;
            wallswitch = false;
            NPC.ai[2] = 0;
        }

        NPC.ai[2]++;
    }
}