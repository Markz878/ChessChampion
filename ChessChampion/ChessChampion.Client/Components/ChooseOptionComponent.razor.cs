using Microsoft.AspNetCore.Components;

namespace ChessChampion.Client.Components;

public partial class ChooseOptionComponent
{
    [Parameter][EditorRequired] public required string Title { get; set; }
    [Parameter][EditorRequired] public required string LeftText { get; set; }
    [Parameter][EditorRequired] public required string RightText { get; set; }
    [Parameter] public bool IsDisabled { get; set; }
    [Parameter][EditorRequired] public required string TextWidth { get; set; }
    [Parameter] public EventCallback<bool> LeftSelected { get; set; }
    [Parameter] public bool IsLeftSelected { get; set; } = true;

    private string LeftButtonClass => IsLeftSelected ? "activeButton" : "";
    private string RightButtonClass => !IsLeftSelected ? "activeButton" : "";

    private Task SelectLeft()
    {
        IsLeftSelected = true;
        return LeftSelected.InvokeAsync(true);
    }

    private Task SelectRight()
    {
        IsLeftSelected = false;
        return LeftSelected.InvokeAsync(false);
    }
}