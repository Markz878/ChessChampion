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
            if (Square.State == SquareState.Selected)
            {
                return "selected";
            }
            else if (Square.State == SquareState.Movable)
            {
                return columnRemainder == 0 ? "movable-light" : "movable-dark";
            }
            return columnRemainder == 0 ? "light" : "dark";
        }

        private Task HandleClick()
        {
            return HandleClickCallback.InvokeAsync(Square);
        }
    }
}
