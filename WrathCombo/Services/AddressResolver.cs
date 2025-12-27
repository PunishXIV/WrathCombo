using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
namespace WrathCombo.Core;

/// <summary> Plugin address resolver. </summary>
internal class AddressResolver
{
    /// <summary> Gets the address of fpIsIconReplacable. </summary>
    public nint IsActionIdReplaceable { get; private set; }

    /// <inheritdoc />
    public void Setup(ISigScanner scanner)
    {
        IsActionIdReplaceable = scanner.ScanText("40 53 48 83 EC 20 8B D9 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 85 C0 74 1F");

        Svc.Log.Debug("===== W R A T H   C O M B O =====");
        Svc.Log.Debug($"{nameof(IsActionIdReplaceable)} 0x{IsActionIdReplaceable:X}");
    }
}
