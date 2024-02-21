using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Exercise2.ViewModel;

public abstract class ViewModelBase : IViewModel
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public abstract void Load();
}