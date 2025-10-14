using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Tundra.Items.PermafrostHook;

public class PermafrostHook : ModItem
{
    public override void SetDefaults()
    {
        Item.Size = new Vector2(26, 20);
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item1;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.rare = ItemRarityID.Pink;
        Item.expert = true;
        Item.value = Item.sellPrice(0, 6, 0, 0);
        Item.shoot = ModContent.ProjectileType<PermafrostHookProjectile>();
        Item.shootSpeed = 18f;
    }
}

public class PermafrostHookProjectile : ModProjectile
{
    public override void Load()
    {
        IL_Projectile.AI_007_GrapplingHooks += FreezePlayerOnHookAttach;
    }

    public override void Unload()
    {
        IL_Projectile.AI_007_GrapplingHooks -= FreezePlayerOnHookAttach;
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.SingleGrappleHook[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
    }

    private void FreezePlayerOnHookAttach(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);

        // Move the cursor to an area of code checking for maximum grapple range. Not what we're interested in.
        if (!cursor.TryGotoNext(MoveType.Before, x => x.MatchLdcI4(ProjectileID.QueenSlimeHook)))
            return;

        // Move the cursor to an area of code trying to teleport with the Hook of Dissonance.
        // Specifically, the cursor should be placed between an instruction for loading the projectile instance and an instruction for loading ProjectileID.QueenSlimeHook.
        if (!cursor.TryGotoNext(MoveType.Before, x => x.MatchLdcI4(ProjectileID.QueenSlimeHook)))
            return;
        cursor.Index--;

        // Insert our code.
        cursor.EmitDelegate((Projectile projectile) =>
        {
            if (projectile.type == ModContent.ProjectileType<PermafrostHookProjectile>())
            {
                Player player = Main.player[projectile.owner];
                Projectile.NewProjectileDirect(
                    player.GetSource_ItemUse(player.QuickGrapple_GetItemToUse()),
                    player.Center - new Vector2(24, 24),
                    Vector2.Zero,
                    ModContent.ProjectileType<PermafrostHookIceBlock>(),
                    160,
                    6,
                    projectile.owner
                );
            }
        });

        // Emit the projectile instance to restore the stack back to it original state.
        cursor.EmitLdarg0();
    }

    public override bool? CanUseGrapple(Player player)
    {
        int hooksOut = 0;
        foreach (var projectile in Main.ActiveProjectiles)
        {
            if (projectile.owner == Main.myPlayer && projectile.type == Projectile.type)
            {
                hooksOut++;
            }
        }

        return hooksOut <= 0;
    }

    public override bool? GrappleCanLatchOnTo(Player player, int x, int y)
    {
        if (Projectile.ai[0] == 2f && Projectile.Center.Distance(player.Center) < 102)
        {
            Projectile.ai[0] = 1;
            return false;
        }
        return null;
    }

    public override float GrappleRange()
    {
        return 450f;
    }

    public override void NumGrappleHooks(Player player, ref int numHooks)
    {
        numHooks = 1;
    }

    public override void GrappleRetreatSpeed(Player player, ref float speed)
    {
        speed = 20f;
    }

    public override void GrapplePullSpeed(Player player, ref float speed)
    {
        Projectile? iceBlock = Main.projectile.FirstOrDefault(projectile => projectile.active &&
                                                                            projectile.type == ModContent.ProjectileType<PermafrostHookIceBlock>() &&
                                                                            projectile.owner == player.whoAmI);

        if (iceBlock != null)
            speed = Utils.Remap(iceBlock.ai[0], 0, 60, 0f, 32);
        else
            speed = 14;
    }

    public override bool PreDrawExtras()
    {
        Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("CalamityVanilla/Content/Items/Equipment/Other/PermafrostHookChain");

        Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
        Vector2 projectileCenter = Projectile.Center;
        Vector2 directionToPlayer = Main.player[Projectile.owner].MountedCenter - projectileCenter;

        float rotationToPlayer = directionToPlayer.ToRotation() - MathHelper.PiOver2;
        float distanceToPlayer = directionToPlayer.Length();

        while (distanceToPlayer > 25f && !float.IsNaN(distanceToPlayer))
        {
            directionToPlayer /= distanceToPlayer; // Turns directionToPlayer into a unit vector to...
            directionToPlayer *= chainTexture.Height(); // ...resize it to the chain texture's length.

            projectileCenter += directionToPlayer;
            directionToPlayer = mountedCenter - projectileCenter;
            distanceToPlayer = directionToPlayer.Length();

            Color lightColor = Lighting.GetColor((int)projectileCenter.X / 16, (int)(projectileCenter.Y / 16f));
            Main.spriteBatch.Draw(chainTexture.Value, projectileCenter - Main.screenPosition, null, lightColor, rotationToPlayer, chainTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        return false;
    }
}

public class PermafrostHookIceBlock : ModProjectile
{
    public ref float Timer => ref Projectile.ai[0];

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(64, 64);
        Projectile.friendly = true;
        Projectile.netImportant = true;
        Projectile.DamageType = DamageClass.Default;
        Projectile.hide = true;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        Timer++;
        float progress = Utils.GetLerpValue(0, 10, Timer, true);
        progress = progress * progress;
        Projectile.scale = MathHelper.Lerp(1.5f, 1f, progress);
        Projectile.Opacity = MathHelper.Lerp(0, 0.75f, progress);

        Player owner = Main.player[Projectile.owner];
        bool isGrappling = owner.grapCount > 0;

        if (owner.active && (owner.controlLeft || owner.controlRight || owner.controlUp || owner.controlDown || owner.controlJump))
        {
            Projectile.Kill();
            return;
        }

        if (isGrappling)
        {
            Projectile.Center = owner.Center;
            Projectile.velocity = owner.velocity;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        else
        {
            Projectile.velocity.Y += 0.2f;
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y, 16);

            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;

            if (Projectile.velocity.Y == 0f)
            {
                Collision.StepDown(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref owner.stepSpeed, ref Projectile.gfxOffY);
            }
            if (Projectile.velocity.Y >= 0f)
            {
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref owner.stepSpeed, ref Projectile.gfxOffY, (int)owner.gravDir, false);
            }

            owner.Center = Projectile.Center;
            owner.velocity = Projectile.velocity;
        }
    }

    public override void OnKill(int timeLeft)
    {
        Player owner = Main.player[Projectile.owner];

        SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

        for (int i = 0; i < 32; i++)
        {
            Dust dust = Dust.NewDustDirect(
                Projectile.position, Projectile.width, Projectile.height,
                DustID.Ice,
                Scale: 1.5f
            );
            //dust.noGravity = true;
            dust.velocity *= 0.5f;
        }

        owner.SetImmuneTimeForAllTypes(owner.longInvince ? 120 : 60);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 5f)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item50, Projectile.Center);
        }

        if (Projectile.velocity.X != oldVelocity.X)
        {
            return true;
        }

        return false;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> texture = TextureAssets.Projectile[Type];
        Main.spriteBatch.Draw(texture.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, 0f, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}