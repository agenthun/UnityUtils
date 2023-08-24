using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Data;
using Http.Manager.Runtime;
using Http.ManagerExt.Runtime;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace Api
{
    public class GithubApi : HttpManager.IHeaderProvider
    {
        [NotNull] private string _baseUrl = "https://api.github.com";
        private readonly GithubDomainLayer _githubDomainLayer = new GithubDomainLayer();

        private static GithubApi _instance = null;

        private GithubApi()
        {
            Init();
        }

        public static GithubApi Instance => _instance ??= new GithubApi();

        #region common request params

        private void Init()
        {
            HttpManager.Instance.SetCommonHeaderProvider(this);
            HttpManager.Instance.TimeoutSeconds = 10;
        }

        private string getBaseUrl(string api) => $"{_baseUrl}{api}";

        public Dictionary<string, object> ProvideHeaders(UnityWebRequest request)
        {
            var headers = new Dictionary<string, object>();
            headers.Add("Accept", "application/json");
            headers.Add("Content-Type", "application/json");
            return headers;
        }

        #endregion

        #region api

        public async Task<IResult> contributors(string owner, string repo)
        {
            var url = getBaseUrl($"/repos/{owner}/{repo}/contributors");
            var request = UnityWebRequest.Get(url);
            var result = await HttpManager.Instance.RequestAsync(request)
                .Convert<List<GithubContributor>>(_githubDomainLayer);
            Debug.Log($"contributors, result={result}");
            return result;
        }

        #endregion

        public void CancelAll()
        {
            HttpManager.Instance.CancelAll();
        }
    }
}