using System;
using System.Collections.Generic;
using System.Windows.Forms;
using textsmile.net.VM;

namespace textsmile.net.UI {
   public class TrayContextMenu : ContextMenu {
      public TrayContextMenu(EventHandler onExit) {
         var quitItem = new MenuItem("Quit");
         quitItem.Click += onExit;

         MenuItems.Add(quitItem);
      }
   }

   //public class MainContextMenu : System.Windows.Controls.ContextMenu {
   //   public MainContextMenu(IEnumerable<SmileWrapper> smiles, EventHandler onConfigure) {

   //      var config = new MenuItem("Configure");
   //      config.Click += onConfigure;
   //      Items.Add(config);

   //      Items.Add("-");
         
   //      foreach (var smile in smiles) {
   //         var newItem = new MenuItem(smile.Content);
   //         newItem.Click += (sender, args) => {
   //            smile.ClickCommand.Execute(smile);
   //         };

   //         Items.Add(newItem);
   //      }
   //   }
   //}
}
