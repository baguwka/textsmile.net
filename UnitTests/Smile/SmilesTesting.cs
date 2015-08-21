using NUnit.Framework;
using textsmile.net.Model.Smile;

namespace UnitTests.Smile {
   [TestFixture]
   public class SmilesTesting {
      [Test]
      public void Instantiate_two_smile_items_click_on_one_and_remove_another_check_it_out() {
         var smileAAAclicked = false;
         var smileAABclicked = false;
         var smileAACclicked = false;

         var smileAAAremoved = false;
         var smileAABremoved = false;
         var smileAACremoved = false;

         var collection = new SmileCollection();

         var smileAAA = collection.InstantiateSmile("AAA");
         var smileAAB = collection.InstantiateSmile("AAB");
         var smileAAC = collection.InstantiateSmile("AAC");

         collection.AddSmile(smileAAA);
         collection.AddSmile(smileAAB);
         collection.AddSmile(smileAAC);

         collection.SmileClicked += (sender, item) => {
            if (item == smileAAA) {
               smileAAAclicked = true;
            } else if (item == smileAAB) {
               smileAABclicked = true;
            } else if (item == smileAAC) {
               smileAACclicked = true;
            }
         };

         collection.SmileRemoved += (sender, item) => {
            if (item == smileAAA) {
               smileAAAremoved = true;
            } else if (item == smileAAB) {
               smileAABremoved = true;
            } else if (item == smileAAC) {
               smileAACremoved = true;
            }
         };

         smileAAB.ClickCommand.Execute(null);
         smileAAC.RemoveCommand.Execute(null);

         Assert.That(smileAAAclicked, Is.False);
         Assert.That(smileAABclicked, Is.True);
         Assert.That(smileAACclicked, Is.False);

         Assert.That(smileAAAremoved, Is.False);
         Assert.That(smileAABremoved, Is.False);
         Assert.That(smileAACremoved, Is.True);
      }
   }
}