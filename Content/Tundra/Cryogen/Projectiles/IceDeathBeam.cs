using CalamityVanilla.Common.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
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

namespace CalamityVanilla.Content.Tundra.Cryogen.Projectiles;

public class IceDeathBeam : ModProjectile
{
    SlotId _sound;
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 8;
        Projectile.hostile = true;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 40;
        Projectile.timeLeft = 4300;
    }
    public override void OnKill(int timeLeft)
    {
        //if (SoundEngine.TryGetActiveSound(_sound, out ActiveSound sound) && sound != null && sound.IsPlaying)
        //{
        //    sound.Stop();
        //}
    }
    public override void AI()
    {
        if (Projectile.ai[1] == 0)
        {
            _sound = SoundEngine.PlaySound(Assets.Sounds.CryogenDeathLaser.Asset, Projectile.Center);
        }
        NPC owner = Main.npc[(int)Projectile.ai[0]];
        if (!owner.active)
            Projectile.Kill();
        Projectile.Center = owner.Center;
        Projectile.ai[1]++;
        int fadeTime = 600;
        if (Projectile.ai[1] < fadeTime) 
            Projectile.Opacity = Projectile.ai[1] / fadeTime;
        else if (Projectile.ai[1] - 300 > (4000 - fadeTime))
        {
            Projectile.Opacity = Projectile.timeLeft / (float)fadeTime;
        }
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
        int dustType = ModContent.DustType<SimpleColorableGlowyDust>();
        Color dustColor = Cryogen.GetAuroraColor((int)(Main.timeForVisualEffects * 3)) with { A = 128 };
        if (Projectile.numUpdates % 10 == 0 || Math.Abs(oldRot - Projectile.rotation) > MathHelper.ToRadians(1))
        {
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, dustType);
                d.color = dustColor;
                d.noGravity = true;
                d.fadeIn = Main.rand.NextFloat(-1, 2);
            }
            for (int i = 0; i < Projectile.velocity.Length() / 64; i++)
            {
                Dust d2 = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(), dustType);
                d2.color = dustColor * 0.5f;
                d2.noGravity = true;
                d2.velocity *= 0.5f;
                d2.velocity += Vector2.UnitX.RotatedBy(Projectile.rotation) * -3;
            }
        }
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        Utils.DrawLaser(Main.spriteBatch, TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, Projectile.Center + Projectile.velocity - Main.screenPosition, new Vector2(Projectile.Opacity, 1f), LaserDraw);

        var tex = TextureAssets.Extra[ExtrasID.ThePerfectGlow].Value;
        DrawData d = new(tex,Projectile.Center - Main.screenPosition,null, Cryogen.GetAuroraColor((int)(Main.timeForVisualEffects * 3)) with { A = 0 } * Projectile.Opacity,(float)(Main.timeForVisualEffects * 0.025f),tex.Size()/2, new Vector2(1,3) * (float)(Math.Sin(Main.timeForVisualEffects * 0.1f) * 0.2f + 1f), SpriteEffects.None);
        
        for(int i = 0; i < 3; i++)
        {
            d.rotation += MathHelper.Pi / 3f;
            Main.EntitySpriteDraw(d);
            Main.EntitySpriteDraw(d with { color = d.color * 0.25f, scale = d.scale * new Vector2(3,1.5f)});
        }

        var ring = TextureAssets.Extra[ExtrasID.KeybrandRing].Value;
        float ringGlow = Utils.Remap(Projectile.timeLeft,1000,4600,0,1);
        ringGlow *= ringGlow * ringGlow;
        Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, null, d.color * ringGlow * 2, Projectile.rotation, ring.Size() / 2, 3 - ringGlow * 3, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, null, d.color * ringGlow * 4, Projectile.rotation, ring.Size() / 2, 1.5f - ringGlow * 1.5f, SpriteEffects.None, 0);
        return false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity, Projectile.Opacity * 32, ref collisionPoint);
    }
    public static void LaserDraw(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distCovered, out Rectangle frame, out Vector2 origin, out Color color)
    {
        color = Cryogen.GetAuroraColor((int)(Main.timeForVisualEffects * 3)) with { A = 128 };
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