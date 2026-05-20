using Daybreak.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Crimson.Items.DentataWand;

public class DentataWand : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToStaff(ModContent.ProjectileType<Dentata>(), 6f, 24, 6);

        // Set damage and knockBack
        Item.SetWeaponValues(30, 4);
        Item.UseSound = SoundID.NPCHit13;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.channel = true;

        // Set rarity and value
        Item.SetShopValues(ItemRarityColor.Blue1, 2000);
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddTile(TileID.Anvils)
            .AddIngredient(ItemID.CrimtaneBar, 8)
            .AddIngredient(ItemID.TissueSample, 6)
            .Register();
    }

    public override bool CanUseItem(Player player)
    {
        if (player.ownedProjectileCounts[ModContent.ProjectileType<Dentata>()] < 1)
            return true;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type == ModContent.ProjectileType<Dentata>() && proj.owner == player.whoAmI && proj.ai[0] == 0)
            {
                return false;
            }
        }
        return true;
    }
}

public class Dentata : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 5;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 20);
        Projectile.DamageType = DamageClass.Magic;
        Projectile.tileCollide = true;
        DrawOffsetX = -4;
        DrawOriginOffsetY = -4;
    }

    public override void AI()
    {
        if (++Projectile.frameCounter >= 5)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }

        if (Main.rand.NextBool(2))
        {
            Vector2 dustPosition = Projectile.position + new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, 5));
            Vector2 vel = Main.rand.NextVector2Circular(Projectile.width / 4, Projectile.height / 8) + new Vector2(0, 2);
            Dust dust = Dust.NewDustDirect(dustPosition, Projectile.width, Projectile.height, DustID.Blood, vel.X, vel.Y);
            dust.velocity.X *= 0.3f;
            dust.noGravity = false;
            dust.scale = Main.rand.NextFloat(0.85f, 1.2f);
        }

        // In Multi Player (MP) This code only runs on the client of the projectile's owner, this is because it relies on mouse position, which isn't the same across all clients.
        if (Main.myPlayer == Projectile.owner)
        {
            float maxDistance = 12f; // This also sets the maximun speed the projectile can reach while following the cursor.
            Player player = Main.player[Projectile.owner];
            // If the player channels the weapon, do something. This check only works if item.channel is true for the weapon.
            if (player.channel && player.HeldItem.shoot == Type && Projectile.ai[0] == 0f && (player.Center - Projectile.Center).Length() < 500f)
            {
                Vector2 vectorToCursor = Main.MouseWorld - Projectile.Center;
                float distanceToCursor = vectorToCursor.Length();

                // Here we can see that the speed of the projectile depends on the distance to the cursor.
                if (distanceToCursor > maxDistance)
                {
                    distanceToCursor = maxDistance / distanceToCursor;
                    vectorToCursor *= distanceToCursor;
                }

                int velocityXBy1000 = (int)(vectorToCursor.X * 1000f);
                int oldVelocityXBy1000 = (int)(Projectile.velocity.X * 1000f);
                int velocityYBy1000 = (int)(vectorToCursor.Y * 1000f);
                int oldVelocityYBy1000 = (int)(Projectile.velocity.Y * 1000f);

                // This code checks if the precious velocity of the projectile is different enough from its new velocity, and if it is, syncs it with the server and the other clients in MP.
                // We previously multiplied the speed by 1000, then casted it to int, this is to reduce its precision and prevent the speed from being synced too much.
                if (velocityXBy1000 != oldVelocityXBy1000 || velocityYBy1000 != oldVelocityYBy1000)
                {
                    Projectile.netUpdate = true;
                }

                Projectile.velocity = vectorToCursor;

            }
            // If the player stops channeling, do something else.
            else if (Projectile.ai[0] == 0f)
            {
                // This code block is very similar to the previous one, but only runs once after the player stops channeling their weapon.
                Projectile.netUpdate = true;
                Vector2 vectorToCursor = Main.MouseWorld - Projectile.Center;
                float distanceToCursor = vectorToCursor.Length();

                //If the projectile was at the cursor's position, set it to move in the oposite direction from the player.
                if (distanceToCursor == 0f)
                {
                    vectorToCursor = Projectile.Center - player.Center;
                    distanceToCursor = vectorToCursor.Length();
                }

                distanceToCursor = maxDistance / distanceToCursor;
                vectorToCursor *= distanceToCursor;
                Projectile.velocity = vectorToCursor;

                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.Kill();
                }

                Projectile.ai[0] = 1f;
            }

            if (Projectile.ai[0] == 1f)
            {
                Projectile.velocity.Y += 0.5f;
                Projectile.velocity.X *= 0.99f;

                if (Projectile.velocity.Y > 16f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
                {
                    Projectile.velocity.Y = 16f;
                }
            }
        }

        // Set the rotation so the projectile points towards where it's going.
        if (Projectile.velocity != Vector2.Zero)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
        SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.position);
        for (int i = 0; i < 10; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 0, Color.White, 1f);
            dust.noGravity = false;
            dust.velocity *= 2f;
            dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 0, Color.White, 1.4f);
        }

        for (int i = 0; i < 20; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.Crimson);
            dust.noGravity = false;
            dust.velocity *= 2f;
            dust.scale = Main.rand.NextFloat(0.75f, 1.1f);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Rectangle frame = tex.Frame(1, 5, 0, Projectile.frame);
        if (Main._multiplyBlendState == null)
        {
            Main._multiplyBlendState = new BlendState
            {
                ColorBlendFunction = BlendFunction.ReverseSubtract,
                ColorDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.SourceColor,
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                AlphaDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.SourceColor
            };
        }
        Main.spriteBatch.End(out var ss);
        Main.spriteBatch.Begin(ss with
        {
            BlendState = Main._multiplyBlendState
        });

        Vector2 origin = frame.Size() / 2 + new Vector2(0, 2);
        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos = (Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition);
            Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 1.15f;
            Main.EntitySpriteDraw(tex, drawPos, frame, (color * 1f).MultiplyRGBA(Color.Cyan), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
        }
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(ss);
        return true;
    }
}
