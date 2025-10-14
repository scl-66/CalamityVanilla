using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Items.MagicChisel;

public class MagicChisel : ModItem
{
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.TwinsPetItem);
        Item.DefaultToVanitypet(ModContent.ProjectileType<MagicChiselPet>(), ModContent.BuffType<MagicChiselBuff>());
    }
    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            player.AddBuff(Item.buffType, 3600);
        }
        return true;
    }
}
public class MagicChiselBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
        Main.lightPet[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        bool unused = false;
        player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<MagicChiselPet>());
    }
}
public class MagicChiselPet : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projPet[Projectile.type] = true;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        ProjectileID.Sets.LightPet[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.penetrate = -1;
        Projectile.netImportant = true;
        Projectile.timeLeft *= 5;
        Projectile.friendly = true;
        Projectile.hide = true;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];

        if (Projectile.timeLeft == ContentSamples.ProjectilesByType[Type].timeLeft)
            Projectile.frame = Main.rand.Next(5);
        if (!owner.active)
        {
            Projectile.active = false;
            return;
        }
        if (!owner.dead && owner.HasBuff(ModContent.BuffType<MagicChiselBuff>()))
        {
            Projectile.timeLeft = 2;
        }
        if (Projectile.Center.Distance(owner.Center) > 2000)
        {
            Projectile.Center = owner.Center;
        }
        else if (Projectile.Center.Distance(owner.Center) > 500)
        {
            Projectile.tileCollide = false;
        }
        else if (owner.velocity.Y == 0 && Projectile.Center.Distance(owner.Center) < 100 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
        {
            Projectile.tileCollide = true;
        }
        if (Projectile.tileCollide)
        {
            Projectile.velocity.Y += 0.4f;
            if (Projectile.Center.Distance(owner.Center) > 100 || !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height + 2))
            {
                Projectile.velocity.X -= Math.Sign(Projectile.Center.X - owner.Center.X) * 0.1f;
            }
            else
            {
                Projectile.velocity *= 0.94f;
            }
            for (int i = 0; i < (Projectile.velocity.Length() / 5f) - 0.25f; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Left, Projectile.width, Projectile.height / 2, DustID.Snow);
                d.velocity = -Projectile.velocity * 0.3f;
                d.velocity.Y = Main.rand.NextFloat(-Projectile.velocity.Length() * 0.3f, 0f);
                d.noGravity = !Main.rand.NextBool(15);
                d.alpha = 255 - (int)(Projectile.velocity.Length() / 10 * 255);
            }
        }
        else
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
            d.velocity = Projectile.velocity;
            d.noGravity = true;
            d.alpha = 255 - (int)(Projectile.velocity.Length() / 10 * 255);
            if (Projectile.Center.Distance(owner.Center) > 30)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity + Projectile.Center.DirectionTo(owner.Center) * 0.4f, Projectile.Center.DirectionTo(owner.Center) * Projectile.velocity.Length(), 0.2f);
            }
        }
        Projectile.hide = Projectile.tileCollide;
        Projectile.velocity = Projectile.velocity.LengthClamp(10);
        Projectile.rotation += Math.Clamp(Projectile.velocity.X * 0.05f, -0.4f, 0.4f);
        Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.8f, 1.8f));
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        if (Projectile.hide)
            behindNPCsAndTiles.Add(index);
        base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (oldVelocity.X != Projectile.velocity.X)
        {
            Projectile.velocity.Y = -5f;
        }
        return base.OnTileCollide(oldVelocity);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Rectangle flake = new Rectangle();

        switch (Projectile.frame) // I blame the spriter
        {
            case 0:
                flake = new Rectangle(0, 4, 38, 38);
                break;
            case 1:
                flake = new Rectangle(44, 0, 50, 44);
                break;
            case 2:
                flake = new Rectangle(100, 2, 38, 42);
                break;
            case 3:
                flake = new Rectangle(144, 0, 42, 46);
                break;
            case 4:
                flake = new Rectangle(192, 0, 42, 46);
                break;
        }

        //flake
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, flake, new Color(1f, 1f, 1f, 1f) * 0.8f, Projectile.rotation, flake.Size() / 2, Projectile.scale, SpriteEffects.None);
        // face
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, new Rectangle(240, 14, 16, 18), new Color(1f, 1f, 1f, 1f), Projectile.velocity.X * 0.1f, new Vector2(8, 9), Projectile.scale, SpriteEffects.None);
        return false;
    }
}