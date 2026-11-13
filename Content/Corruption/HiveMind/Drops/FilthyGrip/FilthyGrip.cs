using CalamityVanilla.Common.Dusts;
using CalamityVanilla.Common.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Corruption.HiveMind.Drops.FilthyGrip;

[AutoloadEquip(EquipType.HandsOn)]
public class FilthyGrip : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 1;
        Item.value = 65000;
        Item.rare = ItemRarityID.Pink;
        Item.accessory = true;
        // Set other Item.X values here
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<FilthyGripCrit>().filthyGripEquipped = true;
    }
}
public class FilthyGripCrit : ModPlayer
{
    public bool filthyGripEquipped = false;
    public override void ResetEffects()
    {
        filthyGripEquipped = false;
    }
    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hit.Crit && filthyGripEquipped)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float randWidth = Main.rand.NextFloat(-target.width, target.width) / 8;
                float randHeight = Main.rand.NextFloat(-target.height, target.height) / 8;
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Hitbox.ClosestPointInRect(Player.Center), Vector2.Zero, ModContent.ProjectileType<FilthyGripSlash>(), 10, 0f, Main.myPlayer);
            }
        }
    }
    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hit.Crit && filthyGripEquipped)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float randWidth = Main.rand.NextFloat(-target.width, target.width) / 8;
                float randHeight = Main.rand.NextFloat(-target.height, target.height) / 8;
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Hitbox.ClosestPointInRect(proj.Center), Vector2.Zero, ModContent.ProjectileType<FilthyGripSlash>(), 10, 0f, Main.myPlayer);
            }
        }
    }
}
public class FilthyGripSlash : ModProjectile
{
    int frameSpeed = 2;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 6;
    }
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 30;
        Projectile.friendly = true;
        Projectile.timeLeft = 6 * frameSpeed;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 7;
        Projectile.DamageType = DamageClass.Default;
        Projectile.penetrate = 3;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        DrawOffsetX = 7;
        DrawOriginOffsetY = -10;
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.ScalingArmorPenetration += 1f;
    }
    public override Color? GetAlpha(Color lightColor)
    {
        return Color.Lerp(lightColor,Color.White,0.33f);
    }
    public override void AI()
    {
        if (++Projectile.frameCounter >= frameSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
        }

        Projectile.ai[0]++;

        if (Projectile.ai[0] <= 1)
        {
            SoundEngine.PlaySound(Assets.Sounds.FilthyGrip.Asset with { Volume = 0.7f, PitchVariance = 0.5f, MaxInstances = 5 }, Projectile.position);
            Projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            Vector2 speed = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), ModContent.DustType<SimpleColorableGlowyDust>(), speed * Main.rand.NextFloat(2, 8));
                d.noGravity = true;
                d.color = new Color(1f, 0.75f, 0.55f, 0.25f) * 0.75f;
                d.scale *= 0.8f;
                d.noLight = true;
            }
            //for (int i = -1; i < 2; i++)
            //{
            //    var s = VanillaParticles.RequestPrettySparkleParticle();
            //    s.TimeToLive = Main.rand.Next(10,20);
            //    s.ColorTint = new Color(1f, 0.55f, 0.55f) * 0.75f;
            //    s.Scale = new Vector2(2,i == 0? 2.3f : 1.5f);
            //    s.Rotation = Projectile.rotation;
            //    s.DrawHorizontalAxis = false;
            //    s.LocalPosition = Projectile.Center + speed.RotatedBy(MathHelper.PiOver2) * i * Main.rand.NextFloat(5,10);
            //    s.Velocity = speed * Main.rand.NextFloat(2,4);
            //    Main.ParticleSystem_World_OverPlayers.Add(s);
            //    s.FadeInEnd = 1;
            //    s.FadeOutStart = 1;
            //}
        }
    }
}