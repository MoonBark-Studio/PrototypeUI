using Godot;

namespace PrototypeUI.Godot.UI;

public partial class CooldownOverlayView : Control
{
    private static class CooldownOverlayConstants
    {
        public static readonly Color OverlayColor = new(0, 0, 0, 0.7f);
        public const int OverlayZIndex = 10;
        public const int LabelZIndex = 11;
        public const int LabelOffsetLeft = -20;
        public const int LabelOffsetTop = -10;
        public const int LabelOffsetRight = 20;
        public const int LabelOffsetBottom = 10;
    }

    private ColorRect? _cooldownOverlay;
    private Label? _cooldownLabel;

    public bool IsOnCooldown { get; private set; }

    public float CooldownProgress { get; private set; }

    public float RemainingCooldownSeconds { get; private set; }

    public override void _Ready()
    {
        _cooldownOverlay = new ColorRect
        {
            Color = CooldownOverlayConstants.OverlayColor,
            AnchorsPreset = (int)LayoutPreset.FullRect,
            OffsetLeft = 0,
            OffsetTop = 0,
            OffsetRight = 0,
            OffsetBottom = 0,
            ZIndex = CooldownOverlayConstants.OverlayZIndex
        };
        AddChild(_cooldownOverlay);

        _cooldownLabel = new Label
        {
            Text = string.Empty,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            AnchorsPreset = (int)LayoutPreset.Center,
            OffsetLeft = CooldownOverlayConstants.LabelOffsetLeft,
            OffsetTop = CooldownOverlayConstants.LabelOffsetTop,
            OffsetRight = CooldownOverlayConstants.LabelOffsetRight,
            OffsetBottom = CooldownOverlayConstants.LabelOffsetBottom,
            ZIndex = CooldownOverlayConstants.LabelZIndex,
            Modulate = Colors.White
        };
        AddChild(_cooldownLabel);

        SetCooldownState(false, 0.0f, 0.0f);
    }

    public void SetCooldownState(bool isOnCooldown, float progress, float remainingSeconds)
    {
        IsOnCooldown = isOnCooldown;
        CooldownProgress = progress;
        RemainingCooldownSeconds = remainingSeconds;
        UpdateVisuals();
    }

    public override void _Process(double delta)
    {
        if (IsOnCooldown)
        {
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (_cooldownOverlay == null || _cooldownLabel == null)
        {
            return;
        }

        if (!IsOnCooldown)
        {
            _cooldownOverlay.Visible = false;
            _cooldownLabel.Visible = false;
            return;
        }

        _cooldownOverlay.Visible = true;
        _cooldownLabel.Visible = true;

        float fillHeight = CooldownProgress * Size.Y;
        _cooldownOverlay.OffsetTop = Size.Y - fillHeight;
        _cooldownLabel.Text = RemainingCooldownSeconds > 0.0f
            ? RemainingCooldownSeconds.ToString("F1")
            : string.Empty;
    }
}
