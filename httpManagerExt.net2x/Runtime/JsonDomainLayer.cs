using System;
using Newtonsoft.Json;

namespace Http.ManagerExt.Runtime
{
    public class JsonDomainLayer : IDomainLayer
    {
        public virtual IResult ParseError(Error e, string content)
        {
            return e;
        }

        public virtual IResult ParseSuccess<T>(string content)
        {
            T resp;
            try
            {
                resp = JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                return new Error(-2, ex);
            }

            if (resp == null) return new Error(-2, new Exception(content));
            return new Success<T>(resp);
        }
    }
}