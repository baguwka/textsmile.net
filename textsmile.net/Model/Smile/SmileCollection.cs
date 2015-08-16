using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Mvvm;

namespace textsmile.net.Model.Smile {
   [UsedImplicitly]
   public class SmileCollection : BindableBase {
      private SmartCollection<SmileItem> _items;

      public SmileCollection() {
         Items = new SmartCollection<SmileItem>();
         Items.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(Items));
      }

      public SmartCollection<SmileItem> Items {
         get { return _items; }
         set { SetProperty(ref _items, value); }
      }

      public void Load(IEnumerable<SmileItem> smiles) {
         _items.Clear();
         _items.AddRange(smiles);
      }
   }
}