using CalamityVanilla.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.NPCs.Bosses.Cryogen
{
    public partial class Cryogen : ModNPC
    {
        public void IceBarrier()
        {

        }
        public void FlyAndShoot()
        {
            NPC.velocity += NPC.Center.DirectionTo(target.Center) * 0.4f;

            if (NPC.ai[0] <= 0)
            {
                NPC.velocity.Y += 0.3f;
                NPC.velocity += NPC.Center.DirectionTo(target.Center) * 0.4f;
                NPC.velocity = NPC.velocity.LengthClamp(6, 0);
                if (NPC.ai[0] is > -30 or < -90 )
                {
                    NPC.velocity *= 0.8f;
                }
            }
            else
            {
                NPC.velocity *= 0.99f;
            }

            if (NPC.ai[0] == 0)
            {
                NPC.TargetClosest();
                NPC.velocity = NPC.Center.DirectionTo(target.Center) * 16f;
            }
            else if (NPC.ai[0] == 120)
            {
                NPC.ai[1]++;
                NPC.ai[0] = -120;
            }
            NPC.ai[0]++;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                //Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(target.Center) * 16, ProjectileID.FrostBlastHostile, 24, 0);
                
                switch(NPC.ai[1] % 3)
                {
                    case 0: // Ice blocks
                        if (NPC.ai[0] is -110 or -100 or -90) {
                            Vector2 blockPlacement = (target.Center + target.velocity * 40) + Main.rand.NextVector2Circular(16 * 4, 16 * 4);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(blockPlacement) * 48, ModContent.ProjectileType<CryogenIceBlock>(), 0, 0, ai0: blockPlacement.X, ai1: blockPlacement.Y);
                        }
                        break;
                    case 1: // Statues
                        if (NPC.ai[0] == -110)
                        {
                            for (int i = -1; i <= 1; i++)
                            {
                                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(target.Center).RotatedBy(0.1 * i) * (i == 0 ? 10 : 8), ProjectileID.FrostBlastHostile, 24, 0);
                            }
                        }
                        break;
                    case 2: // Bombs
                        if (NPC.ai[0] == -110)
                        {
                            for (int i = -1; i <= 1; i++)
                            {
                                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(target.Center).RotatedBy(0.2 * i) * (i == 0 ? 10 : 8), ModContent.ProjectileType<IceBomb>(), 24, 0, -1, Main.rand.Next(2));
                            }
                        }
                        break;
                }
            }
        }
    }
}
