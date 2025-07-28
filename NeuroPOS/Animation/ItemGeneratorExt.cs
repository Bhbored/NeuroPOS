using Syncfusion.Maui.ListView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Animation
{
    public class ItemGeneratorExt : ItemsGenerator
    {
        public SfListView listView;

        public ItemGeneratorExt(SfListView listView) : base(listView)
        {
            this.listView = listView;
        }

        protected override ListViewItem OnCreateListViewItem(int itemIndex, ItemType type, object data = null)
        {
            if (type == ItemType.Record)
                return new ListViewItemExt(this.listView);
            return base.OnCreateListViewItem(itemIndex, type, data);
        }
    }
}
