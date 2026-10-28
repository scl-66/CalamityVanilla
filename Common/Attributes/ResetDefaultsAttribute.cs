using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Attributes;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class ResetDefaultsAttribute : Attribute
{
    public object Default { get; }
    public ResetDefaultsAttribute(object reset)
    {
        Default = reset;
    }
}
public abstract class RDModPlayer : ModPlayer
{
    private FieldInfo[] _info;
    public RDModPlayer()
    {
        _info = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
    }
    public override void ResetEffects()
    {
        for (int i = 0; i < _info.Length; i++)
        {
            ResetDefaultsAttribute a = _info[i].GetCustomAttribute<ResetDefaultsAttribute>();
            if (a != null)
                _info[i].SetValue(this, a.Default);
        }
    }
}
public abstract class RDGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    private FieldInfo[] _info;
    public RDGlobalNPC()
    {
        _info = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
    }
    public override void ResetEffects(NPC npc)
    {
        for (int i = 0; i < _info.Length; i++)
        {
            ResetDefaultsAttribute a = _info[i].GetCustomAttribute<ResetDefaultsAttribute>();
            if (a != null)
                _info[i].SetValue(this, a.Default);
        }
    }
}
