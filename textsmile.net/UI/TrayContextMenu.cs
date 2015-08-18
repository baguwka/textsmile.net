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
}
