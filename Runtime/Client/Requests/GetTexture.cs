/* A C# Component
 * <author>Paul Nasdalack</author>
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ImageAnnotation.Client.Requests
{
    public class GetTexture : GetRequestWrapper<Texture2D>
    {
        public GetTexture(string endpoint) : base(endpoint, Deserialize)
        { }


        public GetTexture(string endpoint, string[] parameters) : base(endpoint, parameters, Deserialize)
        { }

        public GetTexture(UnityWebRequest request) : base(request, Deserialize)
        { }

        private static Texture2D Deserialize(byte[] data)
        {
            var tex2d = new Texture2D(1, 1);
            tex2d.LoadImage(data);
            return tex2d;
        }
    }
}
