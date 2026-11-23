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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Crimson.GutOfCthulhu.Projectiles;

public class GutShockwave : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 6;
    }
    public override void SetDefaults()
    {
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.width = 60;
        Projectile.height = 150;
    }
    public override void AI()
    {
        if (Projectile.ai[1] == 0)
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
        }
        Projectile.ai[1]++;
        Projectile.frameCounter++;
        if(Projectile.frameCounter > 3)
        {
            Projectile.frame++;
            Projectile.frameCounter = 0;
            if (Projectile.owner == Main.myPlayer && Projectile.frame == 3 && Projectile.ai[0] != 0) 
            {
                int dir = Math.Sign(Projectile.ai[0]);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), CVUtils.FindNearbySpotOnGround(Projectile.Center + Vector2.UnitX * 60 * dir, 32) - (Vector2.UnitY * ContentSamples.ProjectilesByType[Type].height / 2), Vector2.Zero, Type, Projectile.damage, 1, -1, Projectile.ai[0] - dir);
            }
            else if (Projectile.frame > 6)
                Projectile.Kill();
        }
    }
    public override void OnKill(int timeLeft)
    {
        
    }
    public override bool PreDraw(ref Color lightColor)
    {
        var tex = TextureAssets.Projectile[Type].Value;
        var frame = tex.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
        Main.EntitySpriteDraw(tex,Projectile.Bottom - Main.screenPosition, frame, lightColor,Projectile.rotation, new Vector2(frame.Width / 2, frame.Height - 4), 1, SpriteEffects.None);
        return false;
    }
}
