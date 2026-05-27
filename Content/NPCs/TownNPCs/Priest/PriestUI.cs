using CalamityVanilla.Common.Blessings;
using CalamityVanilla.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.WorldBuilding;

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
    private SlowerUIList list;
    private UIScrollbar scrollbar;
    private ActivateButton activate;
    private ActivateButton deactivate;
    public override void OnInitialize()
    {
        panel = new UIPanel();
        panel.BackgroundColor = Colors.InventoryDefaultColorWithOpacity;
        panel.Width.Set(500, 0);
        panel.Height.Set(250, 0);
        panel.HAlign = 0.5001f;
        panel.Top.Set(30, 0.5f);
        Append(panel);

        list = new SlowerUIList(0.5f)
        {
            Width = StyleDimension.Fill
        };
        list.Height.Set(10, 0.82f);
        list.Top.Set(-17, 0.25f);
        panel.Append(list);

        scrollbar = new UIScrollbar
        {
            HAlign = 1.015f,
            VAlign = 0.5f
        };
        scrollbar.Height.Set(0, 1);
        scrollbar._dragYOffset = 2;
        panel.Append(scrollbar);

        list.SetScrollbar(scrollbar);

        UIText cost = new UIText("Cost");
        cost.Left.Set(-35, 0.95f);
        cost.VAlign = 0.02f;
        panel.Append(cost);

        activate = new ActivateButton("Add Blessings", LeftClick);
        activate.OnLeftClick += LeftClick;
        activate.pressable = true;
        activate.HAlign = 0.025f;
        activate.Top.Set(-4, 0);
        activate.Width.Set(150, 0);
        activate.Height.Set(30, 0);
        panel.Append(activate);

        deactivate = new ActivateButton("Remove Blessings", LeftClick);
        deactivate.OnLeftClick += LeftClick;
        deactivate.HAlign = 0.55f;
        deactivate.Top.Set(-4, 0);
        deactivate.Width.Set(170, 0);
        deactivate.Height.Set(30, 0);
        panel.Append(deactivate);

        partSprite = Main.Assets.Request<Texture2D>("Images/UI/Creative/Research_Spark", AssetRequestMode.ImmediateLoad);

        activateParticle = new UIParticleLayer
        {
            Width = StyleDimension.Fill,
            Height = StyleDimension.Fill
        };
        panel.Append(activateParticle);
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

        UIElement blank = new UIElement();
        blank.Width.Set(40, 0);
        blank.Height.Set(400, 0);
        blank.HAlign = 0.1f;
        list.Add(blank);
    }

    public static List<BlessingButton> buttonList = [];
    public UIParticleLayer activateParticle;
    public Asset<Texture2D> partSprite;

    private void LeftClick(UIMouseEvent evt, UIElement listeningElement)
    {
        //activate effect
        if (listeningElement.Equals(activate) && activate.pressable)
        {
            foreach (var _button in buttonList)
            {
                if (!_button.activated && _button.toggle && _button.blessing.Enable(Main.LocalPlayer))
                {
                    _button.activated = true;
                    _button.toggle = false;
                    _button.timer = 20;

                    for (int i = 0; i < 30; i++)
                    {

                        Vector2 center = _button.GetDimensions().Center() - panel.GetDimensions().Position();
                        float width = _button.GetDimensions().Width;
                        float height = _button.GetDimensions().Height;
                        Vector2 widthHeight = center + new Vector2(Main.rand.NextFloat(-width, width) / 3 - 80, Main.rand.NextFloat(-height, height) / 2 - 10);
                        Vector2 initialVelocity = new Vector2(0, Main.rand.NextFloat(-0.3f, -0.05f) * 5);

                        //initialVelocity *= 0.5f;

                        Vector2 accelerationPerFrame = new Vector2(0f, 0.01f);
                        activateParticle.AddParticle(new CreativeSacrificeParticle(partSprite, null, initialVelocity, widthHeight)
                        {
                            AccelerationPerFrame = accelerationPerFrame,
                            _scale = Main.rand.NextFloat(0.1f, 0.6f),
                            ScaleOffsetPerFrame = -1f / 60f,
                        });
                    }
                }
            }
        }
        //deactivate effect
        if (listeningElement.Equals(deactivate) && deactivate.pressable)
        {
            foreach (var _button in buttonList)
            {
                if (_button.activated && _button.toggle)
                {
                    _button.blessing.Disable(Main.LocalPlayer);
                    _button.activated = false;
                    _button.toggle = false;
                    _button.timer = 20;
                }
            }
        }
    }
    public static void AddBlessing(PriestBlessing blessing)
    {
        BlessingButton button = new BlessingButton(blessing);
        button.activated = blessing.GetState(Main.LocalPlayer);
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
        activate.pressable = false;
        deactivate.pressable = false;
        foreach (var _button in buttonList)
        {
            if (_button.toggle && _button.blessing.CheckEnable(Main.LocalPlayer))
            {
                if (!_button.activated)
                {
                    activate.pressable = true;
                }
                if (_button.activated)
                {
                    deactivate.pressable = true;
                }
            }
        }
        base.Update(gameTime);
    }

}

