using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityVanilla.Content.Underground.Items.MarbleTome;

public class MarbleTome : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<MarbleTomePillar>(), 50, 0);
        Item.width = 28;
        Item.height = 30;
        Item.UseSound = SoundID.Item69;
        // Set damage and knockBack
        Item.SetWeaponValues(50, 10);
        Item.mana = 20;
        // Set rarity and value
        Item.SetShopValues(ItemRarityColor.Blue1, 3000);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        foreach (var projectile in Main.ActiveProjectiles)
        {
            if (projectile.owner == player.whoAmI && projectile.type == type)
            {
                projectile.Kill();
            }
        }
        if (player == Main.LocalPlayer)
        {
            Main.LocalPlayer.FindSentryRestingSpot(type, out int WorldX, out int WorldY, out int PushUpY);
            //check if mouse is within radius of player, if not, spawn pillar at closest point in circle to mouse
            Vector2 pos = new Vector2(WorldX, WorldY + ContentSamples.ProjectilesByType[type].height / 2 + 16);
            Vector2 dist = (player.Center - pos);
            float distLen = player.Center.X - pos.X;
            Vector2 distNorm = player.Center - dist;
            float radius = 300f;
            if (Math.Abs(distLen) > radius)
            {
                distNorm = player.Center - dist.SafeNormalize(Vector2.UnitX) * radius;
            }
            //check if mouse is inside blocks
            Point mouse = Main.MouseWorld.ToTileCoordinates();
            Tile tile = Main.tile[mouse.X, mouse.Y];
            if (tile.HasUnactuatedTile)
            {
                bool foundAir = false;
                for (int i = mouse.Y; i > mouse.Y - 40; i--)
                {
                    if (!Main.tile[mouse.X, i].HasUnactuatedTile)
                    {
                        pos.Y = i * 16 + ContentSamples.ProjectilesByType[type].height / 2 + 32;
                        foundAir = true;
                        break;
                    }
                }
                if (!foundAir)
                {
                    return false;
                }
            }
            Projectile.NewProjectileDirect(source, new Vector2(distNorm.X, pos.Y), new Vector2(0, -25), type, (int)(damage / 1.5), knockback, player.whoAmI, -3);
        }
        return false;
    }
}

public class MarbleTomePillar : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
    }
    public override void SetDefaults()
    {
        Projectile.width = 36;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 60*8;
        Projectile.tileCollide = false;
        Projectile.penetrate = 7;
        Projectile.Opacity = 0f;
        Projectile.hide = true;
        DrawOffsetX = -6;
    }

    public override void AI()
    {
        Projectile.ai[0]++;

        if (Projectile.ai[0] == 1)
        {
            int tX = Projectile.Bottom.ToTileCoordinates().X;
            int tY = Projectile.Bottom.ToTileCoordinates().Y - 5;
            Tile tile = Framing.GetTileSafely(tX, tY);
            for (int i = 0; i < 25; i++)
            {
                //Dust d = Dust.NewDustDirect(Projectile.position + new Vector2(0, 15), Projectile.width, Projectile.height / 3, DustID.Marble);
                //d.velocity = Main.rand.NextVector2Unit(MathHelper.PiOver4, MathHelper.PiOver2 + MathHelper.PiOver4).RotatedBy(MathHelper.Pi - MathHelper.PiOver2 / 3) * Main.rand.NextFloat(1f, 4f);
                //d.scale = Main.rand.NextFloat(0.5f, 1.5f);
                Dust d = Main.dust[WorldGen.KillTile_MakeTileDust(tX, tY, tile)];
                d.velocity = Main.rand.NextVector2Unit(MathHelper.PiOver4, MathHelper.PiOver2 + MathHelper.PiOver4).RotatedBy(MathHelper.Pi - MathHelper.PiOver2 / 3) * Main.rand.NextFloat(1f, 4f);
                d.scale = Main.rand.NextFloat(0.5f, 1.5f);
            }
        }

        if (Projectile.ai[0] < 20)
        {
            Projectile.Opacity += 0.05f;
            Projectile.velocity.Y *= 0.84f;
        } else
        {
            Projectile.alpha = 0;
            Projectile.velocity.Y = 0;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.HitDirectionOverride = Math.Sign(target.Center.X - Projectile.Center.X);
        if (Projectile.ai[0] > 5)
        {
            modifiers.SourceDamage *= 0.2f;
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCsAndTiles.Add(index);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(SoundID.Tink with { Volume = 0.8f });
        int rand = Main.rand.Next(2, 6);
        for (int i = 0; i < rand; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.Hitbox.ClosestPointInRect(target.position), Projectile.width, Projectile.height, DustID.Marble);
            d.velocity = Main.rand.NextVector2Circular(1, 1);
            d.scale = Main.rand.NextFloat(0.5f, 1);
            //d.noGravity = !Main.rand.NextBool(3);
        }
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.position);

        int goreMax = Main.rand.Next(3, 5);
        for (int i = 0; i < goreMax; i++)
        {
            int g = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(6, 6), Mod.Find<ModGore>("MarbleTomePillarGore" + $"{i + 1}").Type);
            Main.gore[g].timeLeft = 5;
        }

        for (int i = 0; i < 40; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Marble);
            d.velocity = Main.rand.NextVector2Circular(6, 6);
            d.scale = Main.rand.NextFloat(0.5f, 1.5f);
            //d.noGravity = !Main.rand.NextBool(3);
        }
    }
}