using CalamityVanilla.Content.Bosses.HiveMind.Projectiles;
using CalamityVanilla.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Minions;

public class HiveMindSwooper : ModNPC
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
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(Name + $"{i}").Type);
            }
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
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            PortraitPositionYOverride = -24f,
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.TrailingMode[Type] = 0;
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
        NPC.knockBackResist = 0.4f;

        NPC.lifeMax = 200;
        NPC.defense = 20;
        NPC.damage = 60;

        NPC.width = 34;
        NPC.height = 36;
        if(!NPC.IsABestiaryIconDummy)
            NPC.alpha = 255;

        NPC.value = 0;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath6;
    }
    private const int _swoopAttackTime = 120;
    public override void AI()
    {
        NPC.Opacity += 0.05f;
        NPC.rotation = Utils.AngleTowards(NPC.rotation, NPC.ai[2] > _swoopAttackTime ? NPC.velocity.ToRotation() + MathHelper.PiOver2: MathHelper.Clamp(NPC.velocity.X / 10f,-0.3f,0.3f), 0.1f);

        NPC.ai[0]++;
        if (NPC.ai[0] < 0)
        {
            return;
        }

        if (Main.rand.NextBool(6))
        {
            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption, Scale: 0.75f);
            d.velocity = Main.rand.NextVector2Circular(0.5f, 0.5f);
            d.alpha = 128;
        }
        if (NPC.HasValidTarget)
        {
            if (NPC.velocity.Y is < -8)
                NPC.velocity.Y += 0.5f;

            int swoopInterval = 300;
            int swoopTime = 30;
            float swoopUpwardsAcceleration = -0.4f;
            if (NPC.ai[0] < swoopInterval)
            {
                if (NPC.localAI[0] > 0)
                {
                    NPC.localAI[0] -= 0.05f;
                }
                NPC.direction = target.Center.X > NPC.Center.X ? 1 : -1;
                NPC.directionY = target.Center.Y > NPC.Center.Y ? 1 : -1;
                float distance = NPC.Center.Distance(target.Center) / 16;
                if (distance < 40)
                {
                    NPC.SimpleFlyMovement(new Vector2(NPC.direction * 2, NPC.directionY * (NPC.directionY == 1 ? 1.5f : 1f)), 0.05f);
                }
                else
                {
                    if (distance > 60 && NPC.ai[0] == swoopInterval - 5)
                    {
                        NPC.ai[0]--;
                    }
                    float speed = Utils.Remap(distance, 40, 120, 6, 24);
                    NPC.SimpleFlyMovement(new Vector2(NPC.direction * speed, NPC.directionY * speed), 0.2f);
                }
            }
            else
            {
                if (NPC.ai[1] == 0)
                {
                    NPC.ai[2]++;
                    if (NPC.ai[2] == _swoopAttackTime)
                    {
                        Vector2 adjustedTargetPosition = target.Center + (target.velocity * swoopTime);
                        NPC.velocity = CVUtils.FindVelocityForGravityAffectedThing(NPC.Bottom, adjustedTargetPosition, swoopUpwardsAcceleration, swoopTime).LengthClamp(24);
                        for(int i = 0; i < 15; i++)
                        {
                            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption, Scale: 0.75f);
                            d.velocity = Main.rand.NextVector2CircularEdge(3, 1).RotatedBy(NPC.rotation);
                            d.fadeIn = Main.rand.NextFloat(2);
                            d.alpha = 128;
                        }
                        SoundEngine.PlaySound(SoundID.Item131 with { MaxInstances = 10, Pitch = 0.1f, PitchVariance = 0.2f}, NPC.position);
                        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                    }
                    else if (NPC.ai[2] > _swoopAttackTime)
                    {
                        for(int i = -1; i < 2; i += 2)
                        {
                            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Corruption, Scale: 0.75f);
                            d.velocity = NPC.velocity.RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * i) * 0.1f;
                            d.alpha = 128;
                        }
                        NPC.velocity.Y += swoopUpwardsAcceleration;
                        if(NPC.Center.Distance(target.Center) > 16 * 80)
                        {
                            NPC.ai[2]+= 2;
                            NPC.velocity *= 0.98f;
                        }
                        if (NPC.ai[2] > 120 + (swoopTime * 2))
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }
                    }
                    else if(NPC.ai[2] < _swoopAttackTime)
                    {
                        NPC.localAI[0] += 0.75f / _swoopAttackTime;
                        NPC.velocity = Vector2.Normalize(NPC.velocity.RotatedBy(0.1f * NPC.direction)) * 4;
                    }
                }
            }
        }
        else
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                NPC.ai[0] = 0;
                NPC.velocity.Y -= 0.2f;
            }
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Color c = NPC.GetNPCColorTintedByBuffs(drawColor) * NPC.Opacity;
        Texture2D tex = TextureAssets.Npc[NPC.type].Value;
        if (NPC.localAI[0] > 0)
        {
            for (int i = NPC.oldPos.Length - 1; i >= 0; i--)
            {
                float opacity = (1f - (i / (float)NPC.oldPos.Length)) * NPC.localAI[0];
                spriteBatch.Draw(tex, NPC.oldPos[i] - screenPos + NPC.Size / 2, NPC.frame, c * opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            }
        }
        spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, c * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
        for (int i = 3; i >= 0; i--)
        {
            float percent = Utils.Remap(NPC.ai[2], _swoopAttackTime - (i * 10) - 30, _swoopAttackTime - (i * 10), 0, 1);
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, c * percent, NPC.rotation, NPC.frame.Size() / 2, NPC.scale + (1f - percent), SpriteEffects.None, 0);
        }

        if(NPC.Opacity < 1)
        {
            tex = TextureAssets.Extra[ExtrasID.ThePerfectGlow].Value;
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.Purple with { A = 0} * (1f - NPC.Opacity), 0, tex.Size() / 2, 2f - NPC.Opacity, SpriteEffects.None, 0);
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.White with { A = 0 } * (1f - NPC.Opacity) * 0.5f, 0, tex.Size() / 2, 0.8f, SpriteEffects.None, 0);

            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.Purple with { A = 0 } * (1f - NPC.Opacity), MathHelper.PiOver2, tex.Size() / 2, 1.5f - NPC.Opacity, SpriteEffects.None, 0);
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.White with { A = 0 } * (1f - NPC.Opacity) * 0.5f, MathHelper.PiOver2, tex.Size() / 2, 0.6f, SpriteEffects.None, 0);
        }
        return false;
    }
    public override void OnKill()
    {
        // Boss minions typically have a chance to drop an additional heart item in addition to the default chance
        Player closestPlayer = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];

        if (Main.rand.NextBool(2) && closestPlayer.statLife < closestPlayer.statLifeMax2)
        {
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart);
        }
        int hive = NPC.FindFirstNPC(ModContent.NPCType<HiveMind>());
        if (Main.npc[hive].ai[0] > 20)
            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HiveShieldBreaker>(), 0, 0, -1, hive);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Makes it so whenever you beat the boss associated with it, it will also get unlocked immediately
        int associatedNPCType = ModContent.NPCType<HiveMind>();
        bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            new FlavorTextBestiaryInfoElement($"Mods.CalamityVanilla.NPCs.HiveMindSwooper.Bestiary")
        });
    }
}