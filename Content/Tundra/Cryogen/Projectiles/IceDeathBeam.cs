using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Cryogen.Projectiles;

public class IceDeathBeam : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 8;
        Projectile.hostile = true;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 40;
    }
    public override void AI()
    {
        NPC owner = Main.npc[(int)Projectile.ai[0]];
        if (!owner.active || Projectile.rotation >= MathHelper.TwoPi * 2)
            Projectile.Kill();
        Projectile.Center = owner.Center;
        Projectile.ai[1]++;
        Projectile.Opacity = Projectile.ai[1] / 150;
        float oldRot = Projectile.rotation;
        Projectile.rotation = MathHelper.SmoothStep(0, MathHelper.TwoPi * 2, Utils.Remap(Projectile.ai[1] - 300, 0, 4000, 0, 1));
        //Projectile.velocity = Vector2.UnitY.RotatedBy(Projectile.rotation) * 100;

        float[] samples = new float[3];
        Collision.LaserScan(Projectile.Center, (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2(), 32, 2400, samples);
        float length = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            length += samples[i];
        }
        length /= 3f;

        Projectile.velocity = Vector2.UnitY.RotatedBy(Projectile.rotation) * length;
        if (Projectile.numUpdates % 10 == 0 || Math.Abs(oldRot - Projectile.rotation) > MathHelper.ToRadians(1))
        {
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustID.FrostHydra);
                d.noGravity = true;
            }
            for (int i = 0; i < Projectile.velocity.Length() / 64; i++)
            {
                Dust d2 = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(), DustID.FrostHydra);
                d2.noGravity = true;
            }
        }
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        Utils.DrawLaser(Main.spriteBatch, TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, Projectile.Center + Projectile.velocity - Main.screenPosition, new Vector2(Projectile.Opacity, 1f), LaserDraw);
        return false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity, Projectile.Opacity * 32, ref collisionPoint);
    }
    public static void LaserDraw(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distCovered, out Rectangle frame, out Vector2 origin, out Color color)
    {
        color = Color.White;
        switch (stage)
        {
            case 0:
                distCovered = 0f;
                frame = new Rectangle(0, 0, 52, 8);
                origin = frame.Size() / 2f;
                break;
            case 1:
                frame = new Rectangle(0, 8, 52, 6);
                distCovered = frame.Height;
                origin = new Vector2(frame.Width / 2, 0f);
                break;
            case 2:
                distCovered = 8f;
                frame = new Rectangle(0, 14, 52, 8);
                origin = new Vector2(frame.Width / 2, 2f);
                color = Color.Transparent;
                break;
            default:
                distCovered = 9999f;
                frame = Rectangle.Empty;
                origin = Vector2.Zero;
                color = Color.Transparent;
                break;
        }
    }
}