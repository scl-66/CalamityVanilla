using CalamityVanilla.Content.Bosses.HiveMind.Drops.PerfectDark;
using CalamityVanilla.Content.Dusts;
using CalamityVanilla.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Drops.MushroomBomber;

public class MushroomBomberShotSmall : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.penetrate = 3;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }
    public override void AI()
    {
        Projectile.ai[0]++;
        if (Projectile.ai[0] > 20)
            Projectile.velocity.Y += 0.3f;

        Projectile.rotation += Projectile.velocity.X * 0.05f;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.velocity = -Projectile.Center.DirectionTo(target.Center) * Projectile.velocity.Length();
    }
    public override void OnKill(int timeLeft)
    {
        int type = ModContent.DustType<VileMushroomDust>();
        for(int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type);
            d.velocity *= 2.5f;
            d.fadeIn = Main.rand.NextFloat(0.2f, 0.5f);
            d.noGravity = Main.rand.NextBool();
        }
    }
}
public class MushroomBomberShotMedium : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Ranged;
    }
    public override void AI()
    {
        if (Projectile.ai[0] == 0)
        {
            Projectile.ai[2]++;
            if (Projectile.ai[2] > 20)
                Projectile.velocity.Y += 0.15f;

            Projectile.rotation = Utils.AngleLerp(Projectile.velocity.ToRotation() - MathHelper.PiOver2, Projectile.rotation + Projectile.velocity.X * 0.05f, Utils.Remap(Projectile.ai[0], 15, 40, 1f, 0, true));
        }
        else
        {
            Projectile.hide = true;
            Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center + Projectile.velocity * 0.95f;
            Projectile.rotation = Projectile.Center.DirectionTo(Main.npc[(int)Projectile.ai[1]].Center).ToRotation() - MathHelper.PiOver2;
            Main.npc[(int)Projectile.ai[1]].AddBuff(ModContent.BuffType<MushroomBomberInfection>(), 2, true);
            Main.npc[(int)Projectile.ai[1]].AddBuff(BuffID.Confused, 60 * 5, true);
            if (!Main.npc[(int)Projectile.ai[1]].active)
            {
                Projectile.Kill();
            }
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        // If attached to an NPC, draw behind tiles (and the npc) if that NPC is behind tiles, otherwise just behind the NPC.
        if (Projectile.ai[0] == 1)
        {
            int npcIndex = (int)Projectile.ai[1];
            if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
            {
                if (Main.npc[npcIndex].behindTiles)
                {
                    behindNPCsAndTiles.Add(index);
                }
                else
                {
                    behindNPCs.Add(index);
                }
            }
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return Projectile.ai[0] == 0;
    }
    private readonly Point[] _sticking = new Point[12];
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.timeLeft = 60 * 20;
        Projectile.damage = 0;
        Projectile.ai[0] = 1;
        Projectile.ai[1] = target.whoAmI;
        Projectile.velocity = Projectile.Center - target.Center;
        Projectile.netUpdate = true;
        Projectile.KillOldestJavelin(Projectile.whoAmI, Type, (int)Projectile.ai[1], _sticking);
    }

    public override void OnKill(int timeLeft)
    {
        int type = ModContent.DustType<VileMushroomDust>();
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type);
            d.velocity *= 2.5f;
            d.fadeIn = Main.rand.NextFloat(0.2f, 0.5f);
            d.noGravity = Main.rand.NextBool();
        }
    }
}
public class MushroomBomberShotLarge : ModProjectile
{
    private static Asset<Texture2D> _explosionTexture;
    private static Asset<Texture2D> _babiesTexture;

