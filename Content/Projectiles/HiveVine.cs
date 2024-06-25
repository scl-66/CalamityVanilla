using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles
{
    public class HiveVine : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(true, 32);
            Projectile.alpha = 255;
            Projectile.timeLeft = 60 * 7;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = TextureAssets.Projectile[Type];
            Rectangle rect = Projectile.frame == 0 ? new Rectangle(0, 0, 42, 50) : new Rectangle(0, 52 + (32 * (Projectile.frame - 1)), 42, 32);
            Main.EntitySpriteDraw(tex.Value, Projectile.Bottom - Main.screenPosition, rect,lightColor * Projectile.Opacity, 0, new Vector2(rect.Width / 2, rect.Height),1,SpriteEffects.None);

            return false;
        }
        public override void AI()
        {
            if(Projectile.alpha > 0)
            {
                Projectile.alpha -= 15;
            }

            if (Projectile.timeLeft <= 17)
            {
                Projectile.alpha += 30;
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
                d.alpha = 128;
                d.velocity *= 0.4f;
            }

            if (Main.rand.NextBool(15))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
                d.alpha = 128;
                d.velocity *= 0.4f;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 5 && Projectile.ai[1] > 0)
            {
                Projectile.frame = Projectile.ai[1] % 2 == 0 ? 1 : 2;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(0, -30), Vector2.Zero, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Projectile.ai[1] - 1);
            }
        }
    }
}
