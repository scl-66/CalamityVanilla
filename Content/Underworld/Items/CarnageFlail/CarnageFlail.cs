using CalamityVanilla.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Underworld.Items.CarnageFlail;

// ExampleFlail and ExampleFlailProjectile show the minimum amount of code needed for a flail using the existing vanilla code and behavior. ExampleAdvancedFlail and ExampleAdvancedFlailProjectile need to be consulted if more advanced customization is desired, or if you want to learn more advanced modding techniques.
// ExampleFlail is a copy of the Sunfury flail weapon.
public class CarnageFlail : ModItem
{
    public override void SetStaticDefaults()
    {
        // This line will make the damage shown in the tooltip twice the actual Item.damage. This multiplier is used to adjust for the dynamic damage capabilities of the projectile.
        // When thrown directly at enemies, the flail projectile will deal double Item.damage, matching the tooltip, but deals normal damage in other modes.
        ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
    }

    public override void SetDefaults()
    {
        // These default values aside from Item.shoot match the Sunfury values, feel free to tweak them.
        Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
        Item.useAnimation = 45; // The item's use time in ticks (60 ticks == 1 second.)
        Item.useTime = 40; // The item's use time in ticks (60 ticks == 1 second.)
        Item.knockBack = 5.75f; // The knockback of your flail, this is dynamically adjusted in the projectile code.
        Item.width = 40; // Hitbox width of the item.
        Item.height = 50; // Hitbox height of the item.
        Item.damage = 40; // The damage of your flail, this is dynamically adjusted in the projectile code.
        Item.scale = 1.1f;
        Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
        Item.shoot = ModContent.ProjectileType<CarnageFlailProjectile>(); // The flail projectile
        Item.shootSpeed = 13f; // The speed of the projectile measured in pixels per frame.
        Item.UseSound = SoundID.Item1; // The sound that this item makes when used
        Item.rare = ItemRarityID.LightRed; // The color of the name of your item
        Item.value = Item.sellPrice(gold: 3); // Sells for 2 gold 50 silver
        Item.DamageType = DamageClass.MeleeNoSpeed; // Deals melee damage
        Item.channel = true;
        Item.noMelee = true; // This makes sure the item does not deal damage from the swinging animation
    }
}

public class CarnageFlailProjectile : BaseFlailProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(30, 30);
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        Projectile.netImportant = true;
    }

    public override int MaxTimeLaunched => 13;
    public override float LaunchSpeed => 15;
    public override float MaxManualRetractionSpeed => 13;
    public override float MaxForcedRetractionSpeed => 16;
    public override int SpinningNPCHitCooldown => 12;

    public override bool ShouldFreelyRotate => false;
    public ref float projTarget => ref Projectile.ai[2];
    public int projStuckTimer;
    public bool isProjStuck;
    /*public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (CurrentAIState == AIState.LaunchingForward && target.CanBeChasedBy())
        {
            if (projStuckTimer == 180 && !isProjStuck)
            {
                isProjStuck = true;
            }

            if (projStuckTimer > 0 && isProjStuck)
            {
                projStuckTimer--;
                Projectile.netUpdate = true;

                Projectile.ai[2] = target.whoAmI;
                Projectile.velocity = Projectile.Center - target.Center;
            }

            if (projStuckTimer <= 0 && isProjStuck)
            {
                isProjStuck = false;
                projStuckTimer = 180;
            }
        }
    }

    public override void AI()
    {
        if (projStuckTimer > 0 && isProjStuck)
        {
            Projectile.Center = Main.npc[(int)projTarget].Center + Projectile.velocity * 0.8f;
        }

        if (!Main.npc[(int)projTarget].active || Projectile.timeLeft <= 10)
        {
            projTarget = -1;
        }
    }*/
}