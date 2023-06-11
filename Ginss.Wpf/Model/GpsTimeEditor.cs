using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.PropertyGrid;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Ginss.Wpf.Model
{
    public class GpsTimeEditor : BaseTypeEditor
    {
        private PropertyViewItem propertyViewItem;

        SfMaskedEdit maskededit;

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
                BindingOperations.SetBinding(maskededit, SfMaskedEdit.ValueProperty, binding);
            }
            else
            {
                maskededit.IsEnabled = false;
                var binding = new Binding("Value")
                {
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(maskededit, SfMaskedEdit.ValueProperty, binding);
            }
        }

        public override object Create(PropertyInfo propertyInfo)
        {
            maskededit = new SfMaskedEdit
            {
                BorderThickness = new System.Windows.Thickness(0),
                MaskType = MaskType.RegEx,
                Mask = "[0-9]{4}\\s[0-9]+\\.[0-9]+"
            };
            return maskededit;
        }

        public override object Create(PropertyDescriptor PropertyDescriptor)
        {
            throw new NotImplementedException();
        }

        public override void Detach(PropertyViewItem property)
        {
            maskededit = null;
        }



        public override bool ShouldPropertyGridTryToHandleKeyDown(Key key)
        {
            return false;
        }
    }
}
