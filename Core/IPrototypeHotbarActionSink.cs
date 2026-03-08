namespace PrototypeUI.Core;

public interface IPrototypeHotbarActionSink
{
    bool TryActivateHotbarSlot(int slotIndex);
}
