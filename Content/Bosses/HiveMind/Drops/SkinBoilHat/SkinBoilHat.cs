using CalamityVanilla.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityVanilla.Content.Bosses.HiveMind.Drops.SkinBoilHat;

[AutoloadEquip(EquipType.Face)]
public class SkinBoilHat : ModItem
{
    public override void SetStaticDefaults()
    {
        ArmorIDs.Face.Sets.PreventHairDraw[Item.faceSlot] = true;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.accessory = true;
        Item.rare = ItemRarityID.Pink;
        Item.expert = true;
        Item.sellPrice(gold: 5);
    }

    public override void UpdateEquip(Player player)
    {
        player.GetModPlayer<SkinBoilHatPlayer>().skinBoilHat = true;
        if (player.ownedProjectileCounts[ModContent.ProjectileType<SkinBoilHatRaincloud>()] <= 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<SkinBoilHatRaincloud>(), 0, 0, Main.myPlayer);
        }
    }
}
public class SkinBoilHatPlayer : ModPlayer
{
    public bool skinBoilHat = false;
    public override void ResetEffects()
    {
        skinBoilHat = false;
    }
}
public class SkinBoilHatRaincloud : ModProjectile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 6;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 54;
        Projectile.height = 28;
        Projectile.friendly = true;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        Vector2 position = player.Top - new Vector2(Projectile.width / 2, 60);
        position.Floor();

        if (!player.GetModPlayer<SkinBoilHatPlayer>().skinBoilHat)
            Projectile.Kill();

        Projectile.timeLeft = 2;
        //interpolated cloud movement to make it more fluid
        Projectile.position = new Vector2(MathHelper.Lerp(Projectile.oldPosition.X, position.X, 0.5f), MathHelper.Lerp(Projectile.position.Y, position.Y, 0.45f));
        if (++Projectile.frameCounter >= 10)
        {
            Projectile.frameCounter = 0;
            if (Projectile.frame++ >= 5)
                Projectile.frame = 0;
        }

        Vector2 rainPosition = Projectile.Bottom + new Vector2(Main.rand.Next(-Projectile.width / 3, Projectile.width / 3), 8);

        if (++Projectile.ai[0] >= 8)
        {
            Projectile.ai[0] = 0;
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), rainPosition, Vector2.Zero, ModContent.ProjectileType<SkinBoilHatRain>(), 8, 0, Projectile.owner);
        }
    }
}
public class SkinBoilHatRain : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 40;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Default;
        Projectile.penetrate = 5;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 180;
    }
    public override void AI()
    {
        Projectile.velocity.Y = 10f;
        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.CursedInferno, 120);
    }
    public override void OnKill(int timeLeft)
    {   
        Dust.NewDustPerfect(Projectile.Bottom, DustID.Rain, new(0, -1), 0, Color.Lime);
    }
    public override bool? CanCutTiles()
    {
        return false;
    }
}

public class SkinBoilHatDrawlayer : PlayerDrawLayer
{
    public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.Head);

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        if (drawInfo.shadow != 0)
            return false;

        if (player.face != EquipLoader.GetEquipSlot(Mod, "SkinBoilHat", EquipType.Face))
            return false;

        return true;
    }

    public override bool IsHeadLayer => true;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player drawPlayer = drawInfo.drawPlayer;

        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;

        Vector2 Position = drawInfo.helmetOffset +
            new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) +
            (float)(drawInfo.drawPlayer.width / 2)),
            (int)(drawInfo.Position.Y - Main.screenPosition.Y +
            (float)drawInfo.drawPlayer.height -
            (float)drawInfo.drawPlayer.bodyFrame.Height + 4f)) +
            drawInfo.drawPlayer.headPosition +
            drawInfo.headVect +
            new Vector2(drawPlayer.direction == 1 ? 1 : -1, drawPlayer.gravDir == 1 ? -13 : 24) +
            Main.OffsetsPlayerHeadgear[drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height] * drawPlayer.gravDir;

        Texture2D hatTexture = ModContent.Request<Texture2D>("CalamityVanilla/Content/Bosses/HiveMind/Drops/SkinBoilHat/SkinBoilHatEquip").Value;

        Rectangle hatFrame = hatTexture.Frame();

        DrawData item = new DrawData(hatTexture, Position, hatFrame, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, hatFrame.Size() / 2f, 1f, drawInfo.playerEffect);
        item.shader = drawInfo.cFace;
        drawInfo.DrawDataCache.Add(item);
        return;
    }
}
