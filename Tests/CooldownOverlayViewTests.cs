using Chickensoft.GoDotTest;
using Godot;
using Shouldly;
using System.Linq;
using PrototypeUI.Godot.UI;

namespace PrototypeUI.Godot.Tests;

public partial class CooldownOverlayViewTests : TestClass
{
    public CooldownOverlayViewTests(Node testScene) : base(testScene)
    {
    }

    [Test]
    public void CooldownOverlayView_SetCooldownState_TogglesOverlay_And_Label()
    {
        CooldownOverlayView overlayView = new();
        overlayView.Size = new Vector2(80.0f, 80.0f);
        TestScene.AddChild(overlayView);

        ColorRect overlay = overlayView.GetChildren().OfType<ColorRect>().Single();
        Label label = overlayView.GetChildren().OfType<Label>().Single();

        overlayView.SetCooldownState(true, 0.25f, 3.5f);

        overlay.Visible.ShouldBeTrue();
        label.Visible.ShouldBeTrue();
        label.Text.ShouldBe("3.5");
        overlay.OffsetTop.ShouldBe(60.0f);

        overlayView.SetCooldownState(false, 0.0f, 0.0f);

        overlay.Visible.ShouldBeFalse();
        label.Visible.ShouldBeFalse();
        overlayView.QueueFree();
    }
}