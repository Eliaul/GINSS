using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ginss.Wpf.Model
{
    public class LogContent
    {
        public string Description { get; set; }

        public string LogTime { get; set; }

        public string ImgName { get; set; }

        public ImageSource ImgSource =>
            new BitmapImage(new Uri("Icon/" + ImgName, UriKind.Relative));

    }
}
