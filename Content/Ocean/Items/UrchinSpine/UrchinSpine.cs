using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Ocean.Items.UrchinSpine;

public class UrchinSpine : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToThrownWeapon(ModContent.ProjectileType<UrchinSpineProj>(), 16, 7, true);
        Item.SetWeaponValues(25, 2);
        Item.value = 25;
        Item.noUseGraphic = true;
        Item.ammo = AmmoID.Dart;
    }
    public override void PickAmmo(Item weapon, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
    {
        damage *= 0.5f;
        speed *= 0.5f;
    }
}
public class UrchinSpineProj : ModProjectile
{
    public override LocalizedText DisplayName => ModContent.GetInstance<UrchinSpine>().DisplayName;
    public override string Texture => ModContent.GetInstance<UrchinSpine>().Texture;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
    }
    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.extraUpdates = 2;
    }
    public override void AI()
    {
        if (Projectile.ai[0] == 0)
        {
            Projectile.ai[2]++;
            if (Projectile.ai[2] > 40)
                Projectile.velocity.Y += 0.075f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Venom);
            d.noGravity = true;
            d.velocity *= 0.1f;
            d.velocity += Projectile.velocity * 0.2f;
            d.alpha = 128;
        }
        else
        {
            Projectile.hide = true;
            Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center + Projectile.velocity * 0.95f;
            Main.npc[(int)Projectile.ai[1]].AddBuff(ModContent.BuffType<UrchinSpineDebuff>(), 2, true);
            if (!Main.npc[(int)Projectile.ai[1]].active)
            {
                Projectile.Kill();
            }
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        // If attached to an NPC, draw behind tiles (and the npc) if that NPC is behind tiles, otherwise just behind the NPC.
        if (Projectile.ai[0] == 1)
        {
            int npcIndex = (int)Projectile.ai[1];
            if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
            {
                if (Main.npc[npcIndex].behindTiles)
                {
                    behindNPCsAndTiles.Add(index);
                }
                else
                {
                    behindNPCs.Add(index);
                }
            }
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return Projectile.ai[0] == 0;
    }
    private readonly Point[] _sticking = new Point[6];
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.timeLeft = 60 * 10;
        Projectile.damage = 0;
        Projectile.ai[0] = 1;
        Projectile.ai[1] = target.whoAmI;
        Projectile.velocity = Projectile.Center - target.Center;
        Projectile.netUpdate = true;
        Projectile.KillOldestJavelin(Projectile.whoAmI, Type, (int)Projectile.ai[1], _sticking);
    }
    public override void OnKill(int timeLeft)
    {
        for(int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TintablePaint);
            d.color = new Color(Main.rand.NextFloat(0.7f,1f), 0.3f, 1f);
            d.noGravity = true;
            d.scale *= 0.8f;
            d.velocity += Vector2.Normalize(Projectile.velocity);
        }
        if (timeLeft > 60 * 5)
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
    }
}
public class UrchinSpineDebuff : ModBuff
{
    public override string Texture => $"Terraria/Images/Buff_{BuffID.BoneJavelin}";
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.GetGlobalNPC<UrchinSpineDebuffNPC>().Active = true;
    }
}
public class UrchinSpineDebuffNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public bool Active = false;

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (Active)
        {
            int stacks = 0;
            int type = ModContent.ProjectileType<UrchinSpineProj>();
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == type && p.ai[1] == npc.whoAmI)
                    stacks++;
            }
            npc.lifeRegen -= 20 * stacks;
            damage = Math.Max(damage, 5 * stacks);
        }
    }
}