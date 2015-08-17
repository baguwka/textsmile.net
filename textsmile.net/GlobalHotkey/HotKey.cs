using System.Text;
using System.Windows.Input;

namespace textsmile.net.GlobalHotkey {
   /// <summary>
   /// Represents system-wide hot key.
   /// </summary>
   public class HotKey {
      /// <summary>
      /// Initializes a new instance of the <see cref="HotKey"/> class.
      /// </summary>
      /// <param name="key">The key.</param>
      /// <param name="modifiers">The key modifiers.</param>
      public HotKey(Key key, ModifierKeys modifiers) {
         Key = key;
         Modifiers = modifiers;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="HotKey"/> class.
      /// </summary>
      public HotKey() {
      }

      public int ID { get; set; }

      /// <summary>
      /// Gets or sets the key.
      /// </summary>
      /// <value>
      /// The key.
      /// </value>
      public Key Key { get; }

      /// <summary>
      /// Gets or sets the key modifiers.
      /// </summary>
      /// <value>
      /// The key modifiers.
      /// </value>
      public ModifierKeys Modifiers { get; }

      public override string ToString() {
         return ConstructHotkeyText(this);
      }

      public static string ConstructHotkeyText(Key key, ModifierKeys mods) {
         var sb = new StringBuilder(32);
         if ((mods & ModifierKeys.Control) != 0) {
            sb.Append("Ctrl+");
         }
         if ((mods & ModifierKeys.Shift) != 0) {
            sb.Append("Shift+");
         }
         if ((mods & ModifierKeys.Alt) != 0) {
            sb.Append("Alt+");
         }
         if ((mods & ModifierKeys.Windows) != 0) {
            sb.Append("Win+");
         }

         sb.Append(key.ToString());

         return sb.ToString();
      }

      public static string ConstructHotkeyText(HotKey hotkey) {
         return ConstructHotkeyText(hotkey.Key, hotkey.Modifiers);
      }

      #region Equality members

      /// <summary>
      /// Determines whether the specified <see cref="HotKey"/> is equal to this instance.
      /// </summary>
      /// <param name="other">The <see cref="HotKey"/> to compare with this instance.</param>
      /// <returns>
      /// <c>true</c> if the specified <see cref="HotKey"/> is equal to this instance; otherwise, <c>false</c>.
      /// </returns>
      public bool Equals(HotKey other) {
         if (ReferenceEquals(null, other))
            return false;

         if (ReferenceEquals(this, other))
            return true;

         return Equals(other.Key, Key) && Equals(other.Modifiers, Modifiers);
      }

      /// <summary>
      /// Determines whether the specified <see cref="object"/> is equal to this instance.
      /// </summary>
      /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
      /// <returns>
      /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
      /// </returns>
      public override bool Equals(object obj) {
         if (ReferenceEquals(null, obj))
            return false;

         if (ReferenceEquals(this, obj))
            return true;

         return obj.GetType() == typeof(HotKey) && Equals((HotKey)obj);
      }

      /// <summary>
      /// Returns a hash code for this instance.
      /// </summary>
      /// <returns>
      /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
      /// </returns>
      public override int GetHashCode() {
         unchecked {
            return (Key.GetHashCode() * 397) ^ Modifiers.GetHashCode();
         }
      }

      #endregion
   }
}