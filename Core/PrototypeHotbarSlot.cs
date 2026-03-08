namespace PrototypeUI.Core;

public sealed record PrototypeHotbarSlot(
    int SlotIndex,
    PrototypeHotbarSlotKind Kind,
    string Id,
    string Label,
    string InputAction,
    string Binding,
    bool IsAvailable,
    string Detail,
    string IconText,
    string IconPath = "",
    bool IsOnCooldown = false,
    float CooldownProgress = 0.0f,
    float CooldownRemainingSeconds = 0.0f);
