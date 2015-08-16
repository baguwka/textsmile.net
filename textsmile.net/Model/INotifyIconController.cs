using System.Windows.Forms;

namespace textsmile.net.Model {
   public interface INotifyIconController {
      NotifyIcon Tray { get; set; }
      
      void Close();
      void Create(string name, ContextMenu contextMenu);
   }
}