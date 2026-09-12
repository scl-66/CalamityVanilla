using CalamityVanilla.Common.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Corruption.HiveMind.Drops.SinisterIncubator;

public class SinisterIncubatorAnimation : DrawAnimation
{
    public static bool Open = false;
    public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1)
    {
        if (Open)
            return new Rectangle(0, 0, 48, 42);
        return new Rectangle(0, 44, 34, 36);
    }
    private class SinisterPlayerLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.ItemAnimationActive)
                Open = true;
        }
    }
    private class SinisterPlayerLayer2 : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Open = false;
        }
    }
}
public class SinisterIncubator : ModItem
{
    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Type, new SinisterIncubatorAnimation());
    }
    public override void SetDefaults()
    {
        Item.width = 37;
        Item.height = 37;
        Item.damage = 8;
        Item.DamageType = DamageClass.Summon;
        Item.mana = 20;
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 0.25f;

        Item.shoot = ModContent.ProjectileType<Fungusmite>();
        Item.buffType = ModContent.BuffType<FungusmiteBuff>();
        Item.shootSpeed = 0;

        Item.UseSound = SoundID.Item44;
        Item.autoReuse = true;

        Item.holdStyle = ItemHoldStyleID.HoldFront;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 2, 10, 0);
    }
    public override void HoldStyle(Player player, Rectangle heldItemFrame)
    {
        player.itemLocation.X += player.direction * -21;
        player.itemLocation.Y += player.gravDir * -21;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        player.AddBuff(Item.buffType, 2);

        Projectile projectile = Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, Main.myPlayer, ai2: Main.rand.Next(3));
        projectile.originalDamage = Item.damage;

        return false;
    }
}
public class FungusmiteBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (player.ownedProjectileCounts[ModContent.ProjectileType<Fungusmite>()] > 0)
        {
            player.buffTime[buffIndex] = 18000;
        }
        else
        {
            player.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
public class Fungusmite : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 8;
        ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        Main.projPet[Projectile.type] = true;
        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;

        Projectile.friendly = true;
        Projectile.minion = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.minionSlots = 1f;
        Projectile.penetrate = -1;
    }
    public override bool? CanCutTiles() => false;
    public override bool MinionContactDamage() => false;

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        fallThrough = true;
        return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = Math.Sign(Projectile.oldVelocity.Y) * -2;
        //Projectile.velocity.Y = -oldVelocity.Y;
        if (Projectile.velocity.X != oldVelocity.X)
            //Projectile.velocity.X = -oldVelocity.X;
            Projectile.velocity.X = Math.Sign(Projectile.oldVelocity.X) * -2;
        return base.OnTileCollide(oldVelocity);
    }
    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.X * 0.1f;
        Player owner = Main.player[Projectile.owner];
        if (!CheckAlive(owner)) return;
        Projectile.ai[1]++;
        Projectile.frameCounter++;
        if (Projectile.frameCounter > 6)
        {
            Projectile.frameCounter = 0;
            if (Projectile.frame == Main.projFrames[Projectile.type] - 1) Projectile.frame = 0;
            else Projectile.frame++;
        }

        float ownerDistance = Projectile.Center.Distance(owner.Center);

        Projectile.tileCollide = Collision.CanHit(Projectile, owner);

        if (ownerDistance > 2400)
        {
            Projectile.Center = owner.Center;
        }

        foreach (Projectile p in Main.ActiveProjectiles)
        {
            if (p.owner == owner.whoAmI && p.type == Type)
            {
                Projectile.ai[1] = p.ai[1];
                break;
            }
        }

        Vector2 targetPos = owner.Center;
        //if (owner.ownedProjectileCounts[Type] > 1)
        targetPos -= new Vector2(0, MathF.Sin(Projectile.ai[1] * 0.05f) * 32 + 64).RotatedBy((Projectile.minionPos / (float)owner.ownedProjectileCounts[Type] * MathHelper.TwoPi) + Projectile.ai[1] * 0.02f);
        //else
        //    targetPos -= new Vector2(0, MathF.Sin(Projectile.ai[1] * 0.05f) * 32 + 64);

        float targetDistance = Projectile.Center.Distance(owner.Center);
        Projectile.velocity += Projectile.Center.DirectionTo(targetPos) * targetPos.Distance(Projectile.Center) * 0.001f;
        if (targetDistance < 128 && Projectile.velocity.Length() > 1f)
            Projectile.velocity *= 0.95f;

        Projectile.velocity = Projectile.velocity.LengthClamp(MathHelper.Max(MathHelper.Max(5, targetDistance * 0.03f), owner.velocity.Length()));
        Projectile.ai[0] -= 1;


        //Dust d = Dust.NewDustPerfect(targetPos, DustID.RainbowMk2);
        //d.velocity = Vector2.Zero;
        //d.color = Color.Orange;
        //d.noGravity = true;
    }
    public static void Attack(Projectile p, NPC target)
    {
        if (!Collision.CanHit(p, target))
            return;
        p.netUpdate = true;
        p.ai[0] = 20;
        Vector2 attackDirection = p.Center.DirectionTo(target.Center);
        p.velocity += attackDirection * -1;
        if (p.owner == Main.myPlayer)
            Projectile.NewProjectile(p.GetSource_FromThis(), p.Center, attackDirection * 8, ModContent.ProjectileType<FungusmiteProjectile>(), p.damage, p.knockBack, p.owner);
    }
    private bool CheckAlive(Player owner)
    {

        if (!owner.active || owner.dead)
        {
            owner.ClearBuff(ModContent.BuffType<FungusmiteBuff>());
            return false;
        }

        if (owner.HasBuff(ModContent.BuffType<FungusmiteBuff>()))
        {
            Projectile.timeLeft = 2;
        }

        return true;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Rectangle frame = tex.Frame(3, Main.projFrames[Type], (int)Projectile.ai[2], Projectile.frame);
        SpriteEffects se = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Vector2 scale = Vector2.One + new Vector2((float)Math.Sin((Main.timeForVisualEffects * 0.5f) + Projectile.identity * 17), -(float)Math.Sin((Main.timeForVisualEffects * 0.5f) + Projectile.identity * 17)) * Utils.Remap(Projectile.ai[0], -10, 20, 0, 0.3f);

        int mod = (int)(Main.timeForVisualEffects % 3);
        //for(int i = Projectile.oldPos.Length - 3 + mod; i > 0; i-= 3)
        for (int i = Projectile.oldPos.Length - 2; i > 0; i -= 3)
        {
            float percent = 1f - (i / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(tex, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, frame, lightColor * 0.5f * percent, Projectile.oldRot[i], frame.Size() / 2, Projectile.scale * scale, se);
        }

        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2, Projectile.scale * scale, se);
        return false;
    }
}

