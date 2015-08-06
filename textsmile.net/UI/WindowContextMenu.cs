using System;
using System.Windows.Forms;

namespace textsmile.net.UI {
   public class WindowContextMenu : ContextMenu{
      public WindowContextMenu(EventHandler onExit) {
         var quitItem = new MenuItem("Quit");
         quitItem.Click += onExit;

         MenuItems.Add(quitItem);
      }
   }
}