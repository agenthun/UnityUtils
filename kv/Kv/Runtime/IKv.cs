using JetBrains.Annotations;

namespace Kv.Runtime
{
    public interface IKv
    {
        [CanBeNull]
        public object GetObjectValue(string key);

        [CanBeNull]
        public string GetString(string key, string defaultValue = null);

        [CanBeNull]
        public T DeserializeObject<T>(string key, T defaultValue);

        public int GetInt(string key, int defaultValue);
        public long GetLong(string key, long defaultValue);
        public float GetFloat(string key, float defaultValue);
        public double GetDouble(string key, double defaultValue);
        public char GetChar(string key, char defaultValue);
        public bool GetBool(string key, bool defaultValue);
    }
}