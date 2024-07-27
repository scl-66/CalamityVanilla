using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Projectiles.Melee
{
    public class Icebreaker : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ThornChakram);
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        private void OnHitAnything()
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 rotation = Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 6 * i).RotatedByRandom(0.2f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + rotation * 24, rotation * Main.rand.NextFloat(3,4), ModContent.ProjectileType<IcebreakerIcicles>(), Projectile.damage / 4, 2, Projectile.owner);
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate != 100)
                return;
            OnHitAnything();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.penetrate != 100)
                return;
            OnHitAnything();
        }
    }
}