public class ActivateButton : UIElement
{
    private object _text;
    private UIElement.MouseEvent _clickAction;
    private UIPanel _uiPanel;
    private UIText _uiText;
    public bool pressable;
    public string Text
    {
        get => _uiText?.Text ?? string.Empty;
        set => _text = value;
    }

    public SoundStyle? HoverSound = SoundID.MenuTick;
    public SoundStyle? ClickSound = null;


    public ActivateButton(object text, UIElement.MouseEvent clickAction) : base()
    {
        _text = text?.ToString() ?? string.Empty;
        _clickAction = clickAction;

        if ((string)_text == "Add Blessings")
        {
            ClickSound = SoundID.ResearchComplete;
        }
        else if ((string)_text == "Remove Blessings")
        {
            ClickSound = SoundID.Grab;
        }
    }

    public override void MouseOver(UIMouseEvent evt)
    {
        base.MouseOver(evt);

        if (HoverSound != null && pressable)
            SoundEngine.PlaySound(HoverSound.Value);
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);

        if (pressable)
        {
            if (ClickSound == SoundID.ResearchComplete)
            {
                SoundEngine.PlaySound(SoundID.Item29 with { Pitch = 0.1f });
                SoundEngine.PlaySound(SoundID.Research);
            }
            else if (ClickSound == SoundID.Grab)
            {
                SoundEngine.PlaySound(SoundID.Grab);
            }
        }
    }

    public override void OnInitialize()
    {
        _uiPanel = new UIPanel();
        _uiPanel.Width = StyleDimension.Fill;
        _uiPanel.Height = StyleDimension.Fill;
        Append(_uiPanel);

        _uiText = new UIText("");
        _uiText.VAlign = _uiText.HAlign = 0.5f;
        _uiPanel.Append(_uiText);

        _uiPanel.OnLeftClick += _clickAction;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime); // Propagate update to child elements.
        if (_text != null)
        {
            _uiText.SetText(_text.ToString());
            _text = null;
            Recalculate();
            base.MinWidth = _uiText.MinWidth;
            base.MinHeight = _uiText.MinHeight;
        }
        if (pressable)
        {
            _uiText._color = Color.White;
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
        else
        {
            _uiPanel.BackgroundColor = new Color(65, 65, 128);
            _uiPanel.BorderColor = Color.Black;
            _uiText._color = Color.Gray;
        }
    }
}
public class BlessingButton : UIElement
{
    private object _text;
    private object _desc;
    private object _stat;
    private UIPanel _uiPanel;
    private UIText _uiText;
    public bool toggle;
    public bool activated;
    public int timer;

    public string Text
    {
        get => _uiText?.Text ?? string.Empty;
        set => _text = _desc = _stat = value;
    }

    private UIElement image;
    private UIElement arrow;
    public PriestBlessing blessing;

