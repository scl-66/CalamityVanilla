using CalamityVanilla.Content.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityVanilla.Content.Bosses.GutOfCthulhu.Drops.TapewormWhip;

internal sealed class TapewormWhipItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.SummonMeleeSpeed;
        Item.damage = 50;
        Item.knockBack = 2;
        Item.rare = ModContent.RarityType<BlackAndGoldRarity>();

        Item.shoot = ModContent.ProjectileType<TapewormWhipProjectile>();
        Item.shootSpeed = 6;

        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.UseSound = SoundID.Item152;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool MeleePrefix()
    {
        return true;
    }
}

internal sealed class TapewormWhipProjectile : ModProjectile
{
    private float _timer
    {
        get => Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.IsAWhip[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ownerHitCheck = true;
        Projectile.extraUpdates = 1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.WhipSettings.Segments = 64;
        Projectile.WhipSettings.RangeMultiplier = 1f;
    }

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * _timer;
        Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

        _timer++;

        float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
        if (_timer >= swingTime || owner.itemAnimation <= 0)
        {
            Projectile.Kill();
            return;
        }

        owner.heldProj = Projectile.whoAmI;
        if (_timer == swingTime / 2)
        {
            // Plays a whipcrack sound at the tip of the whip.
            List<Vector2> points = Projectile.WhipPointsForCollision;
            Projectile.FillWhipControlPoints(Projectile, points);
            SoundEngine.PlaySound(SoundID.Item153, points[^1]);
        }

        var swingProgress = _timer / swingTime;
        if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && !Main.rand.NextBool(3))
        {
            List<Vector2> points = Projectile.WhipPointsForCollision;
            points.Clear();
            Projectile.FillWhipControlPoints(Projectile, points);

            int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
            Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));
            int dustType = DustID.Blood;

            Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, dustType, 0f, 0f, 100, Color.White);
            dust.position = points[pointIndex];
            dust.fadeIn = 0.3f;
            dust.noGravity = true;

            Vector2 spinningPoint = points[pointIndex] - points[pointIndex - 1];
            dust.velocity += spinningPoint.RotatedBy(owner.direction * ((float)Math.PI / 2f));
            dust.velocity *= 0.5f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Player owner = Main.player[Projectile.owner];
        owner.MinionAttackTargetNPC = target.whoAmI;

        var npc = target.GetGlobalNPC<TapewormWhipGlobalNPC>();
        npc.IsTaggedByAWhip = true;
        npc.WhipTagTimer = 60 * 5;
        npc.TaggedByPlayer = owner.whoAmI;

        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, target.whoAmI);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        List<Vector2> list = new List<Vector2>();
        Projectile.FillWhipControlPoints(Projectile, list);

        SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        Texture2D texture = TextureAssets.Projectile[Type].Value;

        Vector2 currentDrawPos = list[0];

        float baseWiggleStrength = 14f;
        float waveFrequency = 0.8f;
        float timeFactor = Main.GlobalTimeWrappedHourly * 12f;

        float segmentAdvanceFactor = 0.8f;

        int totalSegments = list.Count - 1;
        if (totalSegments <= 0) return false;

        for (int i = 0; i < totalSegments; i++)
        {
            Rectangle frame;
            Vector2 origin;
            float scale = 1f;

            int segmentsFromEnd = totalSegments - i;

            if (segmentsFromEnd == 1)
            {
                frame = new Rectangle(0, 88, 14, 18);
                origin = new Vector2(7, 0);
            }
            else if (segmentsFromEnd < 6)
            {
                frame = new Rectangle(0, 72, 14, 8);
                origin = new Vector2(7, 0);
            }
            else if (segmentsFromEnd == 6)
            {
                frame = new Rectangle(0, 72, 14, 8);
                origin = new Vector2(7, 0);
            }
            else if (segmentsFromEnd == 7)
            {
                frame = new Rectangle(0, 56, 14, 8);
                origin = new Vector2(7, 0);
            }
            else if (segmentsFromEnd == 8)
            {
                frame = new Rectangle(0, 40, 14, 8);
                origin = new Vector2(7, 0);
            }
            else if (i == 0)
            {
                frame = new Rectangle(0, 0, 14, 16);
                origin = new Vector2(7, 0);
            }
            else
            {
                frame = new Rectangle(0, 24, 14, 8);
                origin = new Vector2(7, 0);
            }

            Vector2 element = list[i];
            Vector2 diff = list[i + 1] - element;

            var rotation = diff.ToRotation() - MathHelper.PiOver2;

            var whipProgress = totalSegments > 1 ? (float)i / (totalSegments - 1) : 0f;
            var currentWigglyStrength = baseWiggleStrength * whipProgress;

            Vector2 perpendicular = diff.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
            var sine = (float)Math.Sin(whipProgress * MathHelper.TwoPi * waveFrequency + timeFactor);

            Vector2 wiggleOffset = perpendicular * sine * currentWigglyStrength;
            var rotationWiggle = (float)Math.Cos(whipProgress * MathHelper.TwoPi * waveFrequency + timeFactor) * (currentWigglyStrength * 0.05f);

            Color color = Lighting.GetColor(element.ToTileCoordinates());

            Main.EntitySpriteDraw(texture, currentDrawPos + wiggleOffset - Main.screenPosition, frame, color, rotation + rotationWiggle, origin, scale, flip);

            currentDrawPos += diff * segmentAdvanceFactor;
        }
        return false;
    }
}

public class TapewormWhipGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public bool IsTaggedByAWhip;
    public int WhipTagTimer;
    public int TaggedByPlayer = -1;

    public override void ResetEffects(NPC npc)
    {
        if (IsTaggedByAWhip && WhipTagTimer <= 0)
        {
            IsTaggedByAWhip = false;
            WhipTagTimer = 0;
            TaggedByPlayer = -1;
        }
    }

    public override bool PreAI(NPC npc)
    {
        if (IsTaggedByAWhip)
        {
            WhipTagTimer--;
        }

        return true;
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damage)
    {
        if (IsTaggedByAWhip && TaggedByPlayer != -1 && projectile.owner == TaggedByPlayer)
        {
            if (Main.rand.NextBool(3))
            {
                int type = Main.rand.Next(4) switch
                {
                    0 => ModContent.ItemType<TapewormWhipGiblet1>(),
                    1 => ModContent.ItemType<TapewormWhipGiblet2>(),
                    _ => ModContent.ItemType<TapewormWhipGiblet3>(),
                };

                Item.NewItem(npc.GetSource_OnHit(projectile), npc.Center, type);
            }
        }
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        bitWriter.WriteBit(IsTaggedByAWhip);
        if (IsTaggedByAWhip)
        {
            binaryWriter.Write(WhipTagTimer);
            binaryWriter.Write(TaggedByPlayer);
        }
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
    {
        IsTaggedByAWhip = bitReader.ReadBit();
        if (IsTaggedByAWhip)
        {
            WhipTagTimer = binaryReader.ReadInt32();
            TaggedByPlayer = binaryReader.ReadInt32();
        }
    }
}