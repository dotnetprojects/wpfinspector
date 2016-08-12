using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace ChristianMoser.WpfInspector.UserInterface.Converters
{
    public class ValueConverterBase : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
