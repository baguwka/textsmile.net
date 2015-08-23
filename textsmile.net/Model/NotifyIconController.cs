using System.Monads;
using System.Windows.Forms;
using JetBrains.Annotations;
using textsmile.net.Properties;

namespace textsmile.net.Model {
   [UsedImplicitly]
   public class NotifyIconController : INotifyIconController {
      public NotifyIconController() {}

      public NotifyIcon Tray { get; set; }

      //@"Textsmile.net"
      public void Create(string name, ContextMenu contextMenu) {
         if (Tray?.Visible == true) {
            Close();
         }

         Tray = new NotifyIcon {
            Text = name,
            Icon = Resources.icon16x16,
            ContextMenu = contextMenu,
            Visible = true
         };
      }

      public void Close() {
         Tray.With(t => t.Visible = false);
         Tray = null;
      }
   }
}