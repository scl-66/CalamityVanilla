using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace CalamityVanilla.Content.Projectiles.Magic
{
    public class MyceliumShroom : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(false, 38);
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60*1;
            Projectile.frame = Main.rand.Next(0, 3);
            Projectile.spriteDirection = Main.rand.Next(0, 2);
        }

        public override void AI()
        {
            Projectile.Opacity = (float)Math.Clamp(Projectile.ai[0], 0, 20)/20f;
            Projectile.ai[0]++;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Bottom + new Vector2(0, Main.rand.NextFloat(0f, 3f)), DustID.Corruption, new Vector2((float)Main.rand.NextFloat(-0.8f, 0.8f), (float)Main.rand.NextFloat(-0.6f, -0.3f)));
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Top, new Vector2(Main.rand.NextFloat(-2f, 2f), -10f), ModContent.ProjectileType<MyceliumSpore>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Main.rand.Next(-15, 15));

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Corruption);
            }
            for (int i = 0; i < Main.rand.Next(1,2); i++)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(0.3f, 0.3f), Mod.Find<ModGore>("MyceliumShroom" + "{i}").Type);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityVanilla/Content/Projectiles/Magic/MyceliumShroom").Value;
            float stretch = Projectile.ai[0] < 40 ? (float)(Math.Sin(Projectile.ai[0] * 6) / Math.Pow((Projectile.ai[0] / 8), 3) + 0.95f) : 1;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center + new Vector2(0,19) - Main.screenPosition,
                new Rectangle(0, 42*Projectile.frame, 38, 42),
                Color.White * Projectile.Opacity,
                Projectile.rotation,
                new Vector2(texture.Width/2, texture.Height/3),
                new Vector2(Math.Clamp(stretch, 0f, 3f), stretch),
                Projectile.spriteDirection == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
                );
            
            return false;
        }
    }
}
