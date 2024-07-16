using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles.Ranged
{
    public class BlossomFluxBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.QuickDefaults();
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override void OnKill(int timeLeft)
        {
            for(int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ChlorophyteWeapon);
                d.velocity = Main.rand.NextVector2Circular(6, 6);
                d.noGravity = true;
                d.fadeIn = Main.rand.NextFloat(0, 2f);
            }

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(4, 4), ModContent.ProjectileType<BlossomFluxSpores>(), Projectile.damage / 3, 2,Projectile.owner);
                }
            }
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if(Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if(Projectile.frame == 4)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.velocity.Y += 0.1f;
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 1f, 0) * 0.5f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = TextureAssets.Projectile[Type];

            for(int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float multiply = (1 - (i / 5f));
                Main.EntitySpriteDraw(tex.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, new Rectangle(0, tex.Height() / 4 * Projectile.frame, tex.Width(), tex.Height() / 4), new Color(1f, 1f, 1f, 0f) * multiply, Projectile.rotation, tex.Size() / new Vector2(2, 8), 1f + (multiply * 0.2f), SpriteEffects.None);
            }

            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, new Rectangle(0,tex.Height() / 4 * Projectile.frame,tex.Width(),tex.Height() / 4), new Color(1f,1f,1f,0.5f), Projectile.rotation, tex.Size() / new Vector2(2,8), 1f, SpriteEffects.None);
            return false;
        }
    }
}
