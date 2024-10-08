﻿using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla
{
    public static class CVUtils
    {
        public static void QuickDefaults(this Projectile proj, bool hostile = false, int size = 8, int aiStyle = -1)
        {
            proj.aiStyle = aiStyle;
            proj.hostile = hostile;
            proj.friendly = !hostile;
            proj.width = size;
            proj.height = size;
        }

        /// <summary>
        /// Clamps a Vector2 to be a specific length between max and min. Good for giving something a maximum speed.
        /// </summary>
        public static Vector2 LengthClamp(this Vector2 vector, float max, float min = 0)
        {
            if (vector.Length() > max) return Vector2.Normalize(vector) * max;
            else if (vector.Length() < min) return Vector2.Normalize(vector) * min;
            else return vector;
        }
        /// <summary>
        /// I should learn what this actually does at some point
        /// </summary>
        /// <param name="spriteWidth"></param>
        /// <param name="spriteHeight"></param>
        /// <param name="normalizedPointOnPath"></param>
        /// <param name="itemScale"></param>
        /// <param name="location"></param>
        /// <param name="outwardDirection"></param>
        /// <param name="player"></param>
        public static void GetPointOnSwungItemPath(float spriteWidth, float spriteHeight, float normalizedPointOnPath, float itemScale, out Vector2 location, out Vector2 outwardDirection, Player player)
        {
            float num = (float)Math.Sqrt(spriteWidth * spriteWidth + spriteHeight * spriteHeight);
            float num2 = (float)(player.direction == 1).ToInt() * ((float)Math.PI / 2f);
            if (player.gravDir == -1f)
            {
                num2 += (float)Math.PI / 2f * (float)player.direction;
            }
            outwardDirection = player.itemRotation.ToRotationVector2().RotatedBy(3.926991f + num2);
            location = player.RotatedRelativePoint(player.itemLocation + outwardDirection * num * normalizedPointOnPath * itemScale);
        }
        /// <summary>
        /// Sets defaults to regular sword stuff.
        /// item.useStyle = ItemUseStyleID.Swing;
        /// item.DamageType = DamageClass.Melee;
        /// item.damage = Damage;
        /// item.useTime = UseTime;
        /// item.useAnimation = UseTime;
        /// item.knockBack = Knockback;
        /// item.UseSound = SoundID.Item1;
        /// item.Size = new Vector2(16, 16);
        /// </summary>
        public static void DefaultToSword(this Item item, int Damage, int UseTime, float Knockback)
        {
            item.useStyle = ItemUseStyleID.Swing;
            item.DamageType = DamageClass.Melee;
            item.damage = Damage;
            item.useTime = UseTime;
            item.useAnimation = UseTime;
            item.knockBack = Knockback;
            item.UseSound = SoundID.Item1;
            item.Size = new Vector2(16, 16);
        }
        //public static PlayerStats PlayerStats(this Player player)
        //{
        //    return player.GetModPlayer<PlayerStats>();
        //}
        public static bool PlayerDoublePressedSetBonusActivateKey(this Player player)
        {
            return (player.doubleTapCardinalTimer[Main.ReversedUpDownArmorSetBonuses ? 1 : 0] < 15 && ((player.releaseUp && Main.ReversedUpDownArmorSetBonuses && player.controlUp) || (player.releaseDown && !Main.ReversedUpDownArmorSetBonuses && player.controlDown)));
        }
        //public static bool IsTrueMeleeProjectile(this Projectile projectile)
        //{
        //    return projectile.DamageType == DamageClass.Melee && (projectile.aiStyle == ProjAIStyleID.Spear || projectile.aiStyle == ProjAIStyleID.ShortSword || projectile.aiStyle == ProjAIStyleID.NightsEdge || projectile.type == ProjectileID.Terragrim || projectile.type == ProjectileID.Arkhalis || ProjectileSets.TrueMeleeProjectiles[projectile.type]);
        //}
        public static void Explode(this Projectile projectile, int Diameter)
        {
            projectile.ResetLocalNPCHitImmunity();
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.Resize(Diameter, Diameter);
            projectile.Damage();
        }

        public static int TypeCountNPC(int type)
        {
            int found = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].type == type) found++;
            }
            return found;
        }
        public static int TypeCountProjectile(int type)
        {
            int found = 0;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == type) found++;
            }
            return found;
        }

        /// <summary>
        /// Finds the closest NPC to the given position and returns that NPC. If no NPC can be found, returns null
        /// </summary>
        public static NPC FindClosestNPC(float maxDetectDistance, Vector2 position, bool HostileOnly = true, NPC[] excludedNPCs = null, bool TargetThroughWalls = true)
        {
            NPC closestNPC = null;

            float MaxDetectDistance = maxDetectDistance;

            for (int k = 0; k < Main.npc.Length; k++)
            {
                NPC target = Main.npc[k];

                if (target.CanBeChasedBy() && (!HostileOnly || !target.friendly) && target != null && target.lifeMax > 5 && (!TargetThroughWalls ? Collision.CanHitLine(position - new Vector2(4), 8, 8, target.position, target.width, target.height) : true))
                {
                    if (excludedNPCs != null && !excludedNPCs.Contains(target))
                    {
                        float DistanceToTarget = Vector2.Distance(target.Center, position);

                        if (DistanceToTarget < MaxDetectDistance)
                        {
                            MaxDetectDistance = DistanceToTarget;
                            closestNPC = target;
                        }
                    }
                    else if (excludedNPCs == null)
                    {
                        float DistanceToTarget = Vector2.Distance(target.Center, position);

                        if (DistanceToTarget < MaxDetectDistance)
                        {
                            MaxDetectDistance = DistanceToTarget;
                            closestNPC = target;
                        }
                    }
                }
            }

            return closestNPC;

        }

        /// <summary>
        /// Gets all the NPCs in an Area
        /// </summary>
        public static NPC[] GetAllNPCsInArea(float AreaSize, Vector2 position, bool HostileOnly = true, NPC[] excludedNPCs = null)
        {
            NPC[] AreaNPCs = new NPC[] { };

            for (int k = 0; k < Main.npc.Length; k++)
            {
                NPC target = Main.npc[k];

                if (target.CanBeChasedBy() && (!HostileOnly || !target.friendly) && target != null)
                {
                    if (excludedNPCs != null && !excludedNPCs.Contains(target))
                    {
                        if (Vector2.Distance(target.Center, position) < AreaSize)
                        {
                            AreaNPCs = AreaNPCs.Append(target).ToArray();
                        }
                    }
                    else if (excludedNPCs == null)
                    {
                        if (Vector2.Distance(target.Center, position) < AreaSize)
                        {
                            AreaNPCs = AreaNPCs.Append(target).ToArray();
                        }
                    }

                }
            }

            return AreaNPCs;

        }

        public static bool NotPreBoss(bool includeKingSlime = true)
        {
            if (includeKingSlime)
                return (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedSlimeKing || Main.hardMode);
            return (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || Main.hardMode);
        }

        public static Vector2 FindRestingSpot(Vector2 pointPoisition)
        {
            bool flag = false;
            int num = (int)pointPoisition.X / 16;
            int i = (int)pointPoisition.Y / 16;
            float worldX = num * 16 + 8;


            if (!flag)
            {
                for (; i < Main.maxTilesY - 10 && Main.tile[num, i] != null && !WorldGen.SolidTile2(num, i) && Main.tile[num - 1, i] != null && !WorldGen.SolidTile2(num - 1, i) && Main.tile[num + 1, i] != null && !WorldGen.SolidTile2(num + 1, i); i++)
                {
                }

                i++;
            }

            i--;
            float worldY = i * 16;

            return new Vector2(worldX, worldY);
        }

        //Creates and returns a 16 byte array of replicable random values based on an MD5 hash
        public static byte[] RepeatableRandom(string seed)
        {
            byte[] input = System.Text.Encoding.ASCII.GetBytes(seed);
            MD5 hash = MD5.Create();
            byte[] output = hash.ComputeHash(input);
            return output;
        }
    }
}
