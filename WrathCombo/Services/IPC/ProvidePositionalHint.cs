using ECommons.EzIpcManager;
using WrathCombo.API.Enum;

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
