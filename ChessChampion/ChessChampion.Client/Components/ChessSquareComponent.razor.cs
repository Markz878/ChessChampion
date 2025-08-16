using ChessChampion.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace ChessChampion.Client.Components;
public partial class ChessSquareComponent
{
    [Parameter][EditorRequired] public required GameSquare Square { get; set; }
    [Parameter][EditorRequired] public required bool IsSelected { get; set; }
    [Parameter][EditorRequired] public required bool IsMovable { get; set; }
    [Parameter][EditorRequired] public required EventCallback<GameSquare> HandleClickCallback { get; set; }

    private string GetColorClass()
    {
        int rowRemainder = Square.Y % 2;
        int columnRemainder = (Square.X + rowRemainder) % 2;
        if (IsSelected)
        {
            return "selected";
        }
        if (IsMovable)
        {
            return columnRemainder == 0 ? "movable-light" : "movable-dark";
        }
        return columnRemainder == 0 ? "light" : "dark";
    }

    private string GetBorderClass()
    {
        return Square.WasPreviousMove ? "strong-border" : "border";
    }

    private Task HandleClick()
    {
        return HandleClickCallback.InvokeAsync(Square);
    }
}