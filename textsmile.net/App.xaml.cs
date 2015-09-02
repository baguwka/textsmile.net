using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Unity;
using textsmile.net.GlobalHotkey;
using textsmile.net.Model;
using textsmile.net.Model.Shortcut;
using textsmile.net.Model.Smile;

namespace textsmile.net {
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application {
      private static IUnityContainer _container;

      public static IUnityContainer Container
      {
         get { return _container; }
         private set { _container = value; }
      }

      private void CompositionRoot(object sender, StartupEventArgs e) {
         if (SingleInstance.IsOnlyInstance() == false) {
            SingleInstance.ShowFirstInstance();
            Current.Shutdown();
            return;
         }

         Current.Exit += onAppExit;
         Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
         Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

         Container = new UnityContainer();

         Container.RegisterType<HotKeyManager>(new ContainerControlledLifetimeManager());

         Container.RegisterType<IWinApi, WinApi>();
         Container.RegisterType<ISerializer, SaveLoadJsonSerializer>();
         Container.RegisterType<IDataProvider, AppDataFolderProvider>();
         Container.RegisterType<SaveLoadController>(new ContainerControlledLifetimeManager());

         Container.RegisterType<INotifyIconController, NotifyIconController>(new ContainerControlledLifetimeManager());
         Container.RegisterType<BackgroundService>(new ContainerControlledLifetimeManager());
         Container.RegisterType<SmileCollection>(new ContainerControlledLifetimeManager());
         Container.RegisterType<IShortcutCreator, WSHShortcutCreator>(new ContainerControlledLifetimeManager());

         bool startMinimized = false;
         for (int i = 0; i != e.Args.Length; ++i) {
            if (e.Args[i] == "-minimized") {
               startMinimized = true;
            }
         }

         Container.Resolve<BackgroundService>().Run(startMinimized);
      }

      private void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
         MessageBox.Show(e.Exception.Message + "\n\n See dump at Windows Application Event Log", "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);

#if !DEBUG
         e.Handled = true;
#endif

         Environment.FailFast("Exception occured", e.Exception);
      }

      private void onAppExit(object sender, ExitEventArgs exitEventArgs) {
         Container.Resolve<BackgroundService>().Close();
      }
   }
}
