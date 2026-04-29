using Godot;
using MoonBark.PrototypeUI.Core;
using System.Linq;

namespace MoonBark.PrototypeUI.Godot.UI;

public partial class HotbarButtonView : Button
{
    private const int InvalidSlotIndex = -1;
    private static readonly Color SelectedSlotColor = new("fff2bf");

    private Label? _bindingLabel;
    private TextureRect? _iconTexture;
    private Label? _iconLabel;
    private Label? _titleLabel;
    private Label? _detailLabel;
    private CooldownOverlayView? _cooldownView;

    public IHotbarActionSink? ActionSink { get; set; }

    public int SlotIndex { get; private set; } = InvalidSlotIndex;

    public string? ActionId { get; private set; }

    public override void _Ready()
    {
        _bindingLabel = GetNode<Label>("Binding");
        _iconTexture = GetNode<TextureRect>("Icon");
        _iconLabel = GetNode<Label>("IconLabel");
        _titleLabel = GetNode<Label>("Title");
        _detailLabel = GetNode<Label>("Detail");

        FocusMode = FocusModeEnum.None;
        Pressed += OnPressed;

        _cooldownView = new CooldownOverlayView
        {
            AnchorsPreset = (int)LayoutPreset.FullRect,
            OffsetLeft = 0,
            OffsetTop = 0,
            OffsetRight = 0,
            OffsetBottom = 0
        };
        AddChild(_cooldownView);
    }

    public override void _ExitTree()
    {
        Pressed -= OnPressed;
    }

    public void Configure(HotbarSlotData slot, bool isSelected)
    {
        if (_bindingLabel == null || _iconTexture == null || _iconLabel == null || _titleLabel == null || _detailLabel == null)
        {
            return;
        }

        SlotIndex = slot.SlotIndex;
        ActionId = string.IsNullOrWhiteSpace(slot.Id) ? null : slot.Id;
        Disabled = !slot.IsAvailable;
        _bindingLabel.Text = ResolveBindingText(slot);
        ApplyIcon(slot);
        _titleLabel.Text = slot.Label;
        _detailLabel.Text = slot.IsAvailable ? slot.Detail : "Locked";
        Modulate = isSelected ? SelectedSlotColor : Colors.White;
        _cooldownView?.SetCooldownState(slot.IsOnCooldown, slot.CooldownProgress, slot.CooldownRemainingSeconds);
    }

    private void ApplyIcon(HotbarSlotData slot)
    {
        if (_iconTexture == null || _iconLabel == null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(slot.IconPath))
        {
            _iconTexture.Texture = ResourceLoader.Load<Texture2D>(slot.IconPath);
            _iconTexture.Visible = _iconTexture.Texture != null;
            _iconLabel.Visible = !_iconTexture.Visible;
            _iconLabel.Text = _iconTexture.Visible ? string.Empty : slot.IconText;
            return;
        }

        _iconTexture.Texture = null;
        _iconTexture.Visible = false;
        _iconLabel.Visible = true;
        _iconLabel.Text = slot.IconText;
    }

    private void OnPressed()
    {
        if (SlotIndex == InvalidSlotIndex)
        {
            return;
        }

        ActionSink?.TryActivateHotbarSlot(SlotIndex);
    }

    private static string ResolveBindingText(HotbarSlotData slot)
    {
        if (string.IsNullOrWhiteSpace(slot.InputAction) || !InputMap.HasAction(slot.InputAction))
        {
            return $"[{slot.Binding}]";
        }

        InputEvent? inputEvent = InputMap.ActionGetEvents(slot.InputAction).FirstOrDefault();
        if (inputEvent is InputEventKey keyEvent)
        {
            return $"[{OS.GetKeycodeString(keyEvent.Keycode)}]";
        }

        return $"[{slot.Binding}]";
    }

    /// <summary>
    /// Updates cooldown overlay state for this button
    /// </summary>
    public void SetCooldownState(bool isActive, float progress, float remainingSeconds)
    {
        _cooldownView?.SetCooldownState(isActive, progress, remainingSeconds);
    }
}
