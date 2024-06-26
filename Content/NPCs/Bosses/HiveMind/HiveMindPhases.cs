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
using Terraria.ModLoader.Config;

namespace CalamityVanilla.Content.NPCs.Bosses.HiveMind
{
    public partial class HiveMind
    {
        private void Teleport()
        {
            NPC.ai[0]++;

            if (NPC.ai[0] > 0)
            {
                NPC.alpha += 5;
            }
            if (NPC.ai[0] == (255 / 5))
            {
                NPC.TargetClosest();
                Vector2 chosenTile = Vector2.Zero;
                Point targetPos = ((target.Center + (target.velocity * 64)) / 16).ToPoint();
                if (NPC.AI_AttemptToFindTeleportSpot(ref chosenTile, targetPos.X, targetPos.Y))
                {
                    chosenTile *= 16;
                    NPC.position = new Vector2(chosenTile.X - NPC.width / 2, chosenTile.Y - NPC.height);
                }
                else
                {
                    NPC.Bottom = target.Bottom;
                }
            }
            if (NPC.ai[0] > (255 / 5) && NPC.alpha > 0)
            {
                NPC.alpha -= 10;
            }
            if (NPC.ai[0] == (512 / 5))
            {
                phase = 1;
                NPC.ai[0] = 0;
                NPC.netUpdate = true;
            }
        }
        private void ShootSporeBombs()
        {
            NPC.ai[0]++;
            if (NPC.ai[0] > 60)
            {
                NPC.ai[0] = 40;
                NPC.ai[1]++;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.Center.DirectionTo(target.Center).RotatedByRandom(0.1f) * Main.rand.NextFloat(6, 12), ModContent.ProjectileType<SporeBomb>(), 25, 1, -1, Main.rand.NextFloat(-1f, 1f));
            }

            if (NPC.ai[1] == 3)
            {
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
                phase = 0;
            }
        }
        private void VineAttack()
        {
        }
    }
}
