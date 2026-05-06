using Chickensoft.GoDotTest;
using Godot;
using MoonBark.PrototypeUI.Core;
using Shouldly;
using System.Linq;
using MoonBark.PrototypeUI.Godot.UI;

namespace MoonBark.PrototypeUI.Godot.Tests;

public partial class HotbarButtonViewTests : TestClass
{
    private static readonly Color SelectedSlotColor = new("fff2bf");
    private const string ScenePath = "res://addons/PrototypeUI/UI/HotbarButtonView.tscn";

    public HotbarButtonViewTests(Node testScene) : base(testScene)
    {
    }

    [Test]
    public void HotbarButtonView_Configure_MapsSlotState_IntoVisualState()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>(ScenePath);
        scene.ShouldNotBeNull($"Expected scene at {ScenePath}.");

        HotbarButtonView button = scene.Instantiate<HotbarButtonView>();
        TestScene.AddChild(button);

        HotbarSlotData slot = new(
            4,
            HotbarSlotKind.Ability,
            "heal",
            "Heal",
            string.Empty,
            "5",
            false,
            "Need safety < 65",
            "!",
            IsOnCooldown: true,
            CooldownProgress: 0.25f,
            CooldownRemainingSeconds: 3.5f);

        button.Configure(slot, true);

        button.SlotIndex.ShouldBe(4);
        button.ActionId.ShouldBe("heal");
        button.Disabled.ShouldBeTrue();
        button.GetNode<Label>("Binding").Text.ShouldBe("[5]");
        button.GetNode<Label>("IconLabel").Visible.ShouldBeTrue();
        button.GetNode<Label>("IconLabel").Text.ShouldBe("!");
        button.GetNode<TextureRect>("Icon").Visible.ShouldBeFalse();
        button.GetNode<Label>("Title").Text.ShouldBe("Heal");
        button.GetNode<Label>("Detail").Text.ShouldBe("Locked");
        button.Modulate.ShouldBe(SelectedSlotColor);

        CooldownOverlayView cooldownView = button.GetChildren().OfType<CooldownOverlayView>().Single();
        cooldownView.IsOnCooldown.ShouldBeTrue();
        cooldownView.CooldownProgress.ShouldBe(0.25f);
        cooldownView.RemainingCooldownSeconds.ShouldBe(3.5f);

        button.QueueFree();
    }

    [Test]
    public void HotbarButtonView_Press_InvokesConfiguredActionSink()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>(ScenePath);
        scene.ShouldNotBeNull($"Expected scene at {ScenePath}.");

        HotbarButtonView button = scene.Instantiate<HotbarButtonView>();
        TestScene.AddChild(button);

        RecordingSink sink = new();
        button.ActionSink = sink;
        button.Configure(new HotbarSlotData(2, HotbarSlotKind.Item, "berries", "Wild Berries", string.Empty, "3", true, "x2", "b"), false);

        button.EmitSignal(BaseButton.SignalName.Pressed);

        sink.LastActivatedSlot.ShouldBe(2);
        button.QueueFree();
    }

    private sealed class RecordingSink : IHotbarActionSink
    {
        public int? LastActivatedSlot { get; private set; }

        public bool TryActivateHotbarSlot(int slotIndex)
        {
            LastActivatedSlot = slotIndex;
            return true;
        }
    }
}
