/* A Marker that just marks Craters at the Center of the UI. Should be used in Conjunction with MapMovement
 * <author>Paul Nasdalack</author>
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ImageAnnotation.Marking
{
	/// <summary>
	/// A Marker that just marks Craters at the Center of the UI. Should be used in Conjunction with MapMovement
	/// </summary>
	[AddComponentMenu("Image Annotation/Marking/Center Marker")]
    public class CenterMarker : MonoBehaviour
    {
        [SerializeField]
        float _radius = 100;

        [SerializeField]
        RawImage _targetImage;
        [SerializeField]
        CraterLogger _craterLogger;

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            for(float r = 0; r < Mathf.PI*2; r+=Mathf.PI*2/32) {
                Gizmos.DrawLine(
                    new Vector3(Mathf.Sin(r), Mathf.Cos(r)) * _radius,
                    new Vector3(Mathf.Sin(r+ Mathf.PI * 2 / 32), Mathf.Cos(r+ Mathf.PI * 2 / 32)) * _radius);
            }
            Gizmos.matrix = Matrix4x4.identity;
        }
        /// <summary>
        /// This Function must be called from User Code (e.g. when pressing a Button) to log a crater in the CraterLogger
        /// </summary>
        public void Submit() {
            Vector2 pos = (Vector2)_targetImage.transform.InverseTransformPoint(transform.position) + Vector2.one * 0.5f;
            pos.y = 1-pos.y;
            pos.x *= _targetImage.texture.width;
            pos.y *= _targetImage.texture.height;
            var radius = _radius / _targetImage.transform.localScale.x * _targetImage.texture.width * 2;
            _craterLogger.LogCrater(pos, radius);
        }
    }
}
