using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Rest
{

    public class RestTemplate
    {
        public string baseUri { get; set; }

        public RestTemplate(string baseUri)
        {
            this.baseUri = baseUri;
        }

        private string createURI(string route) {
            return baseUri + route;
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return bytes;
        }

        public static string GetString(byte[] data)
        {
            if (data == null)
                return "";
            return Encoding.UTF8.GetString(data);
        }

        private UploadHandlerRaw createUploaderHandler(UnityWebRequest uwr, byte[] data) {
            UploadHandlerRaw uH = new UploadHandlerRaw(data);
            uH.contentType = "application/json";
            uwr.uploadHandler = uH;

            return uH;
        }

        private DownloadHandler createDownloadHandler(UnityWebRequest uwr) {
            return uwr.downloadHandler;
        }

        private void passResult(UnityWebRequest webRequest, Action<byte[]> succeed = null, Action<string> failed = null) {
            if (webRequest.isNetworkError)
            {
                if (failed != null)
                    failed(webRequest.error);
            }
            else {
                if (succeed != null) {
                    succeed(webRequest.downloadHandler.data);
                }
            }
        }

        public IEnumerator Get(string route, Action<byte[]> succeed = null, Action<string> failed = null) {
            UnityWebRequest wb = UnityWebRequest.Get(createURI(route));
            createDownloadHandler(wb);

            yield return wb.Send();

            passResult(wb, succeed, failed);
        }

        public IEnumerator Post(string route, string data, Action<byte[]> succeed=null, Action<string> failed = null)
        {
            UnityWebRequest wb = UnityWebRequest.Post(createURI(route), data);
            createDownloadHandler(wb);

            yield return wb.Send();

            passResult(wb, succeed, failed);
        }
    }
}
