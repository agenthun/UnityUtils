using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Kv.Runtime
{
    public class LocalKvImpl : IKv
    {
        private string _name;
        private bool _debug;

        private static string LocalDir
        {
            get
            {
                var dir = Application.streamingAssetsPath;
#if UNITY_ANDROID && !UNITY_EDITOR
                dir = Application.persistentDataPath;
#endif
                return dir;
            }
        }

        [CanBeNull] private JObject _kv;

        private bool ForceUpdateKv
        {
            get
            {
                if (_kv == null) return false;
                var forceUpdateKv = _kv.TryGetValue("forceUpdateKv", out var value) ? value : false;
                return bool.TryParse(forceUpdateKv.ToString(), out var result) && result;
            }
        }

        public LocalKvImpl(string name, bool debug = false)
        {
            _name = name;
            _debug = debug;
            RefreshKv();
        }

        private void RefreshKv()
        {
            var file = new FileInfo($"{LocalDir}/{_name}.json");
            if (!file.Exists)
            {
                _kv = null;
                return;
            }

            try
            {
                var result = File.ReadAllText(file.FullName);
                _kv = JObject.Parse(result);
            }
            catch (Exception e)
            {
                Debug.LogError($"LocalKvImpl.RefreshKv.error, {e}");
                _kv = null;
            }
        }

        public object GetObjectValue(string key)
        {
            if (_kv == null)
            {
                if (_debug) Debug.Log($"LocalKvImpl.GetObjectValue({key})={null}");
                return null;
            }

            if (ForceUpdateKv) RefreshKv();
            var v = _kv.TryGetValue(key, out var value) ? value : null;
            if (_debug) Debug.Log($"LocalKvImpl.GetObjectValue({key})={v}");
            return v;
        }

        public string GetString(string key, string defaultValue = null)
        {
            var obj = GetObjectValue(key)?.ToString();
            return obj ?? defaultValue;
        }

        public T DeserializeObject<T>(string key, T defaultValue)
        {
            var json = GetString(key, null);
            if (string.IsNullOrEmpty(json)) return defaultValue;
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"LocalKvImpl.DeserializeObject.error, {e}");
            }

            return defaultValue;
        }

        public int GetInt(string key, int defaultValue)
        {
            var obj = GetObjectValue(key);
            if (obj == null) return defaultValue;
            return int.TryParse(obj.ToString(), out var result) ? result : defaultValue;
        }

        public long GetLong(string key, long defaultValue)
        {
            var obj = GetObjectValue(key);
            if (obj == null) return defaultValue;
            return long.TryParse(obj.ToString(), out var result) ? result : defaultValue;
        }

        public float GetFloat(string key, float defaultValue)
        {
            var obj = GetObjectValue(key);
            if (obj == null) return defaultValue;
            return float.TryParse(obj.ToString(), out var result) ? result : defaultValue;
        }

        public double GetDouble(string key, double defaultValue)
        {
            var obj = GetObjectValue(key);
            if (obj == null) return defaultValue;
            return double.TryParse(obj.ToString(), out var result) ? result : defaultValue;
        }

        public char GetChar(string key, char defaultValue)
        {
            var obj = GetObjectValue(key);
            if (obj == null) return defaultValue;
            return char.TryParse(obj.ToString(), out var result) ? result : defaultValue;
        }

        public bool GetBool(string key, bool defaultValue)
        {
            var obj = GetObjectValue(key);
            if (obj == null) return defaultValue;
            return bool.TryParse(obj.ToString(), out var result) ? result : defaultValue;
        }
    }
}