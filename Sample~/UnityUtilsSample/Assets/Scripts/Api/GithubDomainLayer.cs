using System;
using Http.ManagerExt.Runtime;
using Newtonsoft.Json.Linq;

namespace Api
{
    public class GithubDomainLayer : JsonDomainLayer
    {
        public override IResult ParseError(Error e, string content)
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

            var msg = json["message"]?.ToString();
            var code = json["code"]?.ToString() ?? (e.Code ?? "-3");
            return new Error(code, new Exception(msg));
        }
    }
}