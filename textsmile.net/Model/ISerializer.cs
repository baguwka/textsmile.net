namespace textsmile.net.Model {
   public interface ISerializer {
      string Serialize<T>(T data) where T : class;
      T Deserialize<T>(string serialized) where T : class;
   }
}
