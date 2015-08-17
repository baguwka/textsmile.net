using System;
using System.Windows.Input;
using NSubstitute;
using NUnit.Framework;
using textsmile.net.GlobalHotkey;

namespace UnitTests.Hotkey {
   [TestFixture]
   public class HotKeyManagerTesting {
      [STAThread]
      [Test]
      public void Register_hotkey_and_check_that_api_called_and_manager_can_give_correct_data_back() {
         var apiCalled = false;

         var winApi = Substitute.For<IWinApi>();

         winApi.WhenForAnyArgs(x => x.RegisterHotKey(IntPtr.Zero, 0, Key.A, ModifierKeys.Alt)).Do(info => {
            apiCalled = true;
         });

         winApi.RegisterHotKey(IntPtr.Zero, 0, Key.A, ModifierKeys.Alt).ReturnsForAnyArgs(true);

         var manager = new HotKeyManager(winApi);

         var hotKey = new HotKey(Key.T, ModifierKeys.Shift);
         var label = "test";

         manager.Register(label, hotKey);

         var resultHotKey = manager.GetHotkey(label);
         var resultLabel = manager.GetLabel(hotKey);

         Assert.That(resultHotKey, Is.EqualTo(hotKey));
         Assert.That(resultLabel, Is.EqualTo(label));
         Assert.That(apiCalled, Is.True);
      }

      [STAThread]
      [Test]
      public void Register_hotkey_and_then_dispose_hotkeyManager_and_check_that_it_disposed_correct() {
         var winApi = Substitute.For<IWinApi>();

         winApi.RegisterHotKey(IntPtr.Zero, 0, Key.A, ModifierKeys.Alt).ReturnsForAnyArgs(true);

         var manager = new HotKeyManager(winApi);

         var hotKey = new HotKey(Key.T, ModifierKeys.Shift);
         var label = "test";

         manager.Register(label, hotKey);

         var resultHotKey = manager.GetHotkey(label);
         var resultLabel = manager.GetLabel(hotKey);

         Assert.That(resultHotKey, Is.EqualTo(hotKey));
         Assert.That(resultLabel, Is.EqualTo(label));

         manager.Dispose();

         resultHotKey = manager.GetHotkey(label);
         resultLabel = manager.GetLabel(hotKey);

         Assert.That(resultHotKey, Is.Null);
         Assert.That(resultLabel, Is.Null.Or.Empty);
      }

      [STAThread]
      [Test]
      public void Register_hotkey_and_then_unregister_it_check_that_hotkey_do_not_exists_anmymore() {
         var winApi = Substitute.For<IWinApi>();

         winApi.RegisterHotKey(IntPtr.Zero, 0, Key.A, ModifierKeys.Alt).ReturnsForAnyArgs(true);

         var manager = new HotKeyManager(winApi);

         var hotKey = new HotKey(Key.T, ModifierKeys.Shift);
         var label = "test";

         manager.Register(label, hotKey);

         var resultHotKey = manager.GetHotkey(label);
         var resultLabel = manager.GetLabel(hotKey);

         Assert.That(resultHotKey, Is.EqualTo(hotKey));
         Assert.That(resultLabel, Is.EqualTo(label));

         manager.Unregister(label);

         resultHotKey = manager.GetHotkey(label);
         resultLabel = manager.GetLabel(hotKey);

         Assert.That(resultHotKey, Is.Null);
         Assert.That(resultLabel, Is.Null.Or.Empty);
      }
   }
}