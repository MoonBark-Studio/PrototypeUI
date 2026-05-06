namespace MoonBark.PrototypeUI.Core;

public interface IHotbarActionSink
{
    bool TryActivateHotbarSlot(int slotIndex);
}
