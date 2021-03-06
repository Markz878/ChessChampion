using ChessChampionWebUI.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Components
{
    public partial class ChessSquareComponent
    {
        [Parameter] public GameSquare Square { get; set; }
        [Parameter] public EventCallback<GameSquare> HandleClickCallback { get; set; } 

        private string GetColorClass()
        {
            int rowRemainder = Square.Y % 2;
            int columnRemainder = (Square.X + rowRemainder) % 2;
            return Square.State switch
            {
                SquareState.Selected => "selected",
                SquareState.Movable => columnRemainder == 0 ? "movable-light" : "movable-dark",
                _ => columnRemainder == 0 ? "light" : "dark",
            };
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
}
