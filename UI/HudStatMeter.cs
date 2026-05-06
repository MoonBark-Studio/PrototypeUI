using Godot;
using System;

namespace MoonBark.PrototypeUI.Godot.UI;

public partial class HudStatMeter : HBoxContainer
{
    private static readonly Color HighValueColor = new("79d98a");
    private static readonly Color MediumValueColor = new("e4c267");
    private static readonly Color LowValueColor = new("e18074");

    private Label? _nameLabel;
    private ProgressBar? _meterBar;
    private Label? _valueLabel;

    public override void _Ready()
    {
        _nameLabel = GetNode<Label>("Name");
        _meterBar = GetNode<ProgressBar>("Meter");
        _valueLabel = GetNode<Label>("Value");
    }

    public void Configure(string label, int value)
    {
        if (_nameLabel == null || _meterBar == null || _valueLabel == null)
        {
            return;
        }

        int clamped = Math.Clamp(value, 0, 100);
        _nameLabel.Text = label.ToUpperInvariant();
        _meterBar.Value = clamped;
        _meterBar.Modulate = ResolveMeterColor(clamped);
        _valueLabel.Text = clamped.ToString();
    }

    private static Color ResolveMeterColor(int value)
    {
        if (value >= 75)
        {
            return HighValueColor;
        }

        if (value >= 45)
        {
            return MediumValueColor;
        }

        return LowValueColor;
    }
}
