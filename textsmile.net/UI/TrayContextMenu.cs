using System;
using System.Windows.Forms;

namespace textsmile.net.UI {
   public class TrayContextMenu : ContextMenu {
      public TrayContextMenu(EventHandler onExit) {
         var quitItem = new MenuItem("Quit");
         quitItem.Click += onExit;

         MenuItems.Add(quitItem);
      }
   }
}
