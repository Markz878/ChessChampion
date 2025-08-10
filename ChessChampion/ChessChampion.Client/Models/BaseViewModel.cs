namespace ChessChampion.Client.Models;

public abstract class BaseViewModel
{
    public Action? StateHasChanged { get; set; }

    public void SetProperty<T>(ref T previousValue, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(value, previousValue))
        {
            previousValue = value;
            StateHasChanged?.Invoke();
        }
    }
}
