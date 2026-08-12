using CalamityVanilla.Content.NPCs.TownNPCs.Priest;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityVanilla.Common.UI;

[Autoload(Side = ModSide.Client)]
public class CVTownNPCUI : ModSystem
{
    private static UserInterface _NPCPanelInterface;
    public static PriestUIState PriestUI;
    public static bool PriestUIOpen => _NPCPanelInterface.CurrentState == PriestUI;

    public static void ShowPriestUI(NPC npc)
    {
        PriestUI = new PriestUIState();
        PriestUI.Activate();
        _NPCPanelInterface?.SetState(PriestUI);
        PriestUI.Open(npc);
    }

    public static void ClosePanel()
    {
        _NPCPanelInterface?.SetState(null);
    }

    public override void Load()
    {
        if (!Main.dedServ)
        {
            _NPCPanelInterface = new UserInterface();

            PriestUI = new PriestUIState();
            PriestUI.Activate();
        }
    }
    private GameTime _lastUpdateUIGameTime;
    public override void UpdateUI(GameTime gameTime)
    {
        _lastUpdateUIGameTime = gameTime;
        if (_NPCPanelInterface?.CurrentState != null)
        {
            _NPCPanelInterface?.Update(gameTime);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int npcDialogIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
        if (npcDialogIndex != -1)
        {
            layers.Insert(npcDialogIndex - 1, new LegacyGameInterfaceLayer(
                "CalamityVanilla: Priest UI Panel",
                delegate
                {
                    if (_lastUpdateUIGameTime != null && _NPCPanelInterface?.CurrentState != null)
                    {
                        _NPCPanelInterface.Draw(Main.spriteBatch, new GameTime());
                    }
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }
}