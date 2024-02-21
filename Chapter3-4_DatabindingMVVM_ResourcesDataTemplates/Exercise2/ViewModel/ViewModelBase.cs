using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Exercise2.ViewModel;

public abstract class ViewModelBase : IViewModel
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public abstract void Load();
}