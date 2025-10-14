using CalamityVanilla.Common;
using CalamityVanilla.Content.Bosses.Cryogen;
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

namespace CalamityVanilla.Content.Bosses.Cryogen;

public class IcePillarSpawner : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 16);
        Projectile.tileCollide = false;
        Projectile.timeLeft = 120;
        Projectile.tileCollide = false;
    }
    public override bool ShouldUpdatePosition()
    {
        return true;
    }
    public override void AI()
    {
        float percent = Projectile.timeLeft / 120f;
        Projectile.position.X = MathHelper.Lerp(Projectile.ai[0], Projectile.velocity.X, percent) + Projectile.width;
        Projectile.position.Y = MathHelper.Lerp(Projectile.ai[1], Projectile.velocity.Y, percent) + Projectile.height;
        Projectile.position -= Projectile.velocity;
        Projectile.position.Y -= MathF.Sin(percent * MathHelper.Pi) * 16 * 20;
        Projectile.rotation += Projectile.direction * 0.1f;
    }
    public override void OnKill(int timeLeft)
    {
        if (Main.myPlayer == Projectile.owner)
        {
            for (int i = 0; i < 35; i++)
            {
                Vector2 BlockPlacement = Projectile.Center + new Vector2(Main.rand.Next(-i, i) * 2, (i * -32) + 32);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.Center.DirectionTo(BlockPlacement) * 8, ModContent.ProjectileType<CryogenIceBlockInvisible>(), 0, 0, -1, BlockPlacement.X, BlockPlacement.Y, CryogenIceBlockSystem.DEFAULT_ICE_TIMER * 2);
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> tex = TextureAssets.Projectile[Type];
        for (int i = 0; i < 10; i++)
        {
            Main.spriteBatch.Draw(tex.Value, (Projectile.oldPos[i] + Projectile.Size / 2) - Main.screenPosition, null, (Cryogen.GetAuroraColor((int)(Projectile.whoAmI * 25 + (Projectile.timeLeft * 5)) + (i * 15)) * (1f - (i / 10f))) with { A = 0 }, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }
        Main.spriteBatch.Draw(tex.Value, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}
public class CryogenIceBlockInvisible : CryogenIceBlock
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hide = true;
    }
    public override void AI()
    {
        if (Projectile.Center.Distance(new Vector2(Projectile.ai[0], Projectile.ai[1])) < Projectile.velocity.Length() || CryogenIceBlockSystem.CryogenIceBlocks.Count > 830)
        {
            Projectile.Kill();
        }
    }
}