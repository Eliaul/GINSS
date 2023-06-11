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
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjM1NDg4N0AzMjMxMmUzMDJlMzBIcEhSZFI3ZDg0SDVXaE1HalpYQWZocXl0TjBLWjVtcHNrckE5cjA5bzBRPQ==;Mgo+DSMBaFt+QHJqVk1mQ1BAaV1CX2BZf1J8QGpTf1pgFChNYlxTR3ZZQlphSntadkFgXHlb;Mgo+DSMBMAY9C3t2VFhiQlJPcEBDWXxLflF1VWJYdVt5flVCcDwsT3RfQF5jT35UdEZmUH9ac3NVQw==;Mgo+DSMBPh8sVXJ1S0R+X1pCaV5GQmFJfFBmRGJTeld6dFFWACFaRnZdQV1lSXlRdUBqWXtdeHBQ;MjM1NDg5MUAzMjMxMmUzMDJlMzBZcWdaUERVajhzcTdiRjJIWGFNclppdHdXdFFURG1oMWlqZ0YzYUVNUkt3PQ==;NRAiBiAaIQQuGjN/V0d+Xk9HfVldXGdWfFN0RnNYflR1fV9GZEwxOX1dQl9gSXhTcUdgXHZedHBUTmU=;ORg4AjUWIQA/Gnt2VFhiQlJPcEBDWXxLflF1VWJYdVt5flVCcDwsT3RfQF5jT35UdEZmUH9adHdXQw==;MjM1NDg5NEAzMjMxMmUzMDJlMzBidTlLaWQ3bVpkdEt6TTh6cmNKTFBqTUpsT2tnUlZpWmxZLzlyRmtTaFFJPQ==;MjM1NDg5NUAzMjMxMmUzMDJlMzBVQTczV1Z3YkF3ZExVZm9GbHFLV29xZUU2WXRSSkFsSWI3WC9MS2Y0VXZZPQ==;MjM1NDg5NkAzMjMxMmUzMDJlMzBQSncvUEZxZ0hnNzZxdGE1QXRoK25yYmxQODJUKzA1SWZsZnBrWndHQnI4PQ==;MjM1NDg5N0AzMjMxMmUzMDJlMzBEYUV4VlFKcWw4VDlWUUV5SURsdFZpWFNOMmozQzUvZzNnNVgwYTI5bW5VPQ==;MjM1NDg5OEAzMjMxMmUzMDJlMzBIcEhSZFI3ZDg0SDVXaE1HalpYQWZocXl0TjBLWjVtcHNrckE5cjA5bzBRPQ==");
            var setting = new MaterialLightThemeSettings()
            {
                Palette = Syncfusion.Themes.MaterialLight.WPF.MaterialPalette.Indigo
            };
            SfSkinManager.RegisterThemeSettings("MaterialLight", setting);
            InitializeComponent();

            //var mainWindowViewModel = new MainWindowViewModel();

            DataContext = new MainWindowViewModel();

            //outputBox.ItemsSource = mainWindowViewModel.Logs;
        }

    }
}
