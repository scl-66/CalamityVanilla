using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.NPCs;

// ReSharper once CompareOfFloatsByEqualityOperator

internal abstract class CustomWormNPC : ModNPC
{
    internal enum PartKind
    {
        Head,
        Body,
        Tail
    }

    internal struct PartData()
    {
        public PartKind Kind;
    }

    public PartData Data;
    
    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => NPC.ai[1] == (byte)PartKind.Head;
}