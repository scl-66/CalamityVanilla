using Terraria;

namespace CalamityVanilla.Common.AIStyles;

public static class BatAI
{
    public static void BounceOffWalls(NPC n)
    {
        if (n.collideX)
        {
            n.velocity.X = n.oldVelocity.X * -0.5f;
            if (n.direction == -1 && n.velocity.X > 0f && n.velocity.X < 2f)
            {
                n.velocity.X = 2f;
            }
            if (n.direction == 1 && n.velocity.X < 0f && n.velocity.X > -2f)
            {
                n.velocity.X = -2f;
            }
        }
        if (n.collideY)
        {
            n.velocity.Y = n.oldVelocity.Y * -0.5f;
            if (n.velocity.Y > 0f && n.velocity.Y < 1f)
            {
                n.velocity.Y = 1f;
            }
            if (n.velocity.Y < 0f && n.velocity.Y > -1f)
            {
                n.velocity.Y = -1f;
            }
        }
    }
    /// <summary>
    /// NPC.AI[1] is used.
    /// </summary>
    public static void AI(NPC n, 
        float XSpeed = 4f, float YSpeed = 1.5f, float XAcceleration = 0.1f, float YAcceleration = 0.04f,
        float AggroXSpedd = 4f, float AggroYSpeed = 1.5f, float AggroXAcceleration = 0.2f, float AggroYAcceleration = 0.1f)
    {
        BounceOffWalls(n);
        if (n.direction == -1 && n.velocity.X > 0f - XSpeed)
        {
            n.velocity.X -= XAcceleration;
            if (n.velocity.X > XSpeed)
                n.velocity.X -= XAcceleration;
            else if (n.velocity.X > 0f)
                n.velocity.X += XAcceleration * 0.5f;

            if (n.velocity.X < 0f - XSpeed)
                n.velocity.X = 0f - XSpeed;
        }
        else if (n.direction == 1 && n.velocity.X < XSpeed)
        {
            n.velocity.X += XAcceleration;
            if (n.velocity.X < 0f - XSpeed)
                n.velocity.X += XAcceleration;
            else if (n.velocity.X < 0f)
                n.velocity.X -= XAcceleration * 0.5f;

            if (n.velocity.X > XSpeed)
                n.velocity.X = XSpeed;
        }

        if (n.directionY == -1 && n.velocity.Y > 0f - YSpeed)
        {
            n.velocity.Y -= YAcceleration;
            if (n.velocity.Y > YSpeed)
                n.velocity.Y -= YAcceleration;
            else if (n.velocity.Y > 0f)
                n.velocity.Y += YAcceleration * 0.75f;

            if (n.velocity.Y < 0f - YSpeed)
                n.velocity.Y = 0f - YSpeed;
        }
        else if (n.directionY == 1 && n.velocity.Y < YSpeed)
        {
            n.velocity.Y += YAcceleration;
            if (n.velocity.Y < 0f - YSpeed)
                n.velocity.Y += YAcceleration;
            else if (n.velocity.Y < 0f)
                n.velocity.Y -= YAcceleration * 0.75f;

            if (n.velocity.Y > YSpeed)
                n.velocity.Y = YSpeed;
        }

        n.ai[1] += 1f;
        if (n.ai[1] > 200f)
        {
            if (!Main.player[n.target].wet && Collision.CanHit(n.position, n.width, n.height, Main.player[n.target].position, Main.player[n.target].width, Main.player[n.target].height))
            {
                n.ai[1] = 0f;
            }
            if (n.ai[1] > 1000f)
            {
                n.ai[1] = 0f;
            }
            n.ai[2] += 1f;
            if (n.ai[2] > 0f)
            {
                if (n.velocity.Y < AggroYSpeed)
                {
                    n.velocity.Y += AggroYAcceleration;
                }
            }
            else if (n.velocity.Y > 0f - AggroYSpeed)
            {
                n.velocity.Y -= AggroYAcceleration;
            }
            if (n.ai[2] < -150f || n.ai[2] > 150f)
            {
                if (n.velocity.X < AggroXSpedd)
                {
                    n.velocity.X += AggroXAcceleration;
                }
            }
            else if (n.velocity.X > 0f - AggroXSpedd)
            {
                n.velocity.X -= AggroXAcceleration;
            }
            if (n.ai[2] > 300f)
            {
                n.ai[2] = -300f;
            }
        }
        return;
    }
}
