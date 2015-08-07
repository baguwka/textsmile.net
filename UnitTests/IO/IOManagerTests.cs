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

   [TestFixture]
   public class IOManagerTests {
      private void configureContainer(UnityContainer uc) {
         uc.RegisterType<IoManager>(new ContainerControlledLifetimeManager());
         uc.RegisterType<ISerializer, JsonIOSerializer>();
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
      public void SaveTest() {
         string serialized = Encoding.UTF8.GetString(Properties.Resources.test);
         string result = string.Empty;

         var uc = new UnityContainer();
         configureContainer(uc);

         var dataProvider = Substitute.For<IDataProvider>();
         dataProvider.WhenForAnyArgs(x => x.Write("test", serialized)).Do(info => {
            result = info.ArgAt<string>(1);
         });

         uc.RegisterInstance<IDataProvider>(dataProvider);

         var manager = uc.Resolve<IoManager>();

         var data = createTestData();
         manager.Save("test", data);

         Assert.That(serialized, Is.EqualTo(result));
      }

      [Test]
      public void LoadTest() {
         string serialized = Encoding.UTF8.GetString(Properties.Resources.test);

         var uc = new UnityContainer();
         configureContainer(uc);

         var dataProvider = Substitute.For<IDataProvider>();
         dataProvider.Read("test").ReturnsForAnyArgs(serialized);

         uc.RegisterInstance<IDataProvider>(dataProvider);

         var manager = uc.Resolve<IoManager>();
         TestData data;
         if (manager.TryLoad("test", out data)) {
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
   }
}
