using CommunityToolkit.Mvvm.ComponentModel;
using Ginss.Wpf.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Wpf.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<LogContent> Logs { get; set; } = new();

    }
}
