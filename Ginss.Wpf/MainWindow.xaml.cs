using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Ginss.Wpf.ViewModel;
using MaterialDesignThemes.Wpf;
using Syncfusion.SfSkinManager;
using Syncfusion.Themes.MaterialLightBlue.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using NaviTools;

using Ginss.Core.INS;
using System.Threading;
using System.Formats.Asn1;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using Syncfusion.Themes.MaterialLight.WPF;

namespace Ginss.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt+QHJqXE1hXk5Hd0BLVGpAblJ3T2ZQdVt5ZDU7a15RRnVfRFxnS3xXdEViXnZWcw==;Mgo+DSMBPh8sVXJ1S0R+VVpFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF5jT39XdEdnW39feHVTRQ==;ORg4AjUWIQA/Gnt2VFhiQlhPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXhSckdhXX1ecXxWRWM=;MjQyMjI2NUAzMjMxMmUzMDJlMzBSWmdsdlg0Y0NrQXBhcVVLVFRjczV5UHExdTRSS1o4TU9sL1doVXo0VXc0PQ==;MjQyMjI2NkAzMjMxMmUzMDJlMzBOVTlUOElPSWRzc3VxeHZiaGcvN3p1eEVKTUFhQ0p0bWpSaDBqemY4NFRJPQ==;NRAiBiAaIQQuGjN/V0d+Xk9NfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5Vd0FhW3tccHZVRWhf;MjQyMjI2OEAzMjMxMmUzMDJlMzBlbDJiMUNZU1pvalJ5ODExdW80cExlK2lZNzhEL0VHTlY3SWlXU0x3NWhFPQ==;MjQyMjI2OUAzMjMxMmUzMDJlMzBEVDl1Ri9wNU9MQlNyQW1SRTFoeEFnUERzL2RlTEE4cFJFN0pTUG1ESWQwPQ==;Mgo+DSMBMAY9C3t2VFhiQlhPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXhSckdhXX1ecnVTTmM=;MjQyMjI3MUAzMjMxMmUzMDJlMzBpMzd4NTNtTUR6K1Y4VTRTUTg3MnpuQW45cnQzWkFqMFdvVUdUY3BzZjUwPQ==;MjQyMjI3MkAzMjMxMmUzMDJlMzBlVEVNK2w5cWdtRHBKa3hxYmVKY3BjaURwVGJxWjV5NTUyaysrTUxyNThRPQ==;MjQyMjI3M0AzMjMxMmUzMDJlMzBlbDJiMUNZU1pvalJ5ODExdW80cExlK2lZNzhEL0VHTlY3SWlXU0x3NWhFPQ==");
            var setting = new MaterialLightThemeSettings()
            {
                Palette = Syncfusion.Themes.MaterialLight.WPF.MaterialPalette.Indigo
            };
            SfSkinManager.RegisterThemeSettings("MaterialLight", setting);
            //SfSkinManager.SetTheme(this, new Syncfusion.SfSkinManager.Theme("MaterialLight"));
            InitializeComponent();

            //var mainWindowViewModel = new MainWindowViewModel();

            DataContext = new MainWindowViewModel();
            
            
            //outputBox.ItemsSource = mainWindowViewModel.Logs;
        }

    }
}
