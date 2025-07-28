using NeuroPOS.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Converters
{
    public class DTSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is not Product product)
                return null;

            var key = product.Stock<=5 ? "LowstockTemplate" : "MainTemplate";

            Application.Current.Resources.TryGetValue(key, out var template);
            return template as DataTemplate;

        }
    }
}
