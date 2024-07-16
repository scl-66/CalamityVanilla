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
    public class BlossomFluxLeaf : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.QuickDefaults();
            Projectile.extraUpdates = 1;
            Projectile.arrow = true;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 20)
            {
                Projectile.velocity.Y += 0.15f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 1f, 0) * 0.5f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = TextureAssets.Projectile[Type];

            for(int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float multiply = (1 - (i / 10f));
                Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, null, new Color(0.8f * multiply, 0.8f, 0.6f * multiply,0.3f) * multiply, Projectile.rotation, tex.Size() / 2, 0.2f + multiply, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, new Color(1f,1f,1f,1f), Projectile.rotation, tex.Size() / 2, 1.2f, SpriteEffects.None);
            return false;
        }
    }
}
