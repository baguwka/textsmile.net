namespace textsmile.net.Model {
   public interface IDataProvider {
      string Read(string key);
      void Write(string key, string data);
   }
}
