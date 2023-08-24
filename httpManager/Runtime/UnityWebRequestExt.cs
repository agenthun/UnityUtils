using System;
using System.Text;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace Http.Manager.Runtime
{
    #region UnityWebRequest Ext

    public static class UnityWebRequestExt
    {
        #region POST

        public static UnityWebRequest PostJson(string uri, string json)
        {
            UnityWebRequest request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
            SetupPost(request, json);
            return request;
        }

        private static void SetupPost(UnityWebRequest request, string json)
        {
            byte[] data = (byte[]) null;
            if (!string.IsNullOrEmpty(json))
                data = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(data);
            request.uploadHandler.contentType = "application/json";
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        }

        #endregion

        [CanBeNull]
        public static string ResponseContent(this UnityWebRequest request)
        {
            string result = null;
            try
            {
                result = request.downloadHandler?.text;
            }
            catch (Exception e)
            {
            }

            return result;
        }
    }

    #endregion
}