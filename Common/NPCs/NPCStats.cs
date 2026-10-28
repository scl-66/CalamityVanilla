using CalamityVanilla.Common.Attributes;
using CalamityVanilla.Common.DataSets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityVanilla.Common.NPCs;

public class NPCStats : RDGlobalNPC
{
    public override bool InstancePerEntity => true;
    [ResetDefaults(1f)]
    public float Speed = 1;
    [ResetDefaults(0)]
    public int DOTDamage = 0;
    public Color EffectColor = Color.White;
    public float[] SpeedUpdateCount = new float[2];
    public override void ResetEffects(NPC npc)
    {
        base.ResetEffects(npc);
        EffectColor = Color.White;
    }
    public override void Load()
    {
        On_NPC.AI += On_NPC_AI;
        On_NPC.FindFrame += On_NPC_FindFrame;
        On_NPC.UpdateCollision += On_NPC_UpdateCollision;
    }
    public static bool ImmuneToSlows(int npcType) => NPCSets.SlowResistant[npcType] || ContentSamples.NpcsByNetId[npcType].boss;
    private void On_NPC_UpdateCollision(On_NPC.orig_UpdateCollision orig, NPC self)
    {
        var gNPC = self.GetGlobalNPC<NPCStats>();
        if (self.noGravity)
        {
            self.velocity *= gNPC.Speed;
            orig(self);
            self.velocity /= gNPC.Speed;
        }
        else
        {
            self.velocity.X *= gNPC.Speed;
            orig(self);
            self.velocity.X /= gNPC.Speed;
        }
    }
    private void On_NPC_FindFrame(On_NPC.orig_FindFrame orig, NPC self)
    {
        if (self.IsABestiaryIconDummy)
        {
            orig(self);
            return;
        }
        var gNPC = self.GetGlobalNPC<NPCStats>();
        gNPC.SpeedUpdateCount[1] += gNPC.Speed;
        while (gNPC.SpeedUpdateCount[1] >= 1)
        {
            gNPC.SpeedUpdateCount[1]--;
            orig(self);
        }
    }
    private void On_NPC_AI(On_NPC.orig_AI orig, NPC self)
    {
        var gNPC = self.GetGlobalNPC<NPCStats>();
        //self.GravityMultiplier *= gNPC.Speed;
        //self.MaxFallSpeedMultiplier *= gNPC.Speed;
        gNPC.SpeedUpdateCount[0] += gNPC.Speed;
        while (gNPC.SpeedUpdateCount[0] >= 1)
        {
            gNPC.SpeedUpdateCount[0]--;
            orig(self);
        }
        if (self.noTileCollide)
        {
            self.position += self.velocity * (gNPC.Speed - 1f);
        }
    }
    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        for (int i = 0; i < SpeedUpdateCount.Length; i++)
        {
            binaryWriter.Write(SpeedUpdateCount[i]);
        }
    }
    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
    {
        for (int i = 0; i < SpeedUpdateCount.Length; i++)
        {
            SpeedUpdateCount[i] = binaryReader.ReadSingle();
        }
    }
    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        damage = Math.Max(DOTDamage, damage);
    }
    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        drawColor = drawColor.MultiplyRGBA(EffectColor);
    }
}
public class NPCStatsSystem : ModSystem
{
    public override void PostSetupContent()
    {
        List<int> debuffs = new List<int>();
        for (int i = 0; i < BuffSets.SpeedChangeBuff.Length; i++)
        {
            if (BuffSets.SpeedChangeBuff[i])
                debuffs.Add(i);
        }
        for (int i = 0; i < NPCSets.SlowResistant.Length; i++)
        {
            if (NPCStats.ImmuneToSlows(i))
            {
                foreach (var i2 in debuffs)
                    NPCID.Sets.SpecificDebuffImmunity[i][i2] = true;
            }
        }
    }
}
