/* A C# Component
 * <author>Paul Nasdalack</author>
 */
using ImageAnnotation.Client.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ImageAnnotation.Client.Requests
{
    public class PostObjectJson<T> : PostRequestWrapper<T>
    {
        public PostObjectJson(string endpoint, string json) : base(endpoint, json, Deserialize)
        { }

        public PostObjectJson(UnityWebRequest request, string json) : base(request, json, Deserialize)
        { }

        private static T Deserialize(byte[] data)
        {
            string jsonResponse = System.Text.Encoding.Default.GetString(data);
            return JsonUtility.FromJson<T>(jsonResponse);
        }
    }
}
