using Chickensoft.GoDotTest;
using Godot;
using Shouldly;
using MoonBark.PrototypeUI.Godot.UI;

namespace MoonBark.PrototypeUI.Godot.Tests;

public partial class HudSectionCardTests : TestClass
{
    private const string ScenePath = "res://addons/PrototypeUI/UI/HudSectionCard.tscn";

    public HudSectionCardTests(Node testScene) : base(testScene)
    {
    }

    [Test]
    public void HudSectionCard_Configure_UpdatesHeaderLabels_And_ExposesContentRoot()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>(ScenePath);
        scene.ShouldNotBeNull($"Expected scene at {ScenePath}.");

        HudSectionCard card = scene.Instantiate<HudSectionCard>();
        TestScene.AddChild(card);
        card.Configure("Status", "Player vitals");

        card.ContentRoot.ShouldNotBeNull();
        card.GetNode<Label>("Margin/Frame/Header/Title").Text.ShouldBe("Status");
        card.GetNode<Label>("Margin/Frame/Header/Subtitle").Text.ShouldBe("Player vitals");

        card.QueueFree();
    }
}
