using Microsoft.Maui.Controls;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using System;

namespace NeuroPOS.Converters
{
    public class CategoryTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is not Category category)
            {
                return null;
            }

            var key = category.State == "Active Categorie" ? "ActiveCategoryTemplate" : "InactiveCategoryTemplate";


            Application.Current.Resources.TryGetValue(key, out var template);          
            return template as DataTemplate;
        }

    }
}