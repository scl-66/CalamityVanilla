using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Desert.Items.ForsakenSaber;

public class ForsakenSaber : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        Item.Size = new Vector2(54, 54);
        Item.damage = 67;
        Item.knockBack = 6f;
        Item.useTime = 21;
        Item.useAnimation = 21;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item1;
        Item.DamageType = DamageClass.Melee;
        Item.rare = ItemRarityID.Pink;
        //Item.value = Item.sellPrice(0, 2, 10, 0);
        Item.shoot = ModContent.ProjectileType<ForsakenSaberBoulder>();
        Item.shootSpeed = 19;
    }
}

public class ForsakenSaberBoulder : ModProjectile
{
    public int lerp;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 7;
    }
    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = -1;
        Projectile.aiStyle = -1;
    }
    public override void OnKill(int timeLeft)
    {   
        Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(1, 0), 11, 0.6f);
        Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(-1, 0), 11, 0.6f);
        SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);

        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Top, Vector2.Zero, ModContent.ProjectileType<ForsakenSaberPillar>(), Projectile.damage / 4, Projectile.knockBack, Main.myPlayer);
        
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        width = 32;
        height = 32;
        return true;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        Projectile.velocity.Y += MathHelper.Lerp(0.15f, 0.45f, lerp++ * 0.07f);
        Projectile.velocity.X *= 0.98f;
        Projectile.rotation += 0.21f;

        if (Projectile.Center.Y <= player.Center.Y + 10)
        {
            Projectile.tileCollide = false;
        }
        else
            Projectile.tileCollide = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
        {
            Color color = Color.Yellow;
            color *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
            Rectangle rect = new(0, 0, 32, 32);
            Vector2 pos = Projectile.Center - Projectile.Size + Projectile.velocity;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.oldPos[i] + (Projectile.Size / 2) - Main.screenPosition, rect, color, Projectile.oldRot[i], rect.Size() / 2f, Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
        }
        return base.PreDraw(ref lightColor);
    }
}

public class ForsakenSaberPillar : ModProjectile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
    }
    public override void SetDefaults()
    {
        Projectile.width = 120;
        Projectile.height = 58;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = -1;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 120;
    }
    public override void OnSpawn(IEntitySource source)
    {
        Projectile.ai[0] = Main.rand.NextFloat(0.8f, 1.2f);
        Projectile.ai[1] = Main.rand.Next(1, 4);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity *= 0;
        Projectile.position.Y -= 0.3f;
        return false;
    }
    public override void AI()
    {
        Projectile.velocity.Y += 0.3f;
    }
    float mult;
    int lerp;

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        mult = MathHelper.Lerp(0, 1, ++lerp * 0.07f);
        if (mult >= 1)
            mult = 1;

        Vector2 scale = new(1, 1 * mult);
        for (int i = -(int)Projectile.ai[0]; i <= Projectile.ai[0]; i++)
        {
            Main.NewText(i);
            Main.NewText(Projectile.frame);
            switch (i)
            {
                case 0: Projectile.frame = 0; break;
                case 1 or -1: Projectile.frame = 1; break;
                case 2 or -2: Projectile.frame = 2; break;
            }

            Rectangle rect = new(0, texture.Height / 3 * Projectile.frame, texture.Width, texture.Height / 3);
            Vector2 origin = rect.Size() / 2f;

            float rot = Projectile.rotation + (i * Projectile.ai[0]);

            Vector2 pos = Projectile.Center;
            pos -= Main.screenPosition;
            pos.X += i * 15;

            Main.EntitySpriteDraw(texture, pos, rect, Projectile.GetAlpha(lightColor), rot, new(texture.Width / 2, texture.Height / 5), scale, SpriteEffects.None);
        }
        return false;
    }
}
