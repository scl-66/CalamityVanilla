using CalamityVanilla.Common;
using CalamityVanilla.Content.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.Cryogen;

public class CryogenIceBlock : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(true, 32);
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 2;
    }
    public override void AI()
    {
        Projectile.rotation += Projectile.direction * 0.1f;
        if (Projectile.Center.Distance(new Vector2(Projectile.ai[0], Projectile.ai[1])) < Projectile.velocity.Length() || CryogenIceBlockSystem.CryogenIceBlocks.Count > 830)
        {
            Projectile.Kill();
            SoundEngine.PlaySound(SoundID.NPCDeath15, Projectile.position);
        }
        Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceRod);
        d.velocity = Projectile.velocity;
        d.noGravity = true;
        d.scale = 1.5f;
        Point centerPos = Projectile.Center.ToTileCoordinates();

        if (!Collision.SolidCollision(Projectile.position, 32, 32))
            Projectile.localAI[0] = 1;
        if (Projectile.localAI[0] == 0)
            return;

        if (Projectile.Center.HasNaNs() || centerPos.X < 0 || centerPos.Y < 0 || !WorldGen.InWorld(centerPos.X, centerPos.Y))
            return;
        if (Main.tile[centerPos].HasTile && !Main.tileSolidTop[Main.tile[centerPos].TileType] && Main.tileSolid[Main.tile[centerPos].TileType] && Main.tile[centerPos].TileType != ModContent.TileType<CryogenIceTile>())
        {
            SoundEngine.PlaySound(SoundID.NPCDeath15, Projectile.position);
            Projectile.Kill();
        }
    }
    public override void OnKill(int timeLeft)
    {
        if (timeLeft == 0)
            return;

        Point placePos = Projectile.Center.ToTileCoordinates();
        if (Projectile.Center.HasNaNs() || placePos.X < 0 || placePos.Y < 0 || !WorldGen.InWorld(placePos.X, placePos.Y))
            return;

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            int squaresize = Main.rand.Next(1, 3);

            for (int x = -squaresize; x <= squaresize; x++)
            {
                for (int y = -squaresize; y <= squaresize; y++)
                {
                    if (!Main.rand.NextBool(16))
                    {
                        WorldGen.PlaceTile(placePos.X + x, placePos.Y + y, ModContent.TileType<CryogenIceTile>(), plr: Main.myPlayer);
                        CryogenIceBlockSystem.AddTime(placePos.X + x, placePos.Y + y, (int)Projectile.ai[2] + Main.rand.Next(0, 60));
                    }
                }
            }
            NetMessage.SendTileSquare(-squaresize, placePos.X - squaresize, placePos.Y - 1, (squaresize * 2) + 1, (squaresize * 2) + 1);
        }
    }
}