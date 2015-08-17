using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Mvvm;

namespace textsmile.net.Model.Smile {
   [UsedImplicitly]
   public class SmileCollection : BindableBase {
      private SmartCollection<SmileItem> _items;
      public event EventHandler<SmileItem> SmileClicked;
      public event EventHandler<SmileItem> SmileRemoved;

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

      public void AddSmile() {
         _items.Add(InstantiateSmile());
      }

      public SmileItem InstantiateSmile() {
         return InstantiateSmile(string.Empty);
      }

      public SmileItem InstantiateSmile(string content) {
         return new SmileItem(content) {
            RemoveHandler = OnSmileRemoved,
            ClickHandler = OnSmileClicked
         };
      }

      protected virtual void OnSmileClicked(SmileItem e) {
         SmileClicked?.Invoke(this, e);
      }

      protected virtual void OnSmileRemoved(SmileItem e) {
         SmileRemoved?.Invoke(this, e);
      }
   }
}