using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace CalamityVanilla.Content.NPCs.Bosses.Perforators
{
    public partial class PerforatorBigWorm
    {
        public void Idle()
        {

        }

        public void Follow()
        {
            NPC.TargetClosest();
            NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

            float followspeed = NPC.Center.Distance(target.Center) > 500f ? 0.4f : 0.1f;
            NPC.velocity += NPC.Center.DirectionTo(target.Center) * followspeed;

            NPC.velocity = NPC.velocity.LengthClamp(NPC.Center.Distance(target.Center) / 50f);

            if (Main.rand.NextBool(16)) 
            {
                Dust d2 = Dust.NewDustPerfect(NPC.Center + new Vector2(0, 30).RotatedBy(NPC.rotation) + Main.rand.NextVector2Circular(20f,20f), DustID.Ichor, NPC.velocity);
                d2.noGravity = !Main.rand.NextBool(3);
            }
            if (Main.rand.NextBool(10 - (int)Math.Clamp(NPC.Center.Distance(target.Center) / 50f,0,9)))
            {
                Dust d = Dust.NewDustPerfect(NPC.Center - new Vector2(0, 15).RotatedBy(NPC.rotation) + Main.rand.NextVector2Circular(40f, 60f).RotatedBy(NPC.rotation), DustID.Blood, -NPC.velocity/2, 128, Scale: 1.2f);
                d.noGravity = false;
            }
        }
    }
}
