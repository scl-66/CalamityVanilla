using CalamityVanilla.Common.Blessings;
using CalamityVanilla.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace CalamityVanilla.Content.NPCs.TownNPCs.Priest;

public class PriestUIState : UIState
{
    public NPC Npc
    {
        get;
        set;
    }
    public bool InRangeOfNPC()
    {
        bool isTalking = Main.LocalPlayer.TalkNPC == Npc;
        // Don't bother trying if no valid NPC has been selected yet.
        Rectangle validTalkArea = Utils.CenteredRectangle(Main.LocalPlayer.Center, new Vector2(Player.tileRangeX * 3f, Player.tileRangeY * 2f) * 16f);
        return validTalkArea.Intersects(Npc.Hitbox) && isTalking;
    }
    private UIText text; // Init later
    private UIPanel panel; // Init later
    private UIList list;
    public override void OnInitialize()
    {
        panel = new UIPanel();
        panel.Width.Set(500, 0);
        panel.Height.Set(190, 0);
        panel.HAlign = 0.5f;
        panel.Top.Set(-200, 0.5f);
        Append(panel);

        list = new UIList
        {
            Width = StyleDimension.Fill,
            Height = StyleDimension.Fill
        };
        panel.Append(list);
    }

    public void Open(NPC npc)
    {
        buttonList.Clear();
        BlessingSystem.InitializeBlessings();
        Npc = npc;
        list.Clear();
        list.AddRange(buttonList);
        //list.SetPadding(5);
        list.ListPadding = 6f;
    }

    public static List<BlessingButton> buttonList = [];

    private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        text.SetText("I was clicked!");
    }
    public static void AddBlessing(PriestBlessing blessing)
    {
        BlessingButton button = new BlessingButton(blessing);
        buttonList.Add(button);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch); // This ensures the Draw call is propagated to the children(s)

        // If this code is in the panel or container element, check it directly
        if (ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }
        // Otherwise, we can check a child element instead
        if (panel.ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }

        //if (text.IsMouseHovering || button.IsMouseHovering)
        //{
        //    UICommon.TooltipMouseText("String");
        //}
    }

    public override void Update(GameTime gameTime)
    {
        if (!InRangeOfNPC())
        {
            CVTownNPCUI.ClosePanel();
        }
        base.Update(gameTime);
    }

}
public class BlessingButton : UIElement
{
    private object _text;
    private object _desc;
    private object _stat;
    private UIPanel _uiPanel;
    private UIText _uiText;

    public string Text
    {
        get => _uiText?.Text ?? string.Empty;
        set => _text = _desc = _stat = value;
    }

    private UIElement image;
    public PriestBlessing blessing;

    public BlessingButton(PriestBlessing blessingObj) : base()
    {
        base.Width.Set(-80, 1f);
        base.Height.Set(50, 0);
        _text = blessingObj.DisplayName.Value?.ToString() ?? string.Empty;
        _desc = blessingObj.Description.Value?.ToString() ?? string.Empty;
        _stat = blessingObj.Stats.Value?.ToString() ?? string.Empty;
        blessing = blessingObj;

        _uiPanel = new UIPanel
        {
            Width = StyleDimension.Fill,
            Height = StyleDimension.Fill 
        };
        _uiPanel.SetPadding(0);
        Append(_uiPanel);

        _uiText = new UIText(""); 
        _uiText.VAlign = 0.5f;
        _uiText.Left.Set(20, 0.08f);
        _uiPanel.Append(_uiText);

        image = new UIElement();
        image.Width.Set(30, 0f);
        image.Height.Set(30, 0f);
        image.HAlign = 0.03f;
        image.VAlign = 0.5f;
        image.OnDraw += DrawIcon;
        _uiPanel.Append(image);

        UITributeList list = new UITributeList(blessingObj.TributeList)
        {
            HAlign = 0.9f,
            VAlign = 0.5f
        };
        _uiPanel.Append(list);

        //_uiPanel.OnLeftClick += _clickAction;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        if (IsMouseHovering)
        {
            UICommon.TooltipMouseText(_desc.ToString() + "\n" + _stat.ToString());
        }
    }

    private void DrawIcon(UIElement affectedElement)
    {
        Texture2D tex = blessing.Icon.Value;
        Main.spriteBatch.Draw(tex, affectedElement.GetDimensions().Center(), null, Color.White, 0, tex.Size()/2f, 1f, SpriteEffects.None, 0);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime); // Propagate update to child elements.
        if (_text != null)
        {              
            _uiText.SetText(_text.ToString());
            _text = null;
        }

        if (_uiPanel.IsMouseHovering)
        {
            _uiPanel.BackgroundColor = new Color(70, 95, 165);
            _uiPanel.BorderColor = Main.OurFavoriteColor;
        }
        else
        {
            _uiPanel.BackgroundColor = Colors.InventoryDefaultColorWithOpacity;
            _uiPanel.BorderColor = Color.Black;
        }
    }
}

public class UITribute : UIItemIcon
{
    private UIText text;
    public UITribute (Tribute tribute) : base(ContentSamples.ItemsByType[tribute.ItemType], false)
    {
        text = new UIText(LocalizedText.Empty, 0.8f);
        text.SetText(tribute.Stack.ToString());
        text.SetPadding(0);
        text.HAlign = 1f;
        text.VAlign = 1f;
        text.Width.Set(10f, 0);
        text.Height.Set(10f, 0);
        text.IgnoresMouseInteraction = true;
        _item.stack = tribute.Stack;
        Append(text);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        if (GetDimensions().ToRectangle().Contains(Main.MouseScreen.ToPoint()))
        {
            Main.HoverItem = _item;
            Main.hoverItemName = _item.Name;
            Main.instance.MouseText("", 0, 0);
            Main.mouseText = true;
        }
    }
}

public sealed class UITributeList : UIElement
{
    public UITributeList(Tribute[] tributes)
    {
        float size = 0;
        foreach (var tribute in tributes)
        {
            UIPanel panel = new UIPanel();
            panel.Width.Set(50, 0);
            panel.Height.Set(50, 0);
            panel.VAlign = 0.5f;
            panel.Left.Set(size + 60, 1f);
            panel.SetPadding(0);
            panel.MaxHeight.Set(50, 0);
            panel.BackgroundColor = Colors.InventoryDefaultColorWithOpacity with { R = 35, G = 35, B = 90 };
            Append(panel);

            var display = new UITribute(tribute);
            display.VAlign = 0.5f;
            display.HAlign = 0.5f;
            
            panel.Append(display);
            size += panel.Width.Pixels;
        }
        Width.Set(size, 0f);
        Height.Set(36, 0f);
        Recalculate();
    }
}

