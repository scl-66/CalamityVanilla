using CalamityVanilla.Common.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Extensions;

public static class ProjectileExtensions
{
    extension(Projectile proj)
    {
        public void GetFlailStats(out int launchTimeLimit, out float launchSpeed, out float maxLaunchLength, out float retractAcceleration, out float maxRetractSpeed, out float forcedRetractAcceleration, out float maxForcedRetractSpeed, out int ricochetTimeLimit, out float spinVisualDistance)
        {
            launchTimeLimit = 10;
            launchSpeed = 24;
            maxLaunchLength = 800;
            retractAcceleration = 3;
            maxRetractSpeed = 16;
            forcedRetractAcceleration = 6;
            maxForcedRetractSpeed = 48;
            ricochetTimeLimit = 15;
            spinVisualDistance = 30;
            ProjectileLoader.FlailStats(proj, ref launchTimeLimit, ref launchSpeed, ref maxLaunchLength, ref retractAcceleration, ref maxRetractSpeed, ref forcedRetractAcceleration, ref maxForcedRetractSpeed, ref ricochetTimeLimit, ref spinVisualDistance);
        }
        /// <summary>
        /// if you need multiple flail stats use GetFlailStats instead, since this just calls that.
        /// </summary>
        /// <returns></returns>
        public int GetFlailLaunchTimeLimit()
        {
            GetFlailStats(proj, out var i, out _, out _, out _, out _, out _, out _, out _, out _);
            return i;
        }
        /// <summary>
        /// if you need multiple flail stats use GetFlailStats instead, since this just calls that.
        /// </summary>
        /// <returns></returns>
        public float GetFlailLaunchSpeed()
        {
            GetFlailStats(proj, out _, out var i, out _, out _, out _, out _, out _, out _, out _);
            return i;
        }
        /// <summary>
        /// if you need multiple flail stats use GetFlailStats instead, since this just calls that.
        /// </summary>
        /// <returns></returns>
        public float GetFlailMaxLaunchLength()
        {
            GetFlailStats(proj, out _, out _, out var i, out _, out _, out _, out _, out _, out _);
            return i;
        }
        /// <summary>
        /// if you need multiple flail stats use GetFlailStats instead, since this just calls that.
        /// </summary>
        /// <returns></returns>
        public float GetFlailRetractAcceleration()
        {
            GetFlailStats(proj, out _, out _, out _, out var i, out _, out _, out _, out _, out _);
            return i;
        }
        /// <summary>
        /// if you need multiple flail stats use GetFlailStats instead, since this just calls that.
        /// </summary>
        /// <returns></returns>
        public float GetFlailMaxRetractSpeed()
        {
            GetFlailStats(proj, out _, out _, out _, out _, out var i, out _, out _, out _, out _);
            return i;
        }
        /// <summary>
        /// if you need multiple flail stats use GetFlailStats instead, since this just calls that.
        /// </summary>
        /// <returns></returns>
        public float GetFlailForcedRetractAcceleration()
        {
            GetFlailStats(proj, out _, out _, out _, out _,  out _, out var i,out _, out _, out _);
            return i;
        }
        /// <summary>
        /// if you need multiple flail stats use GetFlailStats instead, since this just calls that.
        /// </summary>
        /// <returns></returns>
        public float GetFlailForcedRetractSpeed()
        {
            GetFlailStats(proj, out _, out _, out _, out _, out _,  out _,out var i, out _, out _);
            return i;
        }
        /// <summary>
        /// if you need multiple flail stats use GetFlailStats instead, since this just calls that.
        /// </summary>
        /// <returns></returns>
        public int GetFlailRichochetTimeLimit()
        {
            GetFlailStats(proj, out _, out _, out _, out _, out _, out _,  out _,out var i, out _);
            return i;
        }
        /// <summary>
        /// if you need multiple flail stats use GetFlailStats instead, since this just calls that.
        /// </summary>
        /// <returns></returns>
        public float GetFlailSpinVisualDistance()
        {
            GetFlailStats(proj, out _, out _, out _, out _, out _, out _, out _, out _, out var i);
            return i;
        }
    }
}
