using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using NSubstitute;
using textsmile.net.Model;
using NUnit.Framework;

namespace UnitTests.IO {
   [Serializable]
   public class TestData {
      public string Str;
      public List<string> Lst { get; set; }
      public Key Key { get; set; }
      public ModifierKeys ModsKeys { get; set; }
   }

   [Serializable]
   public class CorruptedTestData {
      public string Str {
         get { throw new NullReferenceException(); }
      }

      public List<string> Lst {
         get { throw new NullReferenceException(); }
      }

      public Key Key {
         get { throw new NullReferenceException(); }
      }

      public ModifierKeys ModsKeys {
         get { throw new NullReferenceException(); }
      }
   }

   //todo: DRY, too much copypaste
   [TestFixture]
   public class SaveLoadControllerTests {
      private void configureContainer(UnityContainer uc) {
         uc.RegisterType<SaveLoadController>(new ContainerControlledLifetimeManager());
         uc.RegisterType<ISerializer, SaveLoadJsonSerializer>();
         uc.RegisterType<IDataProvider, AppDataFolderProvider>();

      }

      private TestData createTestData() {
         return new TestData {
            Key = Key.K,
            ModsKeys = ModifierKeys.Windows & ModifierKeys.Alt,
            Lst = new List<string> {"first", "second", "third"},
            Str = "Name"
         };
      }

      [Test]
      public void SavingData() {
         string serialized = Encoding.UTF8.GetString(Properties.Resources.test);
         string result = string.Empty;

         var uc = new UnityContainer();
         configureContainer(uc);

         var dataProvider = Substitute.For<IDataProvider>();
         dataProvider.WhenForAnyArgs(x => x.Write("test", serialized)).Do(info => {
            result = info.ArgAt<string>(1);
         });

         uc.RegisterInstance(dataProvider);

         var controller = uc.Resolve<SaveLoadController>();

         var data = createTestData();
         controller.Save("test", data);

         Assert.That(serialized, Is.EqualTo(result));
      }

      [Test]
      public void SavingCorruptedData() {
         string serialized = Encoding.UTF8.GetString(Properties.Resources.corrupted_test);
         string result = string.Empty;
         bool corruptHandlerFired = false;
         bool dataProviderWriteCalled = false;

         var uc = new UnityContainer();
         configureContainer(uc);

         var dataProvider = Substitute.For<IDataProvider>();
         dataProvider.WhenForAnyArgs(x => x.Write("test", serialized)).Do(info => {
            result = info.ArgAt<string>(1);
            dataProviderWriteCalled = true;
         });

         uc.RegisterInstance(dataProvider);

         var controller = uc.Resolve<SaveLoadController>();

         var data = new CorruptedTestData();
         controller.Save("test", data, exception => {
            corruptHandlerFired = true;
            return true;
         });

         Assert.That(dataProviderWriteCalled, Is.False);
         Assert.That(corruptHandlerFired, Is.True);
         Assert.That(result, Is.EqualTo(SaveLoadController.WRITE_IF_CORRUPTED));
      }

      [Test]
      public void LoadingData() {
         string serialized = Encoding.UTF8.GetString(Properties.Resources.test);

         var uc = new UnityContainer();
         configureContainer(uc);

         var dataProvider = Substitute.For<IDataProvider>();
         dataProvider.Read("test").ReturnsForAnyArgs(serialized);

         uc.RegisterInstance(dataProvider);

         var controller = uc.Resolve<SaveLoadController>();
         TestData data;
         if (controller.TryLoad("test", out data)) {
            var sample = createTestData();

            Assert.That(sample.ModsKeys, Is.EqualTo(data.ModsKeys));
            Assert.That(sample.Key, Is.EqualTo(data.Key));
            Assert.That(sample.Str, Is.EqualTo(data.Str));
            Assert.That(sample.Lst, Is.EqualTo(data.Lst));
         }
         else {
            Assert.Fail();
         }
      }

      [Test]
      public void LoadingCorruptedData() {
         string serialized = Encoding.UTF8.GetString(Properties.Resources.corrupted_test);
         bool corruptHandlerFired = false;
         bool dataProviderWriteCalled = false;

         var uc = new UnityContainer();
         configureContainer(uc);

         var dataProvider = Substitute.For<IDataProvider>();
         dataProvider.Read("test").ReturnsForAnyArgs(serialized);
         dataProvider.WhenForAnyArgs(x => x.Write("test", serialized)).Do(info => {
            dataProviderWriteCalled = true;
         });

         uc.RegisterInstance(dataProvider);

         var controller = uc.Resolve<SaveLoadController>();

         var data = controller.Load<CorruptedTestData>("test", exception => {
            corruptHandlerFired = true;
            return true;
         });

         Assert.That(dataProviderWriteCalled, Is.True);
         Assert.That(corruptHandlerFired, Is.True);
         Assert.That(data, Is.Null);
      }
   }
}
