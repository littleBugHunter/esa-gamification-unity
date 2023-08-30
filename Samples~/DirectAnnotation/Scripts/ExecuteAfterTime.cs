using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ImageAnnotation.Samples.DirectAnnotation
{
    public class ExecuteAfterTime : MonoBehaviour
    {
        [SerializeField]
        float _waitTime;
        [SerializeField]
        UnityEvent _event;
        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(WaitAndInvoke(_waitTime));
        }

        private IEnumerator WaitAndInvoke(float time)
        {
            yield return new WaitForSeconds(time);
            _event.Invoke();
        }
    }
}
