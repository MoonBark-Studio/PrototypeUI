using Chickensoft.GoDotTest;
using Godot;
using Shouldly;
using PrototypeUI.Godot.UI;

namespace PrototypeUI.Godot.Tests;

public partial class HudStatMeterTests : TestClass
{
    private static readonly Color HighValueColor = new("79d98a");
    private static readonly Color LowValueColor = new("e18074");
    private const string ScenePath = "res://addons/PrototypeUI/UI/HudStatMeter.tscn";

    public HudStatMeterTests(Node testScene) : base(testScene)
    {
    }

    [Test]
    public void HudStatMeter_Configure_ClampsValue_And_FormatsUppercaseLabel()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>(ScenePath);
        scene.ShouldNotBeNull($"Expected scene at {ScenePath}.");

        HudStatMeter meter = scene.Instantiate<HudStatMeter>();
        TestScene.AddChild(meter);
        meter.Configure("Hunger", 130);

        meter.GetNode<Label>("Name").Text.ShouldBe("HUNGER");
        meter.GetNode<ProgressBar>("Meter").Value.ShouldBe(100.0d);
        meter.GetNode<ProgressBar>("Meter").Modulate.ShouldBe(HighValueColor);
        meter.GetNode<Label>("Value").Text.ShouldBe("100");

        meter.Configure("Safety", -10);

        meter.GetNode<ProgressBar>("Meter").Value.ShouldBe(0.0d);
        meter.GetNode<ProgressBar>("Meter").Modulate.ShouldBe(LowValueColor);
        meter.GetNode<Label>("Value").Text.ShouldBe("0");

        meter.QueueFree();
    }
}
