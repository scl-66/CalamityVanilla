using CalamityVanilla.Content.Bosses.HiveMind.Projectiles;
using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Minions;

public class HiveMindWeeper : ModNPC
{
    public Player target
    { get { return Main.player[NPC.target]; } }
    public override void HitEffect(NPC.HitInfo hit)
    {
        int type = ModContent.DustType<VileMushroomDust>();
        for (int i = 0; i < hit.Damage / 2; i++)
        {
            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, type);
            d.fadeIn = Main.rand.NextFloat(0.2f, 0.5f);
            d.noGravity = Main.rand.NextBool();
        }
        if (NPC.life <= 0)
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(Name + $"{0}").Type);
            for (int i = -1; i <= 1; i += 2) // larger smoke puffs
            {
                for (int j = Main.rand.Next(-1, 1); j <= 1; j += 2)
                {
                    int smoke = Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X, NPC.position.Y), default, Main.rand.Next(61, 64));
                    Main.gore[smoke].velocity *= 0.4f;
                    Main.gore[smoke].velocity.X += i;
                    Main.gore[smoke].velocity.Y += j;
                    Main.gore[smoke].scale = Main.rand.NextFloat(0.5f, 0.75f);
                }
            }
            for (int i = 0; i < 15; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption);
                d.velocity = Main.rand.NextVector2Circular(6, 6);
                //d.scale = Main.rand.NextFloat(0.5f, 1f);
                d.noGravity = true;
            }
        }
    }
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 4;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.TrailCacheLength[Type] = 3;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
    }
    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        if (NPC.frameCounter > 5)
        {
            NPC.frameCounter = 0;
            NPC.frame.Y += frameHeight;
            if (NPC.frame.Y > frameHeight * 3)
            {
                NPC.frame.Y = 0;
            }
        }
    }
    public override void SetDefaults()
    {
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;

        NPC.lifeMax = 130;
        NPC.defense = 20;
        NPC.damage = 60;
        NPC.knockBackResist = 0.4f;

        NPC.width = 34;
        NPC.height = 36;
        if(!NPC.IsABestiaryIconDummy)
            NPC.alpha = 255;

        NPC.value = 0;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath6;
    }
    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.lifeMax = (int)(NPC.lifeMax * 0.85f);
    }
    public override void AI()
    {
        int hoverDistance = 200;
        NPC.Opacity += 0.05f;
        NPC.rotation = MathHelper.Clamp(NPC.velocity.X / 5f, -1f, 1f);
        if (Main.rand.NextBool(6))
        {
            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption, Scale: 0.75f);
            d.velocity = Main.rand.NextVector2Circular(0.5f, 0.5f);
            d.alpha = 128;
        }

        NPC.TargetClosest();
        NPC.localAI[0]++;

        if (NPC.HasValidTarget)
        {
            if (NPC.justHit)
            {
                NPC.ai[0] = 0;
            }
            int FlyTime = 200;
            NPC.ai[0]++;
            if (NPC.ai[0] < FlyTime)
            {
                Vector2 targetPos = target.Top - new Vector2(0, hoverDistance);
                if(NPC.Center.Distance(targetPos) > 64)
                    NPC.SimpleFlyMovement(NPC.Center.DirectionTo(targetPos) * 6, 0.1f);
            }
            else if (NPC.ai[0] < FlyTime + 500)
            {
                if (NPC.ai[0] == FlyTime + 1)
                {
                    NPC.velocity *= 0.9f;
                }
                NPC.velocity.X *= 0.95f;
                if (target.position.Y < NPC.position.Y + hoverDistance && NPC.velocity.Y > -1)
                {
                    NPC.velocity.Y -= 0.03f;
                }
                else if (NPC.velocity.Y < 1)
                {
                    NPC.velocity.Y += 0.03f;
                }
                int vineType = ModContent.ProjectileType<HiveVine>();
                foreach(Projectile p in Main.ActiveProjectiles)
                {
                    if(p.type == vineType && Math.Abs(p.Center.X - NPC.Center.X) < 64)
                    {
                        NPC.velocity.X -= MathF.Sign(p.Center.X - NPC.Center.X) * 0.2f;
                    }
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] % 6 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom + new Vector2(Main.rand.NextFloat(-6, 6), 0), new Vector2(0, 5), ModContent.ProjectileType<HiveMindWeeperTears>(), 15, 1);
                }
            }
            else
            {
                NPC.ai[0] = 0;
            }
        }
        else
        {
            NPC.velocity.Y -= 0.2f;
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D tex = TextureAssets.Npc[NPC.type].Value;
        int FlyTime = 200;
        Color c = NPC.GetNPCColorTintedByBuffs(drawColor) * NPC.Opacity;
        for (int i = NPC.oldPos.Length - 1; i >= 0; i--)
        {
            float opacity = (1f - (i / (float)NPC.oldPos.Length)) * 0.5f;
            spriteBatch.Draw(tex, NPC.oldPos[i] - screenPos + NPC.Size / 2, NPC.frame, c * opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
        }
        spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, c, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
        for (int i = 3; i >= 0; i--)
        {
            float percent = Utils.Remap(NPC.ai[0], FlyTime - (i * 10) - 30, FlyTime - (i * 10), 0, 1);
            Color c2 = c with { A = 0 } * Utils.Remap(NPC.ai[0], FlyTime, FlyTime + 60, 0.5f, 0) * percent;
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, c2 * percent, NPC.rotation, NPC.frame.Size() / 2, NPC.scale + (1f - percent), SpriteEffects.None, 0);
        }
        if (NPC.Opacity < 1)
        {
            tex = TextureAssets.Extra[ExtrasID.ThePerfectGlow].Value;
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.Purple with { A = 0 } * (1f - NPC.Opacity), 0, tex.Size() / 2, 2f - NPC.Opacity, SpriteEffects.None, 0);
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.White with { A = 0 } * (1f - NPC.Opacity) * 0.5f, 0, tex.Size() / 2, 0.8f, SpriteEffects.None, 0);

            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.Purple with { A = 0 } * (1f - NPC.Opacity), MathHelper.PiOver2, tex.Size() / 2, 1.5f - NPC.Opacity, SpriteEffects.None, 0);
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.White with { A = 0 } * (1f - NPC.Opacity) * 0.5f, MathHelper.PiOver2, tex.Size() / 2, 0.6f, SpriteEffects.None, 0);
        }
        return false;
    }
    public override void OnKill()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(1, 1), ModContent.ProjectileType<Spores>(), 15, 2, ai0: Main.rand.NextFloat(MathF.PI * 10));
            }
        }
        // Boss minions typically have a chance to drop an additional heart item in addition to the default chance
        Player closestPlayer = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];

        if (Main.rand.NextBool(2) && closestPlayer.statLife < closestPlayer.statLifeMax2)
        {
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart);
        }
        int hive = NPC.FindFirstNPC(ModContent.NPCType<HiveMind>());
        if (hive != -1)
        {
            if (Main.npc[hive].ai[0] > 20 && Main.npc[hive].dontTakeDamage)
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HiveShieldBreaker>(), 0, 0, -1, hive);
        }
    }
}
public class HiveMindWeeperTears : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.RainNimbus);
        AIType = ProjectileID.RainNimbus;
        Projectile.timeLeft *= 3;
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 3; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Bottom,DustID.Corruption);
            d.velocity *= 0.3f;
            d.velocity.Y -= 0.5f;
            d.noGravity = true;
        }
    }
}