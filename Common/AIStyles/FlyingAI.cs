using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;

namespace CalamityVanilla.Common.AIStyles;

public static class FlyingAI
{
    public static void AI(NPC n, float speed = 6, float acceleration = 0.05f, float bounceStrength = 0.7f, bool occasionalStrongAcceleration = false)
    {
        NPCAimedTarget targetData = n.GetTargetData();
        bool flag = false;
        if (targetData.Type == NPCTargetType.Player)
            flag = Main.player[n.target].dead;

        Vector2 center = new Vector2(n.position.X + (float)n.width * 0.5f, n.position.Y + (float)n.height * 0.5f);
        float targetCenterDistanceX = targetData.Position.X + (float)(targetData.Width / 2);
        float TargetCenterDistanceY = targetData.Position.Y + (float)(targetData.Height / 2);
        targetCenterDistanceX = (int)(targetCenterDistanceX / 8f) * 8;
        TargetCenterDistanceY = (int)(TargetCenterDistanceY / 8f) * 8;
        center.X = (int)(center.X / 8f) * 8;
        center.Y = (int)(center.Y / 8f) * 8;
        targetCenterDistanceX -= center.X;
        TargetCenterDistanceY -= center.Y;
        float num6 = (float)Math.Sqrt(targetCenterDistanceX * targetCenterDistanceX + TargetCenterDistanceY * TargetCenterDistanceY);  // distance from target
        //float num7 = num6;
        //bool flag2 = false;
        //if (num6 > 600f)
        //    flag2 = true;

        if (num6 == 0f)
        {
            targetCenterDistanceX = n.velocity.X;
            TargetCenterDistanceY = n.velocity.Y;
        }
        else
        {
            num6 = speed / num6;
            targetCenterDistanceX *= num6;
            TargetCenterDistanceY *= num6;
        }

        if (flag)
        {
            targetCenterDistanceX = (float)n.direction * speed / 2f;
            TargetCenterDistanceY = (0f - speed) / 2f;
        }

        if (n.velocity.X < targetCenterDistanceX)
        {
            n.velocity.X += acceleration;
            if (occasionalStrongAcceleration && n.velocity.X < 0f && targetCenterDistanceX > 0f)
                n.velocity.X += acceleration;
        }
        else if (n.velocity.X > targetCenterDistanceX)
        {
            n.velocity.X -= acceleration;
            if (occasionalStrongAcceleration && n.velocity.X > 0f && targetCenterDistanceX < 0f)
                n.velocity.X -= acceleration;
        }

        if (n.velocity.Y < TargetCenterDistanceY)
        {
            n.velocity.Y += acceleration;
            if (occasionalStrongAcceleration && n.velocity.Y < 0f && TargetCenterDistanceY > 0f)
                n.velocity.Y += acceleration;
        }
        else if (n.velocity.Y > TargetCenterDistanceY)
        {
            n.velocity.Y -= acceleration;
            if (occasionalStrongAcceleration && n.velocity.Y > 0f && TargetCenterDistanceY < 0f)
                n.velocity.Y -= acceleration;
        }

        if(bounceStrength != 0)
        {
            if (n.collideX)
            {
                n.netUpdate = true;
                n.velocity.X = n.oldVelocity.X * (0f - bounceStrength);
                if (n.direction == -1 && n.velocity.X > 0f && n.velocity.X < 2f)
                    n.velocity.X = 2f;

                if (n.direction == 1 && n.velocity.X < 0f && n.velocity.X > -2f)
                    n.velocity.X = -2f;
            }

            if (n.collideY)
            {
                n.netUpdate = true;
                n.velocity.Y = n.oldVelocity.Y * (0f - bounceStrength);
                if (n.velocity.Y > 0f && (double)n.velocity.Y < 1.5)
                    n.velocity.Y = 2f;

                if (n.velocity.Y < 0f && (double)n.velocity.Y > -1.5)
                    n.velocity.Y = -2f;
            }
        }

        if (((n.velocity.X > 0f && n.oldVelocity.X < 0f) || (n.velocity.X < 0f && n.oldVelocity.X > 0f) || (n.velocity.Y > 0f && n.oldVelocity.Y < 0f) || (n.velocity.Y < 0f && n.oldVelocity.Y > 0f)) && !n.justHit)
            n.netUpdate = true;
    }
}
