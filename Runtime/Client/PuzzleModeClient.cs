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

namespace ImageAnnotation.Client
{
    public class PuzzleModeClient : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField]
        int _puzzleSize = 5;
        [SerializeField, Required]
        private MarkingPanel _markingPanel;
        [Serializable]
        public class PuzzleDoneEvent : UnityEvent<PuzzleScore>
        { }
        [SerializeField]
        public PuzzleDoneEvent OnPuzzleDone = new PuzzleDoneEvent();
        #endregion
        #region Private Variables

        #endregion
        #region Structs
        [Serializable]
        public struct Puzzle
        {
            [Serializable]
            public struct Slice
            {
                public int id;
                public string image;
            }
            public Slice[] slices;
        }
        [Serializable]
        public struct SolvedPuzzle
        {
            [Serializable]
            public struct Slice
            {
                public int id;
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
            public List<Slice> slices;
            public string user;
        }

        [Serializable]
        public struct PuzzleScore
        {
            [Serializable]
            public struct AbsoluteResults
            {
                int totalPossible;
                int totalHit;
				int totalMissed;
				int totalFalse;
            }
            public AbsoluteResults absolute;
            [Serializable]
            public struct RelativeResults
            {
                float hit;
				float missed;
				float invalid;
            }
            public RelativeResults relative;
            public float accuracy;
        }
        #endregion
        #region Unity Functions
        #endregion
        #region Public Functions
        public void StartPuzzle()
        {
            StartCoroutine(PuzzleCoroutine());
        }
    	#endregion

    	#region Private Functions
        IEnumerator PuzzleCoroutine()
        {
            _markingPanel.Open();
            var puzzleRequest = new GetObjectJson<Puzzle>("puzzle", new[] {"user="+ServerConnection.Instance.UserName, "amount="+ _puzzleSize.ToString()});
            yield return new WaitUntil(() => puzzleRequest.isDone);
            var puzzle = puzzleRequest.GetResult();
            var solved = new SolvedPuzzle();
            solved.user = ServerConnection.Instance.UserName;
            solved.slices = new List<SolvedPuzzle.Slice>();
            foreach(var slice in puzzle.slices)
            {
                if(solved.slices.FindIndex((s) => slice.id == s.id) > 0)
                {
                    continue;
                }
                var imageRequest = new GetTexture("slices/" + slice.image);
                yield return new WaitUntil(() => imageRequest.isDone);
                var texture = imageRequest.GetResult();
                var sliceDone = false;
                _markingPanel.StartMarking(texture, (entries) =>
                {
                    var solvedSlice = new SolvedPuzzle.Slice();
                    solvedSlice.image = slice.image;
                    solvedSlice.id = slice.id;
                    var circles = new SolvedPuzzle.Slice.Circle[entries.Length];
                    for (int i = 0; i < entries.Length; i++)
                    {
                        CraterEntry entry = entries[i];
                        circles[i].x = entry.Position.x;
                        circles[i].y = entry.Position.y;
                        circles[i].radius = entry.Radius;
                    }
                    solvedSlice.circles = circles;
                    solved.slices.Add(solvedSlice);
                    sliceDone = true;
                });
                yield return new WaitUntil(() => sliceDone);
            }
            var solvedJson = JsonUtility.ToJson(solved);
            var scoreRequest = new PostObjectJson<PuzzleScore>("submit/", solvedJson);
            yield return new WaitUntil(() => scoreRequest.isDone);
            var score = scoreRequest.GetResult();
            _markingPanel.Close();
            OnPuzzleDone.Invoke(score);
        }
    	#endregion
    }
}
