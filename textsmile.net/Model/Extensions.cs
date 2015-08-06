using System;

namespace textsmile.net.Model {
   public static class Extensions {
      public static TOut With<TIn, TOut>(this TIn self, Func<TIn, TOut> f, TOut onFail = default(TOut))
         where TIn : class{

         return self == null ? onFail : f(self);
      }
   }
}