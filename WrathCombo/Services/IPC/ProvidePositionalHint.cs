using ECommons.EzIpcManager;

namespace WrathCombo.Services.IPC;

public partial class Provider
{
    /// <summary>
    ///     Returns the current upcoming positional hint, or <see langword="null" />
    ///     when no hint is active.
    /// </summary>
    [EzIPC]
    public uint[]? GetUpcomingPositionalHint() =>
        UpcomingPositionalHintService.GetWireSnapshot();
}
