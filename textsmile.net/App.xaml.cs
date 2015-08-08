using System.Diagnostics;
using System.Linq;
using System.Windows;
using GlobalHotKey;
using Microsoft.Practices.Unity;
using textsmile.net.Model;

namespace textsmile.net {
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application {
      private static UnityContainer _container;

      public static UnityContainer Container
      {
         get { return _container; }
         private set { _container = value; }
      }

      private void AppStartup(object sender, StartupEventArgs e) {
         //ConsoleManager.Show();
         isAppAlreadyRunning();
         _container = new UnityContainer();

         _container.RegisterType<HotKeyManager>(new ContainerControlledLifetimeManager());

         _container.RegisterType<ISerializer, JsonIOSerializer>();
         _container.RegisterType<IDataProvider, AppDataFolderProvider>();
         _container.RegisterType<IoManager>(new ContainerControlledLifetimeManager());
      }

      //todo: use mutex
      private void isAppAlreadyRunning() {
         Process currentProcess = Process.GetCurrentProcess();

         var processes = Process.GetProcessesByName(currentProcess.ProcessName);

         if (processes.Any(p => p.Id != currentProcess.Id)) {
            MessageBox.Show("Another instance is already running.", "Application already running",
            MessageBoxButton.OK, MessageBoxImage.Exclamation);

            Current.Shutdown();
            return;
         }
      }
   }
}