    public SoundStyle? HoverSound = SoundID.MenuTick;
    public SoundStyle? ClickSound = SoundID.MenuTick;

    public BlessingButton(PriestBlessing blessingObj) : base()
    {
        base.Width.Set(0, 1f);
        base.Height.Set(50, 0);
        _text = blessingObj.DisplayName.Value?.ToString() ?? string.Empty;
        _desc = blessingObj.Description.Value?.ToString() ?? string.Empty;
        _stat = blessingObj.Stats.Value?.ToString() ?? string.Empty;
        blessing = blessingObj;

        _uiPanel = new UIPanel
        {
            Height = StyleDimension.Fill
        };
        _uiPanel.Width.Set(-130, 1f);
        _uiPanel.SetPadding(0);
        Append(_uiPanel);

        _uiText = new UIText("");
        _uiText.VAlign = 0.5f;
        _uiText.Left.Set(25, 0.08f);
        _uiPanel.Append(_uiText);

        image = new UIElement();
        image.Width.Set(34, 0f);
        image.Height.Set(34, 0f);
        image.HAlign = 0.03f;
        image.VAlign = 1.5f;
        image.OnDraw += DrawIcon;
        _uiPanel.Append(image);

        UITribute _tribute = new UITribute(blessingObj.Tribute)
        {
            VAlign = 0.5f
        };
        _tribute.Left.Set(-40, 0.95f);
        Append(_tribute);

        arrow = new UIElement();
        arrow.Width.Set(46, 0f);
        arrow.Height.Set(30, 0f);
        arrow.HAlign = 0.82f;
        arrow.VAlign = 0.5f;
        arrow.OnDraw += DrawArrow;
        Append(arrow);
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        if (_uiPanel.IsMouseHovering)
        {
            UICommon.TooltipMouseText(_desc.ToString() + "\n" + _stat.ToString());
        }
    }
    private void DrawIcon(UIElement affectedElement)
    {
        Texture2D tex = blessing.Icon.Value;
        Rectangle frame = tex.Frame(1, 2, 0, activated ? 0 : 1);
        Main.spriteBatch.Draw(tex, affectedElement.GetDimensions().Center(), frame, Color.White, 0, tex.Size() / 2f, 1f, SpriteEffects.None, 0);
    }
    private void DrawArrow(UIElement affectedElement)
    {
        Texture2D tex = (Texture2D)ModContent.Request<Texture2D>($"CalamityVanilla/Common/Blessings/TributeArrow");
        Main.spriteBatch.Draw(tex, affectedElement.GetDimensions().Center(), null, Color.White, 0, tex.Size() / 2f, 1f, SpriteEffects.None, 0);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime); // Propagate update to child elements.
        if (_text != null)
        {
            _uiText.SetText(_text.ToString());
            _text = null;
        }

        if (timer > 0)
        {
            timer--;
        }

        if (!activated)
        {
            Color BaseColor = Color.Lerp(Colors.InventoryDefaultColorWithOpacity, Color.IndianRed, timer / 10f);
            Color ToggledBaseColor = Color.Lerp(new Color(107, 108, 196), Color.IndianRed, timer / 10f);

            if (_uiPanel.IsMouseHovering)
            {
                _uiPanel.BackgroundColor = toggle ? ToggledBaseColor * 1.15f : BaseColor * 1.6f;
                _uiPanel.BorderColor = Main.OurFavoriteColor;
            }
            else
            {
                _uiPanel.BackgroundColor = toggle ? ToggledBaseColor : BaseColor;
                _uiPanel.BorderColor = toggle ? new Color(156, 178, 245) : Color.Black;
            }
        }
        else
        {
            Color BaseColor = Color.Lerp(new Color(51, 125, 76, 255) * 0.785f, Main.OurFavoriteColor, timer / 10f);
            Color ToggledBaseColor = Color.Lerp(new Color(86, 166, 116), Main.OurFavoriteColor, timer / 10f);
            if (_uiPanel.IsMouseHovering)
            {
                _uiPanel.BackgroundColor = toggle ? ToggledBaseColor * 1.05f : BaseColor * 1.6f;
                _uiPanel.BorderColor = Main.OurFavoriteColor;
            }
            else
            {
                _uiPanel.BackgroundColor = toggle ? ToggledBaseColor : BaseColor;
                _uiPanel.BorderColor = toggle ? Color.LightGreen : Color.Black;
            }
        }
    }

    public override void MouseOver(UIMouseEvent evt)
    {
        base.MouseOver(evt);

        if (HoverSound != null && _uiPanel.IsMouseHovering)
            SoundEngine.PlaySound(HoverSound.Value);
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);
        if (_uiPanel.IsMouseHovering)
        { 
            toggle = !toggle; 
        }

        if (ClickSound != null && _uiPanel.IsMouseHovering)
            SoundEngine.PlaySound(ClickSound.Value);
    }
}

