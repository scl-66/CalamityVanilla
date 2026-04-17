using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla;

public static class CVUtils
{
    public static void SimpleFlyMovement(this NPC n, Vector2 desiredVelocity, float moveSpeedX, float moveSpeedY)
    {
        if (n.velocity.X < desiredVelocity.X)
        {
            n.velocity.X += moveSpeedX;
            if (n.velocity.X < 0f && desiredVelocity.X > 0f)
                n.velocity.X += moveSpeedX;
        }
        else if (n.velocity.X > desiredVelocity.X)
        {
            n.velocity.X -= moveSpeedX;
            if (n.velocity.X > 0f && desiredVelocity.X < 0f)
                n.velocity.X -= moveSpeedX;
        }

        if (n.velocity.Y < desiredVelocity.Y)
        {
            n.velocity.Y += moveSpeedY;
            if (n.velocity.Y < 0f && desiredVelocity.Y > 0f)
                n.velocity.Y += moveSpeedY;
        }
        else if (n.velocity.Y > desiredVelocity.Y)
        {
            n.velocity.Y -= moveSpeedY;
            if (n.velocity.Y > 0f && desiredVelocity.Y < 0f)
                n.velocity.Y -= moveSpeedY;
        }
    }
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
        item.Size = new Vector2(32, 32);
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
        //projectile.localNPCImmunity[0] = 100;
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
    /// <summary>
    /// Finds the closest floor below a point in the world. If it can't find anything it'll return the lowest spot it could check.
    /// </summary>
    public static Vector2 FindFloorBelowIgnoringSolidTops(Vector2 pointPoisition, int maxCheckTileDistance)
    {
        int num = (int)pointPoisition.X / 16;
        int num2 = (int)pointPoisition.Y / 16;
        for (int i = 0; i < maxCheckTileDistance; i++)
        {
            if ((Main.tileSolid[Main.tile[num, num2 + i].TileType] && !Main.tileSolidTop[Main.tile[num, num2 + i].TileType] && Main.tile[num, num2 + i].HasTile && !Main.tile[num, num2 + i].IsActuated) || !WorldGen.InWorld(num, num2 + i, 5))
            {
                return new Vector2(num * 16 + 8, (num2 + i) * 16);
            }
        }
        return new Vector2(num * 16 + 8, (num2 + maxCheckTileDistance) * 16);
    }
    public static Vector2 FindFloorBelow(Vector2 pointPoisition, int maxCheckTileDistance)
    {
        int num = (int)pointPoisition.X / 16;
        int num2 = (int)pointPoisition.Y / 16;
        for (int i = 0; i < maxCheckTileDistance; i++)
        {
            if ((Main.tileSolid[Main.tile[num, num2 + i].TileType] && Main.tile[num, num2 + i].HasTile && !Main.tile[num, num2 + i].IsActuated) || !WorldGen.InWorld(num, num2 + i, 5))
            {
                return new Vector2(num * 16 + 8, (num2 + i) * 16);
            }
        }
        return new Vector2(num * 16 + 8, (num2 + maxCheckTileDistance) * 16);
    }

    //Creates and returns a 16 byte array of replicable random values based on an MD5 hash
    public static byte[] RepeatableRandom(string seed)
    {
        byte[] input = System.Text.Encoding.ASCII.GetBytes(seed);
        MD5 hash = MD5.Create();
        byte[] output = hash.ComputeHash(input);
        return output;
    }

    public static float AngleDifference(float fromangle, float toangle)
    {
        float from = Math.Abs(fromangle % MathHelper.TwoPi);
        float to = Math.Abs(toangle % MathHelper.TwoPi);
        return Math.Abs(from - to);
    }
    
    public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness) {
        Vector2 edge = end - start;
        float angle = (float)Math.Atan2(edge.Y, edge.X);
        float length = edge.Length();

        var tex = ModContent.Request<Texture2D>("CalamityVanilla/Assets/Textures/Pixel");
        
        spriteBatch.Draw(
            tex.Value,
            start,
            null,
            color,
            angle,
            Vector2.Zero,
            new Vector2(length, thickness),
            SpriteEffects.None,
            0
        );
    }
    
    public static void DrawPixel(SpriteBatch spriteBatch, Vector2 pos, Color color) {
        var tex = ModContent.Request<Texture2D>("CalamityVanilla/Assets/Textures/Pixel");
        
        spriteBatch.Draw(
            tex.Value,
            pos,
            new Rectangle(0, 0, 10, 10),
            color,
            0,
            Vector2.Zero,
            new Vector2(0, 0),
            SpriteEffects.None,
            0
        );
    }

    // Copied from vanilla as it's a private method (used for Finch Staff and Abigail & Storm Tiger counters)
    public static void GetGroupIndex(this Projectile thisProjectile, out int index, out int totalIndexesInGroup)
    {
        index = 0;
        totalIndexesInGroup = 0;
        for (int i = 0; i < 1000; i++)
        {
            Projectile projectile = Main.projectile[i];
            if (projectile.active && projectile.owner == thisProjectile.owner && projectile.type == thisProjectile.type)
            {
                if (thisProjectile.whoAmI > i)
                {
                    index++;
                }
                totalIndexesInGroup++;
            }
        }
    }
}