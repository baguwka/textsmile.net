using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using JetBrains.Annotations;

namespace textsmile.net.GlobalHotkey {
   /// <summary>
   /// Setups system-wide hot keys and provides possibility to react on their events.
   /// </summary>
   [UsedImplicitly]
   public class HotKeyManager : IDisposable {
      private readonly IWinApi _api;
      private readonly HwndSource _windowHandleSource;

      private readonly Dictionary<string, HotKey> _registered;

      /// <summary>
      /// Occurs when registered system-wide hot key is pressed.
      /// </summary>
      public event EventHandler<HotkeyEventArgs> KeyPressed;
      /// <summary>
      /// Occurs when hotkey becomes registered
      /// </summary>
      public event EventHandler<HotkeyEventArgs> KeyRegistered;
      /// <summary>
      /// Occurs when hotkey becomes unregistered
      /// </summary>
      public event EventHandler<HotkeyEventArgs> KeyUnregistered;

      /// <summary>
      /// Initializes a new instance of the <see cref="HotKeyManager"/> class.
      /// </summary>
      public HotKeyManager(IWinApi api) {
         _api = api;
         _windowHandleSource = new HwndSource(new HwndSourceParameters());
         _windowHandleSource.AddHook(messagesHandler);

         _registered = new Dictionary<string, HotKey>();
      }

      /// <summary>
      /// Registers the system-wide hot key.
      /// </summary>
      /// <param name="hotKey">The hot key.</param>
      public void Register(string label, HotKey hotKey) {
         // Check if specified hot key is already registered.
         if (_registered.ContainsValue(hotKey))
            throw new ArgumentException("The specified hot key is already registered.");

         // Register new hot key.
         var id = getFreeKeyId();
         if (!_api.RegisterHotKey(_windowHandleSource.Handle, id, hotKey.Key, hotKey.Modifiers)) {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Can't register the hot key.");
         }

         hotKey.ID = id;
         _registered.Add(label, hotKey);
         onKeyRegistered(new HotkeyEventArgs(label, hotKey));
      }

      /// <summary>
      /// Unregisters previously registered hot key.
      /// </summary>
      public void Unregister(string label) {
         HotKey hotKey;
         if (_registered.TryGetValue(label, out hotKey)) {
            WinApi.UnregisterHotKey(_windowHandleSource.Handle, hotKey.ID);
            _registered.Remove(label);
            onKeyUnregistered(new HotkeyEventArgs(label, hotKey));
         }
      }

      /// <summary>
      /// Unregisters previously registered hot key.
      /// </summary>
      /// <param name="hotKey">The registered hot key.</param>
      public void Unregister(HotKey hotKey) {
         if (_registered.ContainsValue(hotKey)) {
            var item = _registered.FirstOrDefault(r => r.Value.Equals(hotKey));
            if (item.Value.Equals(hotKey)) {
               WinApi.UnregisterHotKey(_windowHandleSource.Handle, hotKey.ID);
               _registered.Remove(item.Key);
               onKeyUnregistered(new HotkeyEventArgs(item.Key, hotKey));
            }
         }
      }

      public HotKey GetHotkey(string label) {
         if (_registered.ContainsKey(label)) {
            return _registered[label];
         }
         return null;
      }

      public string GetLabel(HotKey hotKey) {
         if (_registered.ContainsValue(hotKey)) {
            return _registered.FirstOrDefault(r => r.Value.Equals(hotKey)).Key;
         }
         return string.Empty;
      }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose() {
         // Unregister hot keys.
         foreach (var kvp in _registered) {
            WinApi.UnregisterHotKey(_windowHandleSource.Handle, kvp.Value.ID);
         }

         _registered.Clear();

         _windowHandleSource.RemoveHook(messagesHandler);
         _windowHandleSource.Dispose();
      }

      private int getFreeKeyId() {
         return _registered.Any() ? _registered.Values.Select(h => h.ID).Max() + 1 : 0;
      }

      private IntPtr messagesHandler(IntPtr handle, int message, IntPtr wParam, IntPtr lParam, ref bool handled) {
         if (message == _api.WmHotKey) {
            // Extract key and modifiers from the message.
            var key = KeyInterop.KeyFromVirtualKey(((int)lParam >> 16) & 0xFFFF);
            var modifiers = (ModifierKeys)((int)lParam & 0xFFFF);

            var hotKey = new HotKey(key, modifiers);
            var label = _registered.FirstOrDefault(r => r.Value.Equals(hotKey)).Key;
            onKeyPressed(new HotkeyEventArgs(label, hotKey));

            handled = true;
            return new IntPtr(1);
         }

         return IntPtr.Zero;
      }

      protected virtual void onKeyPressed(HotkeyEventArgs e) {
         var handler = KeyPressed;
         handler?.Invoke(this, e);
      }

      protected virtual void onKeyRegistered(HotkeyEventArgs e) {
         KeyRegistered?.Invoke(this, e);
      }

      protected virtual void onKeyUnregistered(HotkeyEventArgs e) {
         KeyUnregistered?.Invoke(this, e);
      }
   }
}