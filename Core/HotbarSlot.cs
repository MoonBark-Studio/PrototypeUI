namespace MoonBark.PrototypeUI.Core;

using System;
using MoonBark.Framework.Effects;
using MoonBark.Framework.Slots;

/// <summary>
/// Implementation of IActionSlot for Hotbar usage.
/// Follows the slots interface contract from MoonBark Framework.
/// </summary>
public sealed class HotbarSlot : IActionSlot, IMutableSlot<ActionContent?>
{
    private ActionContent? _content;
    private CooldownInfo _cooldownInfo = CooldownInfo.Ready;

    public int SlotIndex { get; }

    public ActionContent? Content
    {
        get => _content;
        set => SetContent(value);
    }

    public bool IsEmpty => Content == null;

    public string DisplayName => Content?.DisplayName ?? string.Empty;

    public ICooldownInfo? CooldownInfo => _cooldownInfo;

    public bool HasCooldown => _cooldownInfo.IsActive;

    /// <summary>
    /// Event raised when slot content changes
    /// </summary>
    public event EventHandler<SlotChangeEventArgs<ActionContent?>>? ContentChanged;

    /// <summary>
    /// Event raised when slot is activated
    /// </summary>
    public event EventHandler<SlotActivationEventArgs>? Activated;

    public HotbarSlot(int slotIndex)
    {
        SlotIndex = slotIndex;
    }

    public bool CanActivate()
    {
        if (IsEmpty) return false;
        if (HasCooldown) return false;
        if (Content?.Effects == null || Content.Value.Effects.Count == 0) return false;

        return true;
    }

    public void Activate(EffectContext context)
    {
        if (!CanActivate())
            return;

        Activated?.Invoke(this, new SlotActivationEventArgs
        {
            SlotIndex = SlotIndex,
            DisplayName = DisplayName,
            Target = context.Target,
            Source = context.Source,
            Timestamp = DateTime.UtcNow
        });
    }

    public void StartCooldown(float durationSeconds)
    {
        if (durationSeconds <= 0)
            return;

        _cooldownInfo = CooldownInfo.FromElapsed(0, durationSeconds);
    }

    public void ClearCooldown()
    {
        _cooldownInfo = CooldownInfo.Ready;
    }

    public bool SetContent(ActionContent? content)
    {
        var previousContent = _content;
        _content = content;

        ClearCooldown();

        ContentChanged?.Invoke(this, new SlotChangeEventArgs<ActionContent?>
        {
            SlotIndex = SlotIndex,
            PreviousContent = previousContent,
            NewContent = content,
            Timestamp = DateTime.UtcNow
        });

        return true;
    }

    public bool SwapWith(IMutableSlot<ActionContent?> other)
    {
        if (other == this)
            return false;

        var temp = other.Content;
        other.SetContent(Content);
        SetContent(temp);

        return true;
    }

    /// <summary>
    /// Updates cooldown state for this slot
    /// </summary>
    /// <param name="deltaTime">Time elapsed since last update in seconds</param>
    public void UpdateCooldown(float deltaTime)
    {
        if (!_cooldownInfo.IsActive)
            return;

        var newElapsed = _cooldownInfo.ElapsedSeconds + deltaTime;
        _cooldownInfo = CooldownInfo.FromElapsed(newElapsed, _cooldownInfo.TotalSeconds);
    }
}
