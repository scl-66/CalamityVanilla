using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityVanilla.Common.Attributes;
using Microsoft.Xna.Framework;

namespace CalamityVanilla.Common.Templates;

public abstract class Flask : ModItem
{
    public override void Load()
    {
        Mod.AddContent(new FlaskBuff(Item.ModItem.Name + "Buff", Texture + "_Buff", this));
    }
    public virtual Color[] DrinkColors => [];
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 20;
        ItemID.Sets.DrinkParticleColors[Type] = DrinkColors;
    }
    public override void SetDefaults()
    {
        Item.UseSound = SoundID.Item3;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useTurn = true;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 14;
        Item.height = 24;
        Item.buffType = Mod.Find<ModBuff>(Item.ModItem.Name + "Buff").Type;
        Item.buffTime = Item.flaskTime;
        Item.value = Item.sellPrice(0, 0, 5);
        Item.rare = ItemRarityID.LightRed;
    }
    public virtual void OnHit(NPC target)
    {
    }
    public virtual void ItemEffects(Player player, Vector2 position, int width, int height)
    {
    }
    public virtual void ProjectileEffects(Projectile projectile, Vector2 position, int width, int height)
    {
    }
}
public class FlaskPlayer : RDModPlayer
{
    [ResetDefaults(null)]
    public Flask CurrentFlask = null;

    public override void MeleeEffects(Item item, Rectangle hitbox)
    {
        if (CurrentFlask == null)
            return;
        if (item.DamageType.CountsAsClass<MeleeDamageClass>() && !item.noMelee && !item.noUseGraphic)
            CurrentFlask.ItemEffects(Player, hitbox.TopLeft(),hitbox.Width,hitbox.Height);
    }
    public override void EmitEnchantmentVisualsAt(Projectile projectile, Vector2 boxPosition, int boxWidth, int boxHeight)
    {
        if (CurrentFlask == null)
            return;
        if ((projectile.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[projectile.type]) && !projectile.noEnchantments)
            CurrentFlask.ProjectileEffects(projectile, boxPosition,boxWidth,boxHeight);
    }
    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (CurrentFlask == null)
            return;
        if (item.DamageType.CountsAsClass<MeleeDamageClass>())
            CurrentFlask.OnHit(target);
    }
    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (CurrentFlask == null)
            return;
        if ((proj.DamageType.CountsAsClass<MeleeDamageClass>() || ProjectileID.Sets.IsAWhip[proj.type]) && !proj.noEnchantments)
            CurrentFlask.OnHit(target);
    }
}
public class FlaskBuff : ModBuff
{
    private string _nameOverride;
    private string _textureOverride;
    private Flask _sourceFlask;
    public FlaskBuff(string nameOverride, string textureOverride, Flask sourceFlask)
    {
        _nameOverride = nameOverride;
        _textureOverride = textureOverride;
        _sourceFlask = sourceFlask;
    }
    public override string Texture => _textureOverride;
    public override string Name => _nameOverride;
    public override void SetStaticDefaults()
    {
        BuffID.Sets.IsAFlaskBuff[Type] = true;
        Main.meleeBuff[Type] = true;
        Main.persistentBuff[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<FlaskPlayer>().CurrentFlask = _sourceFlask;
        player.MeleeEnchantActive = true;
    }
}
