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
    public class GetObjectJson<T> : GetRequestWrapper<T>
    {
        public GetObjectJson(string endpoint) : base(endpoint, Deserialize)
        { }


        public GetObjectJson(string endpoint, string[] parameters) : base(endpoint, parameters, Deserialize)
        { }

        public GetObjectJson(UnityWebRequest request) : base(request, Deserialize)
        { }

        private static T Deserialize(byte[] data)
        {
            string jsonResponse = System.Text.Encoding.Default.GetString(data);
            return JsonUtility.FromJson<T>(jsonResponse);
        }
    }
}
