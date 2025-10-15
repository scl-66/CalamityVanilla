using CalamityVanilla.Content.Bosses.Cryogen;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Miscellaneous.Items.Weapons.Ranger.ThrowingBrick;

public class ThrowingBrick : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }
    public override void SetDefaults()
    {
        Item.DefaultToThrownWeapon(ModContent.ProjectileType<ThrowingBrickProjectile>(), 30, 10, true);
        Item.noUseGraphic = true;
        Item.damage = 16;
        Item.knockBack = 7;
        Item.rare = ItemRarityID.White;
        Item.consumable = true;
        Item.maxStack = 9999;
        Item.UseSound = SoundID.Item1;
        Item.value = 0;
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).AddIngredient(ItemID.ClayBlock, 1).Register();
        CreateRecipe(8).AddIngredient(ItemID.RedBrick, 1).Register();
    }
}

public class ThrowingBrickProjectile : ModProjectile
{
    public override string Texture => ModContent.GetModItem(ModContent.ItemType<ThrowingBrick>()).Texture;
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 24);
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item127 with { PitchVariance = 0.5f }, Projectile.position);
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), DustID.Clay, Main.rand.NextVector2Circular(2, 2));
            d.noGravity = !Main.rand.NextBool(3);
            d.velocity -= Projectile.velocity * 0.2f;
        }
    }
    public override void AI()
    {
        Projectile.rotation += 0.4f * Projectile.velocity.X / 16f;
        Projectile.ai[1] += 1f; // Use a timer to wait 15 ticks before applying gravity.
        if (Projectile.ai[1] >= 5f)
        {
            Projectile.ai[1] = 5f;
            Projectile.velocity.Y += 0.5f;
            Projectile.velocity.X *= 0.98f;
        }

        if (Collision.SolidCollision(Projectile.position - new Vector2(Projectile.width / 1.5f, Projectile.height / 1.5f), (int)(Projectile.width * 1.5f), (int)(Projectile.height * 1.5f)))
        {
            OnTileCollide(Projectile.velocity);
        }

        // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster.
        // Past 16f this projectile will travel through blocks, so this check is useful.
        if (Projectile.velocity.Y > 16f)
        {
            Projectile.velocity.Y = 16f;
        }
        if (Math.Abs(Projectile.velocity.X) > 16f) Projectile.velocity.X = 16f * Math.Sign(Projectile.velocity.X);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        bool hitTile = false;
        int rand = Main.rand.Next(1, 3);
        Point tileCoord = Projectile.Center.ToTileCoordinates();
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                if (Main.tile[tileCoord.X + i, tileCoord.Y + j].TileType == ModContent.TileType<CryogenIceTile>() || Main.tile[tileCoord.X + i, tileCoord.Y + j].TileType is TileID.Glass or TileID.BreakableIce or TileID.MagicalIceBlock or TileID.Waterfall or TileID.Lavafall or TileID.Honeyfall or TileID.SandFallBlock or TileID.Confetti or TileID.ConfettiBlack or TileID.BlueStarryGlassBlock or TileID.GoldStarryGlassBlock or TileID.SnowFallBlock)
                {
                    // create liquid if tile destroyed is a liquidfall block
                    Tile tile = Main.tile[tileCoord.X + i, tileCoord.Y + j];
                    byte amt = 100;
                    switch (Main.tile[tileCoord.X + i, tileCoord.Y + j].TileType)
                    {
                        case TileID.Waterfall:
                            tile.LiquidType = LiquidID.Water;
                            tile.LiquidAmount = amt;
                            Liquid.AddWater(tileCoord.X + i, tileCoord.Y + j);
                            if (Main.netMode == NetmodeID.Server) { NetMessage.sendWater(tileCoord.X + i, tileCoord.Y + j); }
                            break;
                        case TileID.Lavafall:
                            tile.LiquidType = LiquidID.Lava;
                            tile.LiquidAmount = amt;
                            Liquid.AddWater(tileCoord.X + i, tileCoord.Y + j);
                            if (Main.netMode == NetmodeID.Server) { NetMessage.sendWater(tileCoord.X + i, tileCoord.Y + j); }
                            break;
                        case TileID.Honeyfall:
                            tile.LiquidType = LiquidID.Honey;
                            tile.LiquidAmount = amt;
                            Liquid.AddWater(tileCoord.X + i, tileCoord.Y + j);
                            if (Main.netMode == NetmodeID.Server) { NetMessage.sendWater(tileCoord.X + i, tileCoord.Y + j); }
                            break;
                    }

                    // normal item drop code will not run if tile is a liquidfall block
                    bool isLiquidFall = false;
                    if (Main.tile[tileCoord.X + i, tileCoord.Y + j].TileType is TileID.Waterfall or TileID.Lavafall or TileID.Honeyfall)
                    {
                        isLiquidFall = true;
                    }


                    Vector2 worldCoord = new Vector2(tileCoord.X + i, tileCoord.Y + j).ToWorldCoordinates();
                    EntitySource_TileBreak source = new EntitySource_TileBreak(tileCoord.X + i, tileCoord.Y + j);

                    // 100% chance to break inner radius
                    if (Math.Abs(i) < 2 && Math.Abs(j) < 2)
                    {
                        WorldGen.KillTile(tileCoord.X + i, tileCoord.Y + j, noItem: isLiquidFall);
                        if (isLiquidFall)
                        {
                            int item = Item.NewItem(source, worldCoord, 1, 1, ItemID.Glass);
                            //Main.item[item].rare = ItemRarityID.Blue;
                            //ItemID.Sets.IsLavaImmuneRegardlessOfRarity[item] = true;
                        }
                    }
                    else
                    {
                        // chance to break outer radius
                        if (Main.rand.Next(0, 10) > 3)
                        {
                            WorldGen.KillTile(tileCoord.X + i, tileCoord.Y + j, noItem: isLiquidFall);
                            if (isLiquidFall) { Item.NewItem(source, worldCoord, 1, 1, ItemID.Glass); }
                        }
                    }

                    Projectile.velocity = Projectile.oldVelocity;
                    if (Projectile.ai[0] < rand && !hitTile)
                    {
                        Projectile.ai[0]++;
                        hitTile = true;
                        return false;
                    }
                }
            }
        }
        Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
        return true;
    }
}