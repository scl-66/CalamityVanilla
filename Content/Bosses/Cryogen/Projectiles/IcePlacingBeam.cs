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

namespace CalamityVanilla.Content.Bosses.Cryogen.Projectiles;

public class IcePlacingBeam : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 8;
        Projectile.hostile = true;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 90;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 1;
    }
    public override void AI()
    {
        NPC owner = Main.npc[(int)Projectile.ai[0]];
        if (!owner.active)
            Projectile.Kill();
        Projectile.Center = owner.Center;
        Vector2 ep = EndPoint();
        for (int i = 0; i < 12; i++)
        {
            //Dust d = Dust.NewDustPerfect(ep + Main.rand.NextVector2Circular(16,16),DustID.FrostStaff);
            //d.noGravity = true;
            //d.velocity *= 2;
            //d.scale = 1.25f;
            Dust d = Dust.NewDustPerfect(ep + Main.rand.NextVector2Circular(16, 16), DustID.SnowSpray);
            d.velocity = Main.rand.NextVector2Circular(3, 5);
            d.noGravity = true;
            d.fadeIn = Main.rand.NextFloat(1.5f);
        }
        foreach (Player p in Main.ActivePlayers)
        {
            if (p.Center.Distance(ep) < 16 * 3)
            {
                Vector2 dir = p.Center.DirectionTo(Projectile.velocity);
                p.velocity = dir * 5;
                p.position += dir * 32;
            }
        }
        int halfwidth = 1;
        Point epPoint = ep.ToTileCoordinates();
        int type = ModContent.TileType<CryogenIceTile>();
        for (int x = -halfwidth; x <= halfwidth; x++)
        {
            for (int y = -halfwidth; y <= halfwidth; y++)
            {
                CryogenIceBlockSystem.PlaceIceBlock(epPoint.X + x, epPoint.Y + y);
            }
        }
    }
    public override bool ShouldUpdatePosition() => false;
    private Vector2 EndPoint()
    {
        float percent = Projectile.timeLeft / 90f;
        return Projectile.velocity + new Vector2(0, Projectile.ai[1]).RotatedBy(percent * MathHelper.TwoPi * Projectile.ai[2]);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Utils.DrawLaser(Main.spriteBatch, TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, EndPoint() - Main.screenPosition, new Vector2(0.5f, 1f), LaserDraw);
        return false;
    }
    public static void LaserDraw(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distCovered, out Rectangle frame, out Vector2 origin, out Color color)
    {
        color = Color.White;
        switch (stage)
        {
            case 0:
                distCovered = 0f;
                frame = new Rectangle(0, 0, 21, 8);
                origin = frame.Size() / 2f;
                break;
            case 1:
                frame = new Rectangle(0, 8, 21, 6);
                distCovered = frame.Height;
                origin = new Vector2(frame.Width / 2, 0f);
                break;
            case 2:
                distCovered = 8f;
                frame = new Rectangle(0, 14, 21, 8);
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