public class UITribute : UIElement
{
    private UIText text;
    private UIItemIcon item;
    private UIPanel panel;
    private Tribute _tribute;
    public UITribute(Tribute tribute)
    {
        Width.Set(44, 0);
        Height.Set(44, 0);
        this._tribute = tribute;

        panel = new UIPanel();
        panel.Width.Set(0f, 1f);
        panel.Height.Set(0f, 1f);
        Append(panel);

        item = new UIItemIcon(ContentSamples.ItemsByType[tribute.ItemType], false);
        item.HAlign = 1f;
        item.VAlign = 0.5f;
        panel.Append(item);

        text = new UIText(LocalizedText.Empty, 0.8f);
        text.SetText(tribute.Stack.ToString());
        text.SetPadding(0);
        text.HAlign = -0.5f;
        text.VAlign = 1.7f;
        text.Width.Set(10f, 0);
        text.Height.Set(10f, 0);
        text.IgnoresMouseInteraction = true;
        panel.Append(text);

    }
    public Color fulfilledColor = new Color(46, 50, 127);
    public Color unfullfilledColor = new Color(106, 36, 36);
    public override void Update(GameTime gameTime)
    {
        panel.BackgroundColor = Main.LocalPlayer.CountItem(_tribute.ItemType, _tribute.Stack) >= _tribute.Stack ? fulfilledColor : unfullfilledColor;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        if (GetDimensions().ToRectangle().Contains(Main.MouseScreen.ToPoint()))
        {
            Main.HoverItem = item._item;
            Main.hoverItemName = item._item.Name;
            Main.instance.MouseText("", 0, 0);
            Main.mouseText = true;
        }
    }
}

//public sealed class UITributeList : UIElement
//{
//    public Color fulfilledColor = new Color(46, 50, 127);
//    public Color unfullfilledColor = new Color(46, 50, 127);
//    public UITributeList(Tribute[] tributes)
//    {
//        float size = 0;
//        foreach (var tribute in tributes)
//        {
//            UIPanel panel = new UIPanel();
//            panel.Width.Set(50, 0);
//            panel.Height.Set(50, 0);
//            panel.VAlign = 0.5f;
//            panel.Left.Set(size + 60, 1f);
//            panel.SetPadding(0);
//            panel.MaxHeight.Set(50, 0);
//            panel.BackgroundColor = fulfilledColor;
//            panel.OnUpdate += Panel_OnUpdate;
//            Append(panel);

//            var display = new UITribute(tribute);
//            display.VAlign = 0.5f;
//            display.HAlign = 0.5f;

//            panel.Append(display);
//            size += panel.Width.Pixels;
//        }
//        Width.Set(size, 0f);
//        Height.Set(36, 0f);
//        Recalculate();
//    }

//    private void Panel_OnUpdate(UIElement affectedElement)
//    {

//    }
//}

public sealed class SlowerUIList(float speedMod) : UIList
{
    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        if (_scrollbar != null)
        {
            _scrollbar.ViewPosition -= evt.ScrollWheelValue * speedMod;
        }
    }
}
