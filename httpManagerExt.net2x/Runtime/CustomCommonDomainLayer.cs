using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Http.ManagerExt.Runtime
{
    public class CustomCommonDomainLayer : IDomainLayer
    {
        public virtual IResult ParseError(Error e, string content)
        {
            JObject json;
            try
            {
                json = JObject.Parse(content);
            }
            catch (Exception ex)
            {
                return new Error(-2, ex);
            }

            var status = json["status"]?.ToString();
            var msg = json["message"]?.ToString();
            var error = json["error"];
            var statusErrorType = !string.IsNullOrEmpty(status) || !string.IsNullOrEmpty(msg);
            var bodyErrorType = error != null;
            if (statusErrorType || bodyErrorType)
            {
                if (statusErrorType)
                {
                    e.E = new Exception($"[{status}], {msg}");
                }
                else if (bodyErrorType)
                {
                    var errorType = json["error"]["type"]?.ToString();
                    var errorMessage = json["error"]["message"]?.ToString();
                    e.E = new Exception($"[{errorType}], {errorMessage}");
                }
            }

            return e;
        }

        public virtual IResult ParseSuccess<T>(string content)
        {
            JObject json;
            try
            {
                json = JObject.Parse(content);
            }
            catch (Exception ex)
            {
                return new Error(-2, ex);
            }

            if (json["status"]?.ToString() != "SUCCESS") return new Error(-2, new Exception(content));
            var dataString = json["data"]?.ToString();
            if (string.IsNullOrEmpty(dataString)) return new Success<T>(default);
            try
            {
                var data = JsonConvert.DeserializeObject<T>(dataString);
                return new Success<T>(data);
            }
            catch (Exception e)
            {
                return new Error(-2, e);
            }
        }
    }
}