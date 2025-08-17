using System.Numerics;
namespace WrathCombo.Extensions;

internal class ImGuiExtensions
{
    //Lifted from ReAction. It's very neat.
    private static void AddTextCentered(Vector2 pos, string text, uint color)
    {
        var textSize = ImGui.CalcTextSize(text);
        ImGui.GetWindowDrawList().AddText(pos - textSize / 2, color, text);
    }
    public static void Prefix(string prefix = "◇")
    {
        var dummySize = new Vector2(ImGui.GetFrameHeight());
        ImGui.Dummy(dummySize);
        AddTextCentered(ImGui.GetItemRectMin() + dummySize / 2, prefix, ImGui.GetColorU32(ImGuiCol.Text));
        ImGui.SameLine();
    }

    public static void Prefix(bool isLast) => Prefix(isLast ? "└" : "├");
}