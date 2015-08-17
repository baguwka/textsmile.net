using System;

namespace textsmile.net.GlobalHotkey {
   /// <summary>
   /// Arguments for key pressed event which contain information about pressed hot key.
   /// </summary>
   public class HotkeyEventArgs : EventArgs {
      /// <summary>
      /// Initializes a new instance of the <see cref="HotkeyEventArgs"/> class.
      /// </summary>
      /// <param name="hotKey">The hot key.</param>
      public HotkeyEventArgs(string label, HotKey hotKey) {
         Label = label;
         HotKey = hotKey;
      }

      /// <summary>
      /// Gets the hot key.
      /// </summary>
      public HotKey HotKey { get; private set; }
      public string Label { get; private set; }
   }
}