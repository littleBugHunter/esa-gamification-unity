/* A C# Component
 * <author>Paul Nasdalack</author>
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ImageAnnotation.Client.Requests
{
    public class PostRequestWrapper<T> : IDisposable
    {
        public delegate T GetResultDelegate(byte[] response);
        private UnityWebRequest request;
        private GetResultDelegate getResult;
        public PostRequestWrapper(string endpoint, string json, GetResultDelegate getResult = null)
        {
            var basePath = ServerConnection.Instance.BasePath;
            var path = basePath + endpoint;
            byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(json);
            this.request = new UnityWebRequest(path, "POST", new DownloadHandlerBuffer(), new UploadHandlerRaw(jsonBytes));
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler.contentType = "application/json";
            this.getResult = getResult;
            request.SendWebRequest();
        }
        public PostRequestWrapper(UnityWebRequest request, string json, GetResultDelegate getResult = null)
        {
            this.request = request;
            byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
			request.disposeDownloadHandlerOnDispose = true;
			request.SetRequestHeader("Content-Type", "application/json");
            this.getResult = getResult;
        }

        public bool isDone => request.isDone;

        public T GetResult()
        {
            if (!isDone)
                throw new Exception("Download is not yet finished!");
            return getResult(request.downloadHandler.data);
        }

        public void Dispose()
        {
            request.Dispose();
        }
    }
}
