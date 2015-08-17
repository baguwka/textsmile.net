using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using textsmile.net.Model;
using textsmile.net.Model.Smile;

namespace textsmile.net {
   [Serializable]
   public class AppSaveData : CommonSaveData {
      public List<string> Smiles { get; set; }
      public Key Key { get; set; }
      public ModifierKeys ModsKeys { get; set; }

      public AppSaveData(IEnumerable<SmileItem> smileItems) {
         Smiles = smileItems?.Select(w => w.Content).ToList();
      }
   }
}