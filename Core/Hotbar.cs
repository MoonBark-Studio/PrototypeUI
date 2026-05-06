namespace MoonBark.PrototypeUI.Core;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;
using MoonBark.Framework.Effects;
using MoonBark.Framework.Slots;

/// <summary>
/// Hotbar container that manages multiple slots and input handling.
/// Follows the slots interface pattern and supports activation via input actions or mouse clicks.
/// </summary>
public sealed class Hotbar
{
    private readonly List<HotbarSlot> _slots;
    private readonly Dictionary<string, int> _inputActionBindings = new();

    /// <summary>
    /// Readonly view of hotbar slots
    /// </summary>
    public IReadOnlyList<HotbarSlot> Slots { get; }

    /// <summary>
    /// Number of slots in this hotbar
    /// </summary>
    public int SlotCount => _slots.Count;

    /// <summary>
    /// Default input action prefix for hotbar slots
    /// </summary>
    public const string DefaultHotbarActionPrefix = "hotbar_slot_";

    /// <summary>
    /// Event raised when any slot is activated
    /// </summary>
    public event EventHandler<SlotActivationEventArgs>? SlotActivated;

    /// <summary>
    /// Creates a new Hotbar with specified number of slots
    /// </summary>
    public Hotbar(int slotCount = 10)
    {
        _slots = new List<HotbarSlot>(slotCount);
        Slots = new ReadOnlyCollection<HotbarSlot>(_slots);

        for (var i = 0; i < slotCount; i++)
        {
            var slot = new HotbarSlot(i);
            slot.Activated += OnSlotActivated;
            _slots.Add(slot);
        }
    }

    /// <summary>
    /// Binds an input action to activate a specific slot index
    /// </summary>
    public void BindInputAction(string inputAction, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotCount)
            throw new ArgumentOutOfRangeException(nameof(slotIndex));

        _inputActionBindings[inputAction] = slotIndex;
    }

    /// <summary>
    /// Binds default input actions for all slots following standard naming convention
    /// </summary>
    public void BindDefaultInputActions(string prefix = DefaultHotbarActionPrefix)
    {
        for (var i = 0; i < SlotCount; i++)
        {
            BindInputAction($"{prefix}{i + 1}", i);
        }
    }

    /// <summary>
    /// Processes input event and activates slot if matching binding is found
    /// </summary>
    /// <returns>True if input was handled, false otherwise</returns>
    public bool ProcessInput(InputEvent inputEvent, EffectContext context)
    {
        foreach (var (actionName, slotIndex) in _inputActionBindings)
        {
            if (inputEvent.IsActionPressed(actionName))
            {
                return ActivateSlot(slotIndex, context);
            }
        }

        return false;
    }

    /// <summary>
    /// Activates slot at given index if possible
    /// </summary>
    public bool ActivateSlot(int slotIndex, EffectContext context)
    {
        if (slotIndex < 0 || slotIndex >= SlotCount)
            return false;

        var slot = _slots[slotIndex];
        if (!slot.CanActivate())
            return false;

        slot.Activate(context);
        return true;
    }

    /// <summary>
    /// Updates cooldown state for all slots
    /// </summary>
    public void Update(float deltaTime)
    {
        foreach (var slot in _slots)
        {
            slot.UpdateCooldown(deltaTime);
        }
    }

    /// <summary>
    /// Gets slot at specified index
    /// </summary>
    public HotbarSlot GetSlot(int index) => _slots[index];

    /// <summary>
    /// Clears all slots
    /// </summary>
    public void ClearAllSlots()
    {
        foreach (var slot in _slots)
        {
            slot.SetContent(null);
        }
    }

    private void OnSlotActivated(object? sender, SlotActivationEventArgs e)
    {
        SlotActivated?.Invoke(sender, e);
    }
}
