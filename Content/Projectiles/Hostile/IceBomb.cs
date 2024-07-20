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

namespace CalamityVanilla.Content.Projectiles.Hostile
{
    public class IceBomb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(true, 32);
            Projectile.timeLeft = 60 * 3;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            Projectile.rotation += 0.1f;
            Projectile.scale = 1f + MathF.Sin(Projectile.timeLeft * 0.1f) * 0.1f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14,Projectile.position);
            for(int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,DustID.IceRod);
                d.velocity = Main.rand.NextVector2Circular(6, 6);
                d.noGravity = true;
            }
            if(Main.netMode != NetmodeID.MultiplayerClient)
            {
                for(int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, 8).RotatedBy(MathHelper.PiOver2 * i).RotatedBy(Projectile.ai[0] == 0 ? 0 : MathHelper.PiOver4), ModContent.ProjectileType<IceShrapnel>(), 25, 0);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, new Rectangle(tex.Height(), 0, tex.Height(), tex.Height()), new Color(1f, 1f, 1f, 0.5f), Projectile.rotation, new Vector2(tex.Height() / 2), Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, tex.Height(), tex.Height()), new Color(1f, 1f, 1f, 0.5f), 0,new Vector2(tex.Height() / 2),Projectile.scale,SpriteEffects.None);
            return false;
        }
    }
}
