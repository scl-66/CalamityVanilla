using CalamityVanilla.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underworld.Items.CarnageFlail;

public class CarnageFlail : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
    }

    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 45;
        Item.useTime = 40;
        Item.knockBack = 5.75f;
        Item.width = 40;
        Item.height = 50;
        Item.damage = 40;
        Item.scale = 1.1f;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<CarnageFlailProjectile>();
        Item.shootSpeed = 13f;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(gold: 3);
        Item.DamageType = DamageClass.MeleeNoSpeed;
        Item.channel = true;
        Item.noMelee = true;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: -1);
        return false;
    }
}

public class CarnageFlailProjectile : BaseFlailProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(30, 30);
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        Projectile.netImportant = true;
    }
    public override int MaxTimeLaunched => 13;
    public override float LaunchSpeed => 15;
    public override float MaxManualRetractionSpeed => 13;
    public override float MaxForcedRetractionSpeed => 16;
    public override int SpinningNPCHitCooldown => 12;

    public override bool ShouldFreelyRotate => false;
    private ref float _targetWhoami => ref Projectile.ai[2];
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);
        playerArmPosition -= Vector2.UnitY * player.gfxOffY;

        Projectile.rotation = Projectile.Center.DirectionTo(playerArmPosition).ToRotation() - MathHelper.PiOver4;
        if (Projectile.spriteDirection == 1)
            Projectile.rotation -= MathHelper.PiOver2;
        if (_targetWhoami <= -1)
        {
            base.AI();
            Projectile.spriteDirection = Projectile.Center.X < player.MountedCenter.X ? -1 : 1;
        }
        else
        {
            player.SetDummyItemTime(2);
            player.itemRotation = Projectile.DirectionFrom(player.MountedCenter).ToRotation();
            if (Projectile.Center.X < player.MountedCenter.X)
            {
                player.direction = -1;
                player.itemRotation = MathHelper.WrapAngle(player.itemRotation - (float)Math.PI);
            }
            else
            {
                player.direction = 1;
            }

            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            CurrentAIState = AIState.UnusedState;
            NPC target = Main.npc[(int)_targetWhoami];
            Projectile.Center = target.Center - Projectile.velocity;

            if (!target.active || player.controlUseItem)
            {
                if(!player.controlUseItem && target.lifeMax > 5 && !target.immortal)
                {
                    _targetWhoami = -2;
                }
                Projectile.velocity = Vector2.Zero;
                _targetWhoami = Math.Min(_targetWhoami,-1);
                CurrentAIState = AIState.ForcedRetracting;
            }
        }
    }
    public override bool ShouldUpdatePosition()
    {
        return _targetWhoami <= -1;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (CurrentAIState is AIState.LaunchingForward or AIState.Ricochet or AIState.Dropping)
        {
            CurrentAIState = AIState.UnusedState;
            _targetWhoami = target.whoAmI;
            Projectile.velocity = (target.Center - Projectile.Center) * 0.8f;
        }
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (_targetWhoami > -1)
            return true;
        return base.Colliding(projHitbox, targetHitbox);
    }
    public override bool? CanHitNPC(NPC target)
    {
        if (_targetWhoami > -1 && target.whoAmI != _targetWhoami)
        {
            return false;
        }
        return base.CanHitNPC(target);
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (CurrentAIState == AIState.UnusedState)
        {
            modifiers.SourceDamage *= 0.75f;
            modifiers.DisableKnockback();
        }
    }
    public override Rectangle? InitialChainSourceRectangle(Asset<Texture2D> texture) => texture.Frame(1, 3, 0, 0);
    public override void PreDrawChain(int chainCount, ref Asset<Texture2D> texture, ref Rectangle? sourceRectangle, ref Color lighColor)
    {
        int frameNumber = chainCount % 3;
        sourceRectangle = texture.Frame(1, 3, 0, frameNumber);
        if (chainCount < 1)
        {
            sourceRectangle = new Rectangle();
        }
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (_targetWhoami == -2)
        {
            SoundEngine.PlaySound(SoundID.Grab, Projectile.position);
            Main.player[Projectile.owner].Heal(40);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        base.PreDraw(ref lightColor);

        if (_targetWhoami == -2)
        {
            Asset<Texture2D> heart = TextureAssets.Item[ItemID.Heart];
            float rotation = Projectile.velocity.X * 0.05f;
            Main.EntitySpriteDraw(heart.Value, Projectile.Center - Main.screenPosition, null, new Color(1f, 0, 0, 0), rotation, heart.Size() / 2, 0.6f + (Main.masterColor * 0.4f), SpriteEffects.None);
            Main.EntitySpriteDraw(heart.Value, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0.8f), rotation, heart.Size() / 2, 0.6f + (Main.masterColor * 0.2f), SpriteEffects.None);
        }
        return false;
    }
}