public class FungusmiteProjectile : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.MinionShot[Type] = true;
    }
    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;

        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.extraUpdates = 2;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.Opacity = 0;
        //Projectile.penetrate = -1;
        //Projectile.timeLeft = 80;
    }

    public override bool? CanCutTiles() => false;

    public override void AI()
    {
        if (Projectile.ai[0] == 0)
        {
            //SoundEngine.PlaySound(SoundID.Item111, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
                d.noGravity = true;
                d.velocity = Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat();
                d.alpha = 128;
            }
            for (int i = 0; i < 15; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
                d.noGravity = true;
                d.velocity = Projectile.velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(2);
            }
        }
        Projectile.Opacity += 0.05f;
        Projectile.ai[0]++;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(
            tex,
            Projectile.Center - Main.screenPosition,
            tex.Bounds,
            lightColor * Projectile.Opacity,
            Projectile.rotation,
            new Vector2(7, 7),
            Projectile.scale,
            SpriteEffects.None);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        int type = ModContent.DustType<VileMushroomDust>();
        Vector2 direction = Vector2.Normalize(Projectile.velocity);
        for (int i = 0; i < 48; i += 8)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center - (direction * i), type);
            d.frame.X = 10;
            d.velocity = direction * Main.rand.NextFloat(2, 4) + Main.rand.NextVector2Square(-1, 1);
            d.noGravity = true;
            d.alpha = Main.rand.Next(128);
        }
    }
}

public class SinisterIncubatorPlayer : ModPlayer
{
    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        int minionType = ModContent.ProjectileType<Fungusmite>();
        if (!proj.minion && !ProjectileID.Sets.MinionShot[proj.type])
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.owner == Player.whoAmI && p.type == minionType && p.ai[0] <= 0)
                {
                    Fungusmite.Attack(p, target);
                }
            }
    }
    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        int minionType = ModContent.ProjectileType<Fungusmite>();
        foreach (Projectile p in Main.ActiveProjectiles)
        {
            if (p.owner == Player.whoAmI && p.type == minionType && p.ai[0] <= 0)
            {
                Fungusmite.Attack(p, target);
            }
        }
    }
}