using Microsoft.AspNetCore.Components;

namespace ChessChampion.Client.Components;

public partial class SliderComponent
{
    [Parameter] public required string Description { get; set; }
    [Parameter] public int Min { get; set; }
    [Parameter] public int Max { get; set; }
    [Parameter] public int Value { get; set; }
    [Parameter] public EventCallback<int> ValueChangedHandler { get; set; }

    private async Task ValueChanged(string? value)
    {
        if (int.TryParse(value, out int parsedValue))
        {
            Value = parsedValue;
            await ValueChangedHandler.InvokeAsync(parsedValue);
            return;
        }
    }
}