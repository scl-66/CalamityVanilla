using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Drops.FungiDish;

public class FungiDish : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToVanitypet(ModContent.ProjectileType<FungoidPet>(), ModContent.BuffType<FungoidBuff>());
        Item.master = true;
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        base.UseStyle(player, heldItemFrame);

        if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
        {
            player.AddBuff(Item.buffType, 3600);
        }
    }
}

public class FungoidBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
        Main.vanityPet[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        bool unused = false;
        player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<FungoidPet>());
    }
}

public class FungoidPet : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 10;
        Main.projPet[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.BlackCat);
        AIType = ProjectileID.BlackCat;
        Projectile.width = 38;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.netImportant = true;
        Projectile.timeLeft *= 5;
        Projectile.friendly = true;
        //Projectile.hide = true;
        Projectile.ignoreWater = true;
    }

    int realFrameCounter;
    int realFrame;

    public override void AI()
    {
        base.AI();

        Player owner = Main.player[Projectile.owner];
        Projectile.spriteDirection = Projectile.Center.X >= owner.Center.X ? -1 : 1;

        if (!owner.active)
        {
            Projectile.active = false;
            return;
        }
        if (!owner.dead && owner.HasBuff(ModContent.BuffType<FungoidBuff>()))
        {
            Projectile.timeLeft = 2;
        }

        if (!Projectile.tileCollide)
        {
            //Main.NewText(Projectile.velocity.Length());
            int dustAmount = 13;
            if (Projectile.velocity.Length() > 7.5f) dustAmount = 3;
            if (Main.rand.NextBool(dustAmount))
            {
                int d = Dust.NewDust(Projectile.Center, 2, 2, DustID.Corruption, -Projectile.velocity.X * 0.3f, -Projectile.velocity.Y * 0.3f, 1, Color.White, 1f);
                Main.dust[d].scale = Main.rand.NextFloat(0.75f, 1f);
                Main.dust[d].velocity *= 0.8f;
                if (Projectile.velocity.Length() < 7.5f) Main.dust[d].velocity *= 0.7f;
            }
            if (owner.velocity.Y != 0)
                Projectile.velocity.Y += 0.13f;

            Projectile.rotation = Projectile.velocity.Length() / 30f * Projectile.spriteDirection;
        }
        else
        {
            //Main.NewText(Projectile.velocity.Length());
            int dustAmount = 75;
            if (Projectile.velocity.Length() > 4f) dustAmount = 10;
            if (Main.rand.NextBool(dustAmount))
            {
                int d = Dust.NewDust(Projectile.Center, 2, 2, DustID.Corruption, -Projectile.velocity.X * 0.3f, -Projectile.velocity.Y * 0.3f, 1, Color.White, 1f);
                Main.dust[d].scale = Main.rand.NextFloat(0.75f, 1f);
                Main.dust[d].velocity *= 0.8f;
            }

            Projectile.rotation = 0;
        }



        if (Projectile.velocity == new Vector2(0, 0.4f))
        {
            realFrame = 0;
        }
        else if (++realFrameCounter >= 8 / (Projectile.velocity.Length() / 1.5f) && Projectile.tileCollide)
        {
            realFrame++;
            realFrameCounter = 0;
            if (realFrame >= 5)
            {
                realFrame = 0;
            }
        }
        else if (++realFrameCounter >= 12 && !Projectile.tileCollide)
        {
            if (realFrame < 5)
                realFrame = 5;

            realFrame++;
            realFrameCounter = 0;
            if (realFrame >= Main.projFrames[Type])
                realFrame = 5;
        }

        Projectile.frame = realFrame;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }
}