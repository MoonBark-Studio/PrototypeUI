using Chickensoft.GoDotTest;
using Godot;
using Shouldly;
using System.Linq;
using PrototypeUI.Godot.UI;

namespace PrototypeUI.Godot.Tests;

public partial class HudListViewTests : TestClass
{
    private const string ScenePath = "res://addons/PrototypeUI/UI/HudListView.tscn";

    public HudListViewTests(Node testScene) : base(testScene)
    {
    }

    [Test]
    public void HudListView_SetItems_CreatesRows_And_ReusesExistingLabels()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>(ScenePath);
        scene.ShouldNotBeNull($"Expected scene at {ScenePath}.");

        HudListView listView = scene.Instantiate<HudListView>();
        TestScene.AddChild(listView);

        listView.SetItems(new[] { "Wood x2", "Herb x1", "Fiber x4" });

        VBoxContainer items = listView.GetNode<VBoxContainer>("Items");
        Label[] rows = items.GetChildren().OfType<Label>().ToArray();
        rows.Length.ShouldBe(3);
        rows[0].Text.ShouldBe("Wood x2");
        rows[1].Text.ShouldBe("Herb x1");
        rows[2].Text.ShouldBe("Fiber x4");

        listView.SetItems(new[] { "Stone x3" });

        rows = items.GetChildren().OfType<Label>().ToArray();
        rows.Length.ShouldBe(3, "Rows should be reused instead of recreated when the list shrinks.");
        rows[0].Visible.ShouldBeTrue();
        rows[0].Text.ShouldBe("Stone x3");
        rows[1].Visible.ShouldBeFalse();
        rows[2].Visible.ShouldBeFalse();

        listView.QueueFree();
    }
}