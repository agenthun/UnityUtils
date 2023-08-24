using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace Http.Manager.Runtime
{
    public class HttpManager
    {
        private static HttpManager _instance = null;
        private List<UnityWebRequestAsyncOperation> _requests = new List<UnityWebRequestAsyncOperation>();

        public bool ShowLog = true;
        public int TimeoutSeconds = 20;

        public interface IHeaderProvider
        {
            [CanBeNull]
            public Dictionary<string, object> ProvideHeaders(UnityWebRequest request);
        }

        [CanBeNull] private IHeaderProvider _commonHeaderProvider;

        private HttpManager()
        {
        }

        public static HttpManager Instance => _instance ??= new HttpManager();

        public void SetCommonHeaderProvider(IHeaderProvider headerProvider)
        {
            _commonHeaderProvider = headerProvider;
        }

        public UnityWebRequestAsyncOperation Request(UnityWebRequest request)
        {
            Log($"HttpManager.Request, [{request.method}], {request.url}");
            request.certificateHandler = new WebRequestCertificate();

            var headers = _commonHeaderProvider?.ProvideHeaders(request);
            if (headers != null)
            {
                foreach (var keyValuePair in headers)
                {
                    request.SetRequestHeader(keyValuePair.Key, keyValuePair.Value.ToString());
                }
            }

            request.timeout = TimeoutSeconds;
            var operation = request.SendWebRequest();
            operation.completed += (it) =>
            {
                try
                {
                    Log($"HttpManager.Response, [{request.method}], {request.url}\n{request.ResponseContent()}");
                }
                catch (Exception e)
                {
                }

                if (_requests.Contains(operation))
                {
                    _requests.Remove(operation);
                }
            };
            return operation;
        }

        internal class WebRequestCertificate : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }

        public async Task<UnityWebRequest> RequestAsync(UnityWebRequest request)
        {
            var req = Request(request);
            _requests.Add(req);
            try
            {
                await req.ToUniTask();
            }
            catch (Exception e)
            {
            }

            return request;
        }

        public void Cancel(UnityWebRequestAsyncOperation operation)
        {
            try
            {
                var r = operation.webRequest;
                r.Abort();
                r.Dispose();
                if (_requests.Contains(operation))
                {
                    _requests.Remove(operation);
                }
            }
            catch (Exception e)
            {
            }
        }

        public void CancelAll()
        {
            for (var i = _requests.Count - 1; i >= 0; i--)
            {
                Cancel(_requests[i]);
            }
        }

        private void Log(string msg)
        {
            if (ShowLog) Debug.Log(msg);
        }
    }
}