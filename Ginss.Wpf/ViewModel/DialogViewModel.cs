using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ginss.Wpf.ViewModel
{
    public partial class DialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = "Title";

        [ObservableProperty]
        private string message = "message.";

        [ObservableProperty]
        private PackIconKind iconKind = PackIconKind.InfoOutline;

        [ObservableProperty]
        private Brush brush = Brushes.Indigo; 
    }
}
