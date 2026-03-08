using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PrototypeUI.Godot.UI;

public partial class HudListView : VBoxContainer
{
    private VBoxContainer? _items;
    private readonly List<Label> _rows = new();

    public override void _Ready()
    {
        _items = GetNode<VBoxContainer>("Items");
    }

    public void SetItems(IEnumerable<string> items)
    {
        if (_items == null)
        {
            return;
        }

        List<string> entries = items.ToList();
        while (_rows.Count < entries.Count)
        {
            Label row = CreateRowLabel();
            _items.AddChild(row);
            _rows.Add(row);
        }

        for (int index = 0; index < _rows.Count; index++)
        {
            bool isVisible = index < entries.Count;
            _rows[index].Visible = isVisible;
            if (isVisible)
            {
                _rows[index].Text = entries[index];
            }
        }
    }

    private static Label CreateRowLabel()
    {
        Label row = new()
        {
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            ThemeTypeVariation = "HudListRowLabel"
        };
        return row;
    }
}
