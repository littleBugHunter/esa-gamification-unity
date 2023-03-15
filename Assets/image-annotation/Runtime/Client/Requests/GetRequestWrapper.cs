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
    public class GetRequestWrapper<T> : IDisposable
    {
        public delegate T GetResultDelegate(byte[] response);
        private UnityWebRequest request;
        private GetResultDelegate getResult;
        public GetRequestWrapper(string endpoint, GetResultDelegate getResult)
        {
            var basePath = ServerConnection.Instance.BasePath;
            var path = basePath + endpoint;
            this.request = new UnityWebRequest(path, "GET");
            this.request.downloadHandler = new DownloadHandlerBuffer();
            this.request.disposeDownloadHandlerOnDispose = true;
            this.getResult = getResult;
            request.SendWebRequest();
        }
        public GetRequestWrapper(string endpoint, string[] parameters, GetResultDelegate getResult)
        {
            var basePath = ServerConnection.Instance.BasePath;
            var path = basePath + endpoint;
            if (parameters.Length > 0)
            {
                path += "?";
                for (int i = 0; i < parameters.Length; i++)
                {
                    string parameter = parameters[i];
                    path += parameter;
                    if(i < parameters.Length-1)
                    {
                        path += "&";
                    }
                }
            }
            this.request = new UnityWebRequest(path, "GET");
            this.request.downloadHandler = new DownloadHandlerBuffer();
			this.request.disposeDownloadHandlerOnDispose = true;
			this.getResult = getResult;
            request.SendWebRequest();
        }
        public GetRequestWrapper(UnityWebRequest request, GetResultDelegate getResult)
        {
            this.request = request;
            this.request.downloadHandler = new DownloadHandlerBuffer();
			this.request.disposeDownloadHandlerOnDispose = true;
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
