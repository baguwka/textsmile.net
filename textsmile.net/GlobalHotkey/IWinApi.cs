using System;
using System.Windows.Input;

namespace textsmile.net.GlobalHotkey {
   public interface IWinApi {
      int WmHotKey { get; set; }
      bool RegisterHotKey(IntPtr handle, int id, Key key, ModifierKeys modifiers);
   }
}