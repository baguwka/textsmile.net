using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using NSubstitute;
using NUnit.Framework;
using textsmile.net.Model;
using UnitTests.Properties;

namespace UnitTests.SaveLoad {
   [Serializable]
   public class TestData {
      public string Str;
      public string RuStr;
      public List<string> Lst { get; set; }
      public Key Key { get; set; }
      public ModifierKeys ModsKeys { get; set; }
   }

   [Serializable]
   public class CorruptedTestData {
      public string Str {
         get { throw new NullReferenceException(); }
      }

      public string RuStr {
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
      }

      private TestData createTestData() {
         return new TestData {
            Key = Key.K,
            ModsKeys = ModifierKeys.Windows & ModifierKeys.Alt,
            Lst = new List<string> { "first", "second", "third" },
            Str = "Name",
            RuStr = "Имя"
         };
      }

      [Test]
      public void Save_data_and_then_compare_it_to_reference() {
         string reference = Encoding.Default.GetString(Resources.test);
         string result = string.Empty;

         var uc = new UnityContainer();
         configureContainer(uc);

         var dataProvider = Substitute.For<IDataProvider>();
         dataProvider.WhenForAnyArgs(x => x.Write("test", reference)).Do(info => {
            result = info.ArgAt<string>(1);
         });

         uc.RegisterInstance(dataProvider);

         var controller = uc.Resolve<SaveLoadController>();

         var data = createTestData();
         controller.Save("test", data);

         Assert.That(reference, Is.EqualTo(result), "Sample and result of Json serialization differs, but must be same.");
      }

      [Test]
      public void Create_instance_of_corrupted_data_and_check_that_CorruptedHandler_fired_and_dataProviders_write_call_blocked() {
         string serialized = Encoding.Default.GetString(Resources.corrupted_test);
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

         Assert.That(dataProviderWriteCalled, Is.False, "IDataProvider.Write() must NOT be called because data is corrupted and we dont want to write it");
         Assert.That(corruptHandlerFired, Is.True, "dataCorruptedHandler must be called because of fired exception while corrupted data deserialization.");
         Assert.That(result, Is.EqualTo(SaveLoadController.WRITE_IF_CORRUPTED));
      }

      [Test]
      public void Load_data_from_reference_and_then_compare_it_with_sample_object() {
         string reference = Encoding.Default.GetString(Resources.test);

         var uc = new UnityContainer();
         configureContainer(uc);

         var dataProvider = Substitute.For<IDataProvider>();
         dataProvider.Read("test").ReturnsForAnyArgs(reference);

         uc.RegisterInstance(dataProvider);

         var controller = uc.Resolve<SaveLoadController>();
         TestData result;
         if (controller.TryLoad("test", out result)) {
            var sampleObject = createTestData();

            var message = "One of TestData parameters differs, they must be the same.";

            Assert.That(result.ModsKeys, Is.EqualTo(sampleObject.ModsKeys), message);
            Assert.That(result.Key, Is.EqualTo(sampleObject.Key), message);
            Assert.That(result.Str, Is.EqualTo(sampleObject.Str), message);
            Assert.That(result.RuStr, Is.EqualTo(sampleObject.RuStr), message);
            Assert.That(result.Lst, Is.EqualTo(sampleObject.Lst), message);
         }
         else {
            Assert.Fail("Data is null, but correct data is expected.");
         }
      }

      [Test]
      public void Load_corrupted_reference_and_check_that_CorruptedHandler_fired_and_data_rewriten() {
         string serialized = Encoding.Default.GetString(Resources.corrupted_test);
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

         Assert.That(dataProviderWriteCalled, Is.True, "IDataProvider.Write() must be called because data is corrupted and we want to rewrite it.");
         Assert.That(corruptHandlerFired, Is.True, "dataCorruptedHandler must be called because of fired exception while corrupted data deserialization.");
         Assert.That(data, Is.Null, "Data must be null because data is corrupted and we expect null then.");
      }
   }
}
