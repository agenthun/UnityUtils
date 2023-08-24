using System;
using System.Threading.Tasks;
using Http.Manager.Runtime;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace Http.ManagerExt.Runtime
{
    #region UnityWebRequest Ext

    public static class HttpManagerExt
    {
        public static Lazy<IDomainLayer> DefaultDomainLayer = new Lazy<IDomainLayer>(() => new JsonDomainLayer());

        public static async Task<IResult> Convert<T>(
            this Task<UnityWebRequest> task,
            IDomainLayer domainLayer = null)
        {
            var request = await task;
            return request.Convert<T>(domainLayer);
        }

        public static async Task<T> ConvertSuccessOr<T>(
            this Task<UnityWebRequest> task,
            IDomainLayer domainLayer = null,
            T fallback = default)
        {
            var request = await task;
            return request.ConvertSuccessOr(domainLayer, fallback);
        }

        public static IResult Convert<T>(
            this UnityWebRequest request,
            IDomainLayer domainLayer = null)
        {
            var domain = domainLayer ?? DefaultDomainLayer.Value;
            try
            {
                var body = request.ResponseContent();
                return request.result == UnityWebRequest.Result.Success
                    ? domain.ParseSuccess<T>(body)
                    : domain.ParseError(new Error(request.responseCode, new Exception(request.error)), body);
            }
            catch (Exception e)
            {
                return new Error(-3, new Exception("request canceled"));
            }
        }

        public static T ConvertSuccessOr<T>(
            this UnityWebRequest request,
            IDomainLayer domainLayer = null,
            T fallback = default)
        {
            return request.Convert<T>(domainLayer: domainLayer).SuccessOr(fallback);
        }
    }

    #endregion

    #region Result

    public interface IResult
    {
    }

    public sealed class Success : IResult
    {
        // dynamic need at least net.4x
        public dynamic Data { get; }

        public Success(dynamic data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return $"{nameof(Data)}: {Data}";
        }
    }

    public sealed class Error : IResult
    {
        public string Code { get; }
        public Exception E;

        public Error(string code, Exception e)
        {
            Code = code;
            E = e;
        }

        public Error(long code, Exception e)
        {
            Code = code.ToString();
            E = e;
        }

        public override string ToString()
        {
            return $"{nameof(Code)}: {Code}, {nameof(E)}: {E}";
        }
    }

    public static class ResultExt
    {
        [CanBeNull]
        public static dynamic SuccessOr(this IResult result, dynamic fallback = default)
        {
            if (result is Success success) return success.Data;
            return fallback;
        }

        [CanBeNull]
        public static Error Error(this IResult result)
        {
            if (result is Error error) return error;
            return null;
        }

        public static bool IsError(this IResult result)
        {
            return result is Error;
        }
    }

    #endregion

    #region Domain process logic code

    public interface IDomainLayer
    {
        public IResult ParseError(Error e, string content);
        public IResult ParseSuccess<T>(string content);
    }

    #endregion
}