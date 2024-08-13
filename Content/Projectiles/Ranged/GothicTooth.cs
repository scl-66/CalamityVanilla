using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles.Ranged
{
    public class GothicTooth : ModProjectile // Example Mod Jumpscare
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = TextureAssets.Projectile[Type];

            if (Projectile.ai[0] == 0)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
                {
                    float multiply = (1 - (i / 6f));
                    Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, null, new Color(1f, 0f, 0f, 0f) * multiply, Projectile.rotation, tex.Size() / 2, 0.8f + (multiply * 0.2f), SpriteEffects.None);
                }
            }

            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, Projectile.ai[0] == 0 ? lightColor : Color.Lerp(lightColor,Color.Red,Main.masterColor), Projectile.rotation, tex.Size() / 2, 1f, SpriteEffects.None);
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.QuickDefaults();
            Projectile.arrow = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0) // Flying
            {
                Projectile.ai[2]++;
                if (Projectile.ai[2] > 40)
                {
                    Projectile.velocity.Y += 0.17f;
                }
                if (Main.rand.NextBool(3))
                {
                    Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.VampireHeal);
                    d.velocity *= 0.5f;
                    d.noGravity = true;
                }
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center + Projectile.velocity * 0.8f;
                if (!Main.npc[(int)Projectile.ai[1]].active)
                {
                    Projectile.Kill();
                }
                //Projectile.ai[2]++;
                //if (Projectile.ai[2] >= 20)
                //{
                //    Projectile.ai[2] = 0;
                //}
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[0] == 0;
        }
        private readonly Point[] stickingTeeth = new Point[12];
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 60 * 6;
            Projectile.damage = 0;
            Projectile.ai[0] = 1;
            Projectile.ai[2] = 0;
            Projectile.ai[1] = target.whoAmI;
            Projectile.velocity = Projectile.Center - target.Center;
            Projectile.netUpdate = true;
            Projectile.KillOldestJavelin(Projectile.whoAmI, Type, (int)Projectile.ai[1], stickingTeeth);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            // If attached to an NPC, draw behind tiles (and the npc) if that NPC is behind tiles, otherwise just behind the NPC.
            if (Projectile.ai[0] == 1)
            {
                int npcIndex = (int)Projectile.ai[1];
                if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
                {
                    if (Main.npc[npcIndex].behindTiles)
                    {
                        behindNPCsAndTiles.Add(index);
                    }
                    else
                    {
                        behindNPCsAndTiles.Add(index);
                    }
                    return;
                }
            }
            // Since we aren't attached, add to this list
            behindNPCsAndTiles.Add(index);
        }
        public override void OnKill(int timeLeft)
        {
            for(int i = 0; i < 15; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.VampireHeal);
                d.velocity *= 0.5f;
                d.noGravity = true;
            }
        }
    }
}
