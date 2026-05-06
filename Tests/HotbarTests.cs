namespace MoonBark.PrototypeUI.Tests;

using Xunit;
using MoonBark.PrototypeUI.Core;
using MoonBark.Framework.Slots;
using MoonBark.Framework.Effects;

public class HotbarTests
{
    [Fact]
    public void Hotbar_CreatesCorrectNumberOfSlots()
    {
        var hotbar = new Hotbar(10);

        Assert.Equal(10, hotbar.SlotCount);
        Assert.Equal(10, hotbar.Slots.Count);

        for (var i = 0; i < hotbar.SlotCount; i++)
        {
            Assert.Equal(i, hotbar.GetSlot(i).SlotIndex);
            Assert.True(hotbar.GetSlot(i).IsEmpty);
        }
    }

    [Fact]
    public void HotbarSlot_ImplementsSlotsInterface()
    {
        var slot = new HotbarSlot(0);

        Assert.IsAssignableFrom<ISlot<ActionContent?>>(slot);
        Assert.IsAssignableFrom<IMutableSlot<ActionContent?>>(slot);
        Assert.IsAssignableFrom<IActionSlot>(slot);
    }

    [Fact]
    public void HotbarSlot_SetContent_WorksCorrectly()
    {
        var slot = new HotbarSlot(0);
        var content = ActionContent.FromEffect(new MockEffectDefinition { Name = "Fireball" });

        var eventRaised = false;
        slot.ContentChanged += (_, args) =>
        {
            eventRaised = true;
            Assert.Equal(0, args.SlotIndex);
            Assert.Null(args.PreviousContent);
            Assert.Equal(content, args.NewContent);
        };

        slot.SetContent(content);

        Assert.Equal(content, slot.Content);
        Assert.False(slot.IsEmpty);
        Assert.Equal("Fireball", slot.DisplayName);
        Assert.True(eventRaised);
    }

    [Fact]
    public void HotbarSlot_CanActivate_ReturnsCorrectState()
    {
        var slot = new HotbarSlot(0);

        // Empty slot cannot activate
        Assert.False(slot.CanActivate());

        // Slot with content can activate
        var content = ActionContent.FromEffect(new MockEffectDefinition { Name = "Fireball" });
        slot.SetContent(content);
        Assert.True(slot.CanActivate());

        // Slot on cooldown cannot activate
        slot.StartCooldown(5);
        Assert.False(slot.CanActivate());

        // After cooldown expires can activate again
        slot.UpdateCooldown(6);
        Assert.True(slot.CanActivate());
    }

    [Fact]
    public void HotbarSlot_SwapWith_WorksCorrectly()
    {
        var slot1 = new HotbarSlot(0);
        var slot2 = new HotbarSlot(1);

        var content1 = ActionContent.FromEffect(new MockEffectDefinition { Name = "Fireball" });
        var content2 = ActionContent.FromEffect(new MockEffectDefinition { Name = "Icebolt" });

        slot1.SetContent(content1);
        slot2.SetContent(content2);

        slot1.SwapWith(slot2);

        Assert.Equal(content2, slot1.Content);
        Assert.Equal(content1, slot2.Content);
    }

    [Fact]
    public void Hotbar_InputBinding_Works()
    {
        var hotbar = new Hotbar(10);
        hotbar.BindDefaultInputActions();

        var content = ActionContent.FromEffect(new MockEffectDefinition { Name = "Fireball" });
        hotbar.GetSlot(0).SetContent(content);

        var activated = false;
        hotbar.SlotActivated += (_, _) => activated = true;

        // Simulate input press
        var inputEvent = new MockInputEvent { ActionName = "hotbar_slot_1", Pressed = true };
        var context = new EffectContext();

        var handled = hotbar.ProcessInput(inputEvent, context);

        Assert.True(handled);
        Assert.True(activated);
    }

    [Fact]
    public void Hotbar_CooldownSystem_WorksCorrectly()
    {
        var hotbar = new Hotbar(1);
        var slot = hotbar.GetSlot(0);

        var content = ActionContent.FromEffect(new MockEffectDefinition { Name = "Fireball" });
        slot.SetContent(content);
        slot.StartCooldown(3.0f);

        Assert.True(slot.HasCooldown);
        Assert.False(slot.CanActivate());

        // Update with 1 second delta
        hotbar.Update(1.0f);
        Assert.Equal(1.0f, slot.CooldownInfo!.ElapsedSeconds, 3);
        Assert.Equal(2.0f, slot.CooldownInfo.RemainingSeconds, 3);
        Assert.Equal(1.0f / 3.0f, slot.CooldownInfo.Progress, 3);

        // Update until complete
        hotbar.Update(3.0f);
        Assert.False(slot.HasCooldown);
        Assert.True(slot.CanActivate());
    }

    [Fact]
    public void HotbarSlot_Activation_RaisesEvent()
    {
        var slot = new HotbarSlot(5);
        var content = ActionContent.FromEffect(new MockEffectDefinition { Name = "Test Ability" });
        slot.SetContent(content);

        SlotActivationEventArgs? receivedArgs = null;
        slot.Activated += (_, args) => receivedArgs = args;

        var context = new EffectContext();
        slot.Activate(context);

        Assert.NotNull(receivedArgs);
        Assert.Equal(5, receivedArgs.Value.SlotIndex);
        Assert.Equal("Test Ability", receivedArgs.Value.DisplayName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(9)]
    public void Hotbar_ActivatesCorrectSlotByIndex(int slotIndex)
    {
        var hotbar = new Hotbar(10);
        var content = ActionContent.FromEffect(new MockEffectDefinition { Name = $"Slot {slotIndex}" });
        hotbar.GetSlot(slotIndex).SetContent(content);

        var activatedIndex = -1;
        hotbar.SlotActivated += (sender, args) =>
        {
            activatedIndex = args.SlotIndex;
        };

        var context = new EffectContext();
        hotbar.ActivateSlot(slotIndex, context);

        Assert.Equal(slotIndex, activatedIndex);
    }

    #region Test Helpers

    private class MockEffectDefinition : IEffectDefinition
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }

    private class MockInputEvent : InputEvent
    {
        public string ActionName { get; init; } = string.Empty;
        public bool Pressed { get; init; }

        public override bool IsActionPressed(string action) => action == ActionName && Pressed;
    }

    #endregion
}
