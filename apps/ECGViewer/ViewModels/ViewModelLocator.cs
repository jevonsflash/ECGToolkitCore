using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace ECGViewer.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
        }

        public MainWindowViewModel MainWindow => Ioc.Default.GetRequiredService<MainWindowViewModel>();



        public static void Cleanup<T>() where T : ObservableObject
        {

        }
    }
}