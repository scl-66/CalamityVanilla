using System;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityVanilla.Common.World;

public abstract class CVGenPass : GenPass, ILoadable
{
    private Mod _mod;
    public virtual LocalizedText ProgressMessage => _mod.GetLocalization("WorldGen." + GetType().Name, () => "LOCALIZE ME PLEASE");
    public virtual bool BeforeParent => false;
    public virtual string Parent => "";
    public virtual string Name => GetType().Name;
    public virtual double LoadWeight => 100;
    public CVGenPass() : base("", 0)
    {
        base.Name = Name;
        Weight = LoadWeight;
    }
    public void Load(Mod mod)
    {
        _mod = mod;
        _ = ProgressMessage;
        GenPassSystem.Passes.Add(this);
    }
    public void Unload()
    {
    }
}
internal class GenPassSystem : ModSystem
{
    internal static List<CVGenPass> Passes = new();
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        for(int i = 0; i < Passes.Count; i++)
        {
            int index = tasks.FindIndex(genpass => genpass.Name.Equals(Passes[i].Parent));
            if(index != -1)
            {
                tasks.Insert(index + (Passes[i].BeforeParent ? 0 : 1), Passes[i]);
            }
        }
    }
}
