using CalamityVanilla.Common;
using CalamityVanilla.Content.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles.Hostile
{
    public class CryogenIceBlock : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.QuickDefaults(true, 32);
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.direction * 0.1f;
            if (Projectile.Center.Distance(new Vector2(Projectile.ai[0], Projectile.ai[1])) < Projectile.velocity.Length())
            {
                Projectile.Kill();
            }
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceRod);
            d.velocity = Projectile.velocity;
            d.noGravity = true;
            d.scale = 1.5f;
        }
        public override void OnKill(int timeLeft)
        {
            Point placePos = Projectile.Center.ToTileCoordinates();
            //WorldGen.PlaceTile(placePos.X, placePos.Y, TileID.MagicalIceBlock);
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            switch (Projectile.ai[2])
            {
                case 0:

                    int squaresize = 2;

                    for (int x = -squaresize; x <= squaresize; x++)
                    {
                        for (int y = -squaresize; y <= squaresize; y++)
                        {
                            WorldGen.PlaceTile(placePos.X + x, placePos.Y + y, ModContent.TileType<CryogenIceTile>(), plr: Main.myPlayer);
                            CryogenIceBlockSystem.CryogenIceBlocks.Add(new Vector3(placePos.X + x, placePos.Y + y,CryogenIceBlockSystem.DEFAULT_ICE_TIMER + Main.rand.Next(0,60)));
                        }
                    }
                    NetMessage.SendTileSquare(-squaresize, placePos.X - squaresize, placePos.Y - 1,(squaresize*2) + 1, (squaresize * 2) + 1);
                    break;
            }
        }
    }
}
