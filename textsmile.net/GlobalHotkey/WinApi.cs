using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using JetBrains.Annotations;

namespace textsmile.net.GlobalHotkey {
   [UsedImplicitly]
   public class WinApi : IWinApi {
      public int WmHotKey { get; set; } = 786;

      public bool RegisterHotKey(IntPtr handle, int id, Key key, ModifierKeys modifiers) {
         int num = KeyInterop.VirtualKeyFromKey(key);
         return RegisterHotKey(handle, id, (uint)modifiers, (uint)num);
      }

      [DllImport("user32.dll", SetLastError = true)]
      private static extern bool RegisterHotKey(IntPtr handle, int id, uint modifiers, uint virtualCode);

      [DllImport("user32.dll", SetLastError = true)]
      public static extern bool UnregisterHotKey(IntPtr handle, int id);
   }
}