using System.Windows;
using System.Windows.Input;

namespace KrishaKzDesktop.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    public ICommand CustomCommand { get; }
    public MainWindowViewModel()
    {
        CustomCommand = new LambdaCommand(e => MessageBox.Show("EXECUTED"), e => true);
    }
}
