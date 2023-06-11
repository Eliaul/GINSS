using CommunityToolkit.Mvvm.ComponentModel;
using Ginss.Wpf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Wpf.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public List<LogContent> Logs { get; set; }

    }
}
