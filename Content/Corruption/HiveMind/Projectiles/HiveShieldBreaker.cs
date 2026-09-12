using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Corruption.HiveMind.Projectiles;

public class HiveShieldBreaker : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.AmethystBolt}";
    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = Projectile.height = 8;
        Projectile.hide = true;
        Projectile.extraUpdates = 2;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2);
        d.velocity = Vector2.Zero;
        d.color = Color.Purple;
        d.noGravity = true;
        NPC target = Main.npc[(int)Projectile.ai[0]];
        Projectile.Center = Projectile.Center.MoveTowards(target.Center, 3);
        if (Projectile.Hitbox.Intersects(target.Hitbox))
        {
            for (int i = 0; i < 15; i++)
            {
                Dust d2 = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2);
                d2.velocity *= 5;
                d2.color = Color.Purple;
                d2.noGravity = true;
            }
            target.ai[3]--;
            Projectile.Kill();
        }
    }
}