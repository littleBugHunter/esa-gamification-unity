using System;
using UnityEngine;
using UnityEngine.Networking;

namespace ImageAnnotation
{
    public static class ImageLoader 
    {
        [Serializable]
        public struct PexelsResponse
        {
            [Serializable]
            public struct Photo
            {
                public int id;
                public int width;
                public int height;
                public string url;
                [Serializable]
                public struct Source
                {
                    public string original;
                }
                public Source src;
            }
            public Photo[] photos;
        }

        public class RequestWrapper<T> : IDisposable
        {
            public delegate T GetResultDelegate(byte[] response);
            private UnityWebRequest request;
            private GetResultDelegate getResult;
            public RequestWrapper(UnityWebRequest request, GetResultDelegate getResult)
            {
                this.request = request;
                this.getResult = getResult;
            }

            public bool isDone => request.isDone;

            public T GetResult()
            {
                if(!isDone)
                    throw new Exception("Download is not yet finished!");
                return getResult(request.downloadHandler.data);
            }

            public void Dispose()
            {
                request.Dispose();
            }
        }

        [Serializable]
        public struct PuzzleList
        {
            [Serializable]
            public struct Puzzle
            {
                public int id;
                public string image;
            }
            public Puzzle[] puzzles;
        }

        public static RequestWrapper<PuzzleList> GetPuzzle()
        {
            var req = UnityWebRequest.Get("localhost:3000/puzzle");
            var wrapper = new RequestWrapper<PuzzleList>(req, (data) =>
            {
                string jsonResponse = System.Text.Encoding.Default.GetString(data);
                Debug.Log(jsonResponse);
                return JsonUtility.FromJson<PuzzleList>(jsonResponse);
            });
            req.SendWebRequest();
            return wrapper;
        }
        
        
        public static RequestWrapper<PexelsResponse> QueryImages()
        {
            var req = UnityWebRequest.Get("https://api.pexels.com/v1/search?query=donut&per_page=15");
            req.SetRequestHeader("Authorization", "563492ad6f91700001000001ad75f02d615a4c1999d50d50e12f804c");
            var wrapper =  new RequestWrapper<PexelsResponse>(req, (data) =>
            {
                string jsonResponse = System.Text.Encoding.Default.GetString(data);
                return JsonUtility.FromJson<PexelsResponse>(jsonResponse);
            });
            req.SendWebRequest();
            return wrapper;
        }
        public static RequestWrapper<Texture2D> LoadImage(string url)
        {
            var req = UnityWebRequest.Get(url);
            var wrapper = new RequestWrapper<Texture2D>(req, (data) =>
            {
                var tex2d = new Texture2D(1,1);
                tex2d.LoadImage(data);
                return tex2d;
            });
            req.SendWebRequest();
            return wrapper;
        }
    }

}