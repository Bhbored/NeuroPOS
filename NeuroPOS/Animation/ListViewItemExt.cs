using Syncfusion.Maui.ListView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Animation
{
    public class ListViewItemExt : ListViewItem
    {
        private SfListView listView;

        public ListViewItemExt(SfListView listView)
        {
            this.listView = listView;
        }

        protected override void OnItemAppearing()
        {
            this.Opacity = 0;
            this.FadeTo(1, 400, Easing.SinInOut);
            base.OnItemAppearing();
        }
    }
}
