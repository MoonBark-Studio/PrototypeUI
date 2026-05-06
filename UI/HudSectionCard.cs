using Godot;

namespace MoonBark.PrototypeUI.Godot.UI;

public partial class HudSectionCard : PanelContainer
{
    private Label? _titleLabel;
    private Label? _subtitleLabel;
    private Control? _contentRoot;

    public Control ContentRoot => _contentRoot!;

    public override void _Ready()
    {
        _titleLabel = GetNode<Label>("Margin/Frame/Header/Title");
        _subtitleLabel = GetNode<Label>("Margin/Frame/Header/Subtitle");
        _contentRoot = GetNode<Control>("Margin/Frame/Content");
    }

    public void Configure(string title, string subtitle)
    {
        if (_titleLabel == null || _subtitleLabel == null)
        {
            return;
        }

        _titleLabel.Text = title;
        _subtitleLabel.Text = subtitle;
    }
}
