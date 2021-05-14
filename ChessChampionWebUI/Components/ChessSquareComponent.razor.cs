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
            if (Square.State == SquareState.Selected)
            {
                return "selected";
            }
            else if (Square.State == SquareState.Movable)
            {
                return "movable";
            }
            int rowRemainder = Square.Row % 2;
            int columnRemainder = (Square.Column + rowRemainder) % 2;
            return columnRemainder == 0 ? "light" : "dark";
        }

        private Task HandleClick()
        {
            return HandleClickCallback.InvokeAsync(Square);
        }
    }
}
