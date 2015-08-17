#define ISDEBUG

using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Unity;
using textsmile.net.GlobalHotkey;
using textsmile.net.Model;
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
         if (isAppAlreadyRunning()) {
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
         //Container.RegisterType<CohesiveUnit>(new ContainerControlledLifetimeManager());


         Container.Resolve<BackgroundService>().Run();
      }

      private void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
         MessageBox.Show(e.Exception.Message + "\n\n" + e.Exception.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);

#if !ISDEBUG
         e.Handled = true;
#endif

         Current.Shutdown();
      }

      private void onAppExit(object sender, ExitEventArgs exitEventArgs) {
         Container.Resolve<BackgroundService>().Close();
      }

      //todo: use mutex
      //todo: WindowState.Normal on existing app instance, no MessageBox
      private bool isAppAlreadyRunning() {
         var currentProcess = Process.GetCurrentProcess();

         var processes = Process.GetProcessesByName(currentProcess.ProcessName);

         if (processes.Any(p => p.Id != currentProcess.Id)) {

            MessageBox.Show("Another instance is already running.", "Application already running",
            MessageBoxButton.OK, MessageBoxImage.Exclamation);

            Current.Shutdown();
            return true;
         }
         return false;
      }
   }
}
