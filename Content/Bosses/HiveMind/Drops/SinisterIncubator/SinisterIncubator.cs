using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Drops.SinisterIncubator;

public class SinisterIncubator : ModItem
{
    public override void SetStaticDefaults()
    {
        // Since the open box frame is the first frame, we can set a drawAnimation 
        // that never increments to mimic the box opening on use, since
        // we manually override the item's drawing in inventory and world.
        DrawAnimationVertical drawAnim = new DrawAnimationVertical(1, 2);
        drawAnim.NotActuallyAnimating = true;
        Main.RegisterItemAnimation(Type, drawAnim);

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
        Item.shootSpeed = 1f;

        Item.UseSound = SoundID.Item44;
        Item.autoReuse = true;


        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 2, 10, 0);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        position = Main.MouseWorld;
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        player.AddBuff(Item.buffType, 2);

        Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
        projectile.originalDamage = Item.damage;

        return false;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        spriteBatch.Draw(TextureAssets.Item[Type].Value,
            position,
            new Rectangle(0, 44, 48, 44),
            drawColor,
            0f,
            new Vector2(17, 18),
            scale * 1.4f,
            SpriteEffects.None,
            0f);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        spriteBatch.Draw(TextureAssets.Item[Type].Value,
            Item.Center - Main.screenPosition,
            new Rectangle(0, 44, 48, 44),
            alphaColor,
            rotation,
            new Vector2(17, 18),
            scale,
            SpriteEffects.None,
            0f);
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
    private Vector2 push = Vector2.Zero;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 8;
        ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

        Main.projPet[Projectile.type] = true;

        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 46;
        Projectile.height = 46;
        Projectile.tileCollide = false;

        Projectile.friendly = true;
        Projectile.minion = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.minionSlots = 1f;
        Projectile.penetrate = -1;
    }

    public override bool? CanCutTiles() => false;
    public override bool MinionContactDamage() => false;

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];

        if (!CheckAlive(owner)) return;

        if (Projectile.ai[0] % 6 == 0)
        {
            if (Projectile.frame == Main.projFrames[Projectile.type] - 1) Projectile.frame = 0;
            else Projectile.frame++;
        }

        if (Projectile.Center.Distance(owner.Center) > 10)
        {
            float dist = Projectile.Center.Distance(owner.Center) / 50f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.Center.DirectionTo(owner.Center) * dist, 0.1f) + push;
            Projectile.velocity = Projectile.velocity.LengthClamp(16f, 2f);
            Projectile.velocity += Main.rand.NextVector2Circular(0.2f, 0.2f);

            Projectile.netUpdate = true;

        }


        if (owner.GetModPlayer<SinisterIncubatorPlayer>().NPCHit != null && Projectile.ai[1] <= 0)
        {
            Projectile.ai[1] = 20;
            NPC target = owner.GetModPlayer<SinisterIncubatorPlayer>().NPCHit;
            push = -Projectile.Center.DirectionTo(target.Center) * 0.75f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.Center.DirectionTo(target.Center) * 14f, ModContent.ProjectileType<FungusmiteProjectile>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
        }

        push *= 0.9f;
        Projectile.ai[1] -= 1;
        Projectile.ai[0]++;
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
}

public class FungusmiteProjectile : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.tileCollide = false;

        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.extraUpdates = 2;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        //Projectile.penetrate = -1;
        //Projectile.timeLeft = 80;
    }

    public override bool? CanCutTiles() => false;

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        if (Projectile.ai[0] > 25) Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.03f, -24f, 24f);
        Projectile.ai[0]++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;

        Main.EntitySpriteDraw(
            tex,
            Projectile.Center - Main.screenPosition,
            tex.Bounds,
            lightColor,
            Projectile.rotation,
            new Vector2(7, 7),
            Projectile.scale,
            SpriteEffects.None);

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Main.EntitySpriteDraw(
                tex,
                Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition,
                tex.Bounds,
                lightColor * (1f - i / (float)Projectile.oldPos.Length) * 0.3f,
                Projectile.oldRot[i],
                new Vector2(7, 7),
                Projectile.scale,
                SpriteEffects.None);
        }
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 6; i++)
        {
            Dust.NewDust(Projectile.Center, 4, 4, DustID.Sand, Projectile.velocity.X * 0.3f + Main.rand.NextFloat(-1f, 1f), Projectile.velocity.Y * 0.3f + Main.rand.NextFloat(-1f, 1f));
        }
    }
}

public class SinisterIncubatorPlayer : ModPlayer
{
    public NPC NPCHit = null;
    public int resetInt = 0;
    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!proj.minion && proj.DamageType != DamageClass.Summon)
        {
            resetInt = 0;
            NPCHit = target;
        }
    }

    public override void ResetEffects()
    {
        if (resetInt < 1) resetInt++;
        else
        {
            resetInt = 0;
            NPCHit = null;
        }
    }
}