    public override void SetStaticDefaults()
    {
        _explosionTexture = ModContent.Request<Texture2D>(Texture + "Explosion");
        _babiesTexture = ModContent.Request<Texture2D>(Texture + "Babies");
    }
    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }
    public override void AI()
    {
        Projectile.ai[0]++;
        if (Projectile.ai[0] > 20)
            Projectile.velocity.Y += 0.3f;

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }
    public override bool? CanHitNPC(NPC target)
    {
        if (Projectile.ai[1] == 1)
        {
            return target.Hitbox.ClosestPointInRect(Projectile.Center).Distance(Projectile.Center) < Projectile.width;
        }
        return base.CanHitNPC(target);
    }
    public override void OnKill(int timeLeft)
    {
        if(Main.myPlayer == Projectile.owner)
        {
            int spore = ModContent.ProjectileType<MushroomBomberSpores>();
            for(int i = 0; i < 12; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.1f,0.4f) + new Vector2(0,-1), spore, Projectile.damage / 5, 0, Projectile.owner, Main.rand.Next(90, 150));
        }

        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
        Projectile.ai[1] = 1;
        Projectile.Resize(200, 200);
        Projectile.Damage();
        int type = ModContent.DustType<VileMushroomDust>();
        for (int i = 0; i < 45; i++)
        {
            Vector2 vect = Main.rand.NextVector2CircularEdge(Projectile.width / 2, Projectile.width / 2) * Main.rand.NextFloat(0.5f, 1f);
            Dust d = Dust.NewDustPerfect(Projectile.Center + vect, type, Vector2.Normalize(vect) * Main.rand.NextFloat(5,11));
            d.fadeIn = Main.rand.NextFloat(0.2f, 0.5f);
            d.noGravity = true;
            d.scale += Main.rand.NextFloat();
        }
        type = ModContent.DustType<SimpleColorableGlowyDust>();
        for (int i = 0; i < 45; i++)
        {
            Vector2 vect = Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
            Dust d = Dust.NewDustPerfect(Projectile.Center + vect, type, Vector2.Normalize(vect) * Main.rand.NextFloat(2, 7));
            //d.color = new Color(Main.rand.NextFloat(0.75f), 0.1f, 1f, 0f);
            d.color = Color.Lerp(new Color(1f, 0.5f, 0.5f, 0f), new Color(0.5f, 0.25f, 1f, 0f), Main.rand.NextFloat());
            d.noGravity = true;
            d.fadeIn = Main.rand.NextFloat(1.5f);
        }
        for (int i = 0; i < 8; i++)
        {
            var p2 = FadingParticleWithLighting.RequestFadingParticleWithLighting();
            p2.SetBasicInfo(_babiesTexture, _babiesTexture.Frame(1, 3, 0, Main.rand.Next(3)), Vector2.Zero, Projectile.Center);
            p2.SetTypeInfo(Main.rand.Next(50, 75));
            p2.ColorTint = Color.White * 0.5f;
            p2.FadeInNormalizedTime = 0.15f;
            p2.FadeOutNormalizedTime = 0.7f;
            p2.Scale = Vector2.One * Main.rand.NextFloat(0.8f,1f);
            p2.Velocity = Main.rand.NextVector2Circular(4, 3) + new Vector2(0, -3);
            p2.RotationVelocity = Main.rand.NextFloat(-0.2f,0.2f);
            p2.AccelerationPerFrame = new Vector2(0f, 0.1f);
            Main.ParticleSystem_World_BehindPlayers.Add(p2);
        }

        var p = AnimatedParticle.RequestAnimatedParticle();
        p.SetTypeInfo(10, 50, _explosionTexture, Color.White);
        p.LocalPosition = Projectile.Center + new Vector2(0,-10);
        p.Scale = Vector2.One;
        Main.ParticleSystem_World_OverPlayers.Add(p);
    }
}
public class MushroomBomberSpores : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
    }
    public override void SetDefaults()
    {
        Projectile.QuickDefaults(false, 36);
        Projectile.timeLeft = 300;
        Projectile.alpha = 128;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 40;
        Projectile.frame = Main.rand.Next(3);
    }
    public override void AI()
    {
        if (Main.rand.NextBool(15))
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption);
            d.alpha = 128;
            d.velocity *= 0.4f;
        }

        Projectile.ai[0]++;
        Projectile.velocity *= 0.99f;
        Projectile.velocity.Y += 0.0025f;

        Projectile.scale = 1f + (float)Math.Sin((Projectile.ai[0] + Projectile.identity) * 0.1f) * 0.1f;
        Projectile.rotation += Projectile.velocity.X * 0.02f + Projectile.direction * 0.02f;

        foreach(Projectile p in Main.ActiveProjectiles)
        {
            if (p.whoAmI == Projectile.whoAmI || p.type != Type || p.Center == Projectile.Center || p.Center.Distance(Projectile.Center) > 32)
                continue;

            Projectile.velocity += Projectile.Center.DirectionTo(p.Center) * -0.0015f;
        }
        Projectile.velocity = Projectile.velocity.LengthClamp(1f);
        if (Projectile.timeLeft < 20)
        {
            Projectile.alpha += 6;
        }
    }
}