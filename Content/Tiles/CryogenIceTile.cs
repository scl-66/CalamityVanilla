using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityVanilla.Content.Tiles
{
    public class CryogenIceTile : ModTile
    {
        public override string Texture => $"Terraria/Images/Tiles_{TileID.IceBlock}";
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            DustType = DustID.IceRod;
            HitSound = SoundID.Item27;
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.rand.NextBool(30))
            {
                Dust d = Dust.NewDustDirect(new Vector2(i, j) * 16, 16, 16, DustType, 0, 0, 128);
                d.noGravity = false;
                d.velocity *= 0.2f;
            }
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            base.PlaceInWorld(i, j, item);
        }
    }
}
