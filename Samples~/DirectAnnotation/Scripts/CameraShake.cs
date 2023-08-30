/* A C# Component
 * <author>Paul Nasdalack</author>
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ImageAnnotation.Samples.DirectAnnotation
{
    public class CameraShake : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField]
        Vector3 _translateIntensity;
        [SerializeField]
        float _translateSpeed = 0.5f;
        [SerializeField]
        Vector3 _rotationIntensity;
        [SerializeField]
        float rotationSpeed = 0.5f;
        [SerializeField]
        int octaves = 1;
        [SerializeField]
        float falloff = 2.0f;

        #endregion

        #region Unity Functions
        private void Update()
        {
            float translateTime = Time.time * _translateSpeed;
            float rotationTime = Time.time * rotationSpeed;
            transform.localPosition = Vector3.Scale(Perlin3(translateTime, octaves, falloff), _translateIntensity);
            transform.localEulerAngles = Vector3.Scale(Perlin3(rotationTime, octaves, falloff), _rotationIntensity);
        }
        #endregion

        #region Private Functions
        Vector3 Perlin3(float time, int octaves, float falloff)
        {
            Vector3 ret = new Vector3();
            for(int i = 0; i < octaves; ++i)
            {
                Vector3 p = new Vector3(Mathf.PerlinNoise(time*i, i*1), Mathf.PerlinNoise(time*i, i*3), Mathf.PerlinNoise(time*i, i*5));
                p *= 2.0f;
                p -= Vector3.one;
                p /= Mathf.Pow(falloff, i);
                ret += p;
            }
            return ret;
        }
        #endregion
    }
}
