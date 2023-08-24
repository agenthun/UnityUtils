using System;
using Kv.Runtime;

namespace Sample
{
    public class Kv
    {
        private static Lazy<IKv> _debug = new Lazy<IKv>(() => new LocalKvImpl("debug", true));
        public static IKv Debug => _debug.Value;
    }
}