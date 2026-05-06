namespace MoonBark.PrototypeUI.Godot.UI;

using Godot;
using MoonBark.PrototypeUI.Core;

public partial class HotbarView : HBoxContainer
{
    private Hotbar? _hotbar;
    private readonly List<HotbarButtonView> _buttonViews = new();

    [Export] public PackedScene? HotbarButtonScene { get; set; }

    [Export] public int SlotCount { get; set; } = 10;

    public IHotbarActionSink? ActionSink { get; set; }

    public override void _Ready()
    {
        InitializeButtons();
    }

    public override void _Process(double delta)
    {
        _hotbar?.Update((float)delta);
        RefreshSlotStates();
    }

    public override void _Input(InputEvent @event)
    {
        if (_hotbar == null)
            return;

        // For demo purposes, create empty context.
        // In actual game this will be populated with proper target/source entities
        var context = new Framework.Effects.EffectContext();

        if (_hotbar.ProcessInput(@event, context))
        {
            AcceptEvent();
        }
    }

    /// <summary>
    /// Binds this view to a Hotbar instance
    /// </summary>
    public void BindHotbar(Hotbar hotbar)
    {
        _hotbar = hotbar;
        RefreshAllSlots();
    }

    private void InitializeButtons()
    {
        foreach (var child in GetChildren())
        {
            if (child is HotbarButtonView button)
            {
                _buttonViews.Add(button);
                button.ActionSink = ActionSink;
            }
        }

        // Create any missing buttons up to SlotCount
        while (_buttonViews.Count < SlotCount && HotbarButtonScene != null)
        {
            var button = HotbarButtonScene.Instantiate<HotbarButtonView>();
            button.ActionSink = ActionSink;
            AddChild(button);
            _buttonViews.Add(button);
        }
    }

    private void RefreshAllSlots()
    {
        if (_hotbar == null)
            return;

        for (var i = 0; i < Math.Min(_buttonViews.Count, _hotbar.SlotCount); i++)
        {
            RefreshSlot(i);
        }
    }

    private void RefreshSlotStates()
    {
        if (_hotbar == null)
            return;

        for (var i = 0; i < Math.Min(_buttonViews.Count, _hotbar.SlotCount); i++)
        {
            var slot = _hotbar.GetSlot(i);
            var button = _buttonViews[i];

            if (slot.CooldownInfo != null)
            {
                button.SetCooldownState(slot.CooldownInfo.IsActive, slot.CooldownInfo.Progress, slot.CooldownInfo.RemainingSeconds);
            }

            button.Disabled = !slot.CanActivate();
        }
    }

    private void RefreshSlot(int index)
    {
        if (_hotbar == null || index >= _buttonViews.Count)
            return;

        var slot = _hotbar.GetSlot(index);
        var button = _buttonViews[index];

        button.Configure(new HotbarSlotData(
            SlotIndex: slot.SlotIndex,
            Kind: HotbarSlotKind.Action,
            Id: slot.DisplayName,
            Label: slot.DisplayName,
            InputAction: $"{Hotbar.DefaultHotbarActionPrefix}{index + 1}",
            Binding: (index + 1).ToString(),
            IsAvailable: !slot.IsEmpty,
            Detail: slot.IsEmpty ? "Empty" : string.Empty,
            IconText: slot.DisplayName.Length > 0 ? slot.DisplayName[0].ToString().ToUpper() : "?",
            IconPath: string.Empty,
            IsOnCooldown: slot.HasCooldown,
            CooldownProgress: slot.CooldownInfo?.Progress ?? 1.0f,
            CooldownRemainingSeconds: slot.CooldownInfo?.RemainingSeconds ?? 0.0f
        ), false);
    }
}
