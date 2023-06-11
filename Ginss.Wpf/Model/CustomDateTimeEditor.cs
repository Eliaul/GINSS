using Syncfusion.Windows.PropertyGrid;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Ginss.Wpf.Model
{
    public class CustomDateTimeEditor : BaseTypeEditor
    {
        DateTimeEdit dateTimeEdit;

        public override void Attach(PropertyViewItem property, PropertyItem info)
        {
            if (info.CanWrite)
            {
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.TwoWay,
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(dateTimeEdit, DateTimeEdit.DateTimeProperty, binding);
            }
            else
            {
                dateTimeEdit.IsEnabled = false;
                var binding = new Binding("Value")
                {
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(dateTimeEdit, DateTimeEdit.DateTimeProperty, binding);
            }
        }

        public override object Create(PropertyInfo PropertyInfo)
        {
            return CreateEditor();
        }

        public override object Create(PropertyDescriptor PropertyDescriptor)
        {
            return CreateEditor();
        }

        public override void Detach(PropertyViewItem property)
        {
            dateTimeEdit = null;
        }

        private DateTimeEdit CreateEditor()
        {
            dateTimeEdit = new DateTimeEdit()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                BorderThickness = new Thickness(0),
                Pattern = DateTimePattern.CustomPattern,
                CustomPattern = "yyyy/MM/dd HH:mm:ss.fff",
                CanEdit = true,
                DropDownView = DropDownViews.Combined
            };
            return dateTimeEdit;
        }
    }
}
