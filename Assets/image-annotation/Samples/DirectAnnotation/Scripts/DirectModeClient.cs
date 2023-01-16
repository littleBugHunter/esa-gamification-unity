/* A C# Component
 * <author>Paul Nasdalack</author>
 */
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using ImageAnnotation.Client.Requests;
using ImageAnnotation.Marking;
using System.Collections.Generic;
using UnityEngine.Events;
using ImageAnnotation.Client;
using ImageAnnotation.Samples.DirectAnnotation;

namespace ImageAnnotation.Samples.DirectMode
{
    public class DirectModeClient : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField, Required]
        private MarkingPanel _markingPanel;
        [SerializeField]
        public UnityEvent OnPuzzleDone;
        [SerializeField]
        public UnityEvent OnPuzzleSkipped;
        [SerializeField]
        private StateGroup _stateGroup;
        [SerializeField]
        private StateSwitcher stateSwitcher;
        [SerializeField]
        private string _waitingState;
        [SerializeField]
        private string _markingState;
        #endregion
        #region Private Variables
        Coroutine currentCoroutine;
        #endregion
        #region Structs
        [Serializable]
        struct ImageRequest
        {
            public string image;
        }
        [Serializable]
        struct Submission
        {
            public string user;
            public string image;
            [Serializable]
            public struct Circle
            {
                public float x;
                public float y;
                public float radius;
            }
            public Circle[] circles;
        }
        #endregion
        #region Unity Functions
        #endregion
        #region Public Functions
        public void StartMarking()
        {
            if(currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(PuzzleCoroutine());
        }
        #endregion


        #region Private Functions
        IEnumerator PuzzleCoroutine()
        {
            _stateGroup.SelectState(_waitingState);
            var imageEntryRequest = new GetObjectJson<ImageRequest>("image", new[] {"user="+ServerConnection.Instance.UserName});
            yield return new WaitUntil(() => imageEntryRequest.isDone);
            var imageEntry = imageEntryRequest.GetResult();
            var submission = new Submission();
            submission.user = ServerConnection.Instance.UserName;
            submission.image = imageEntry.image;
            var imageRequest = new GetTexture("slices/" + imageEntry.image);
            yield return new WaitUntil(() => imageRequest.isDone);
            var texture = imageRequest.GetResult();
            var sliceDone = false;
            stateSwitcher.SwitchToState(_markingState);
            _markingPanel.StartMarking(texture, (entries) =>
            {
                var circles = new Submission.Circle[entries.Length];
                for (int i = 0; i < entries.Length; i++)
                {
                    CraterEntry entry = entries[i];
                    circles[i].x = entry.Position.x;
                    circles[i].y = entry.Position.y;
                    circles[i].radius = entry.Radius;
                }
                submission.circles = circles;
                sliceDone = true;
            });
            yield return new WaitUntil(() => sliceDone);
            _stateGroup.SelectState(_waitingState);
            var submissionJson = JsonUtility.ToJson(submission);
            var scoreRequest = new PostRequestWrapper<string>("direct-submit/", submissionJson);
            yield return new WaitUntil(() => scoreRequest.isDone);
            OnPuzzleDone.Invoke();
        }
        #endregion
    }
}
