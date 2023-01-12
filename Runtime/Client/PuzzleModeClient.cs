/* The Client for Puzzle Mode
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
using UnityEngine.Serialization;

namespace ImageAnnotation.Client
{
    /// <summary>
    /// The PuzzleModeClient Manages the Execution of Puzzle Mode Marking.
    /// Puzzle Mode Marking presents the Player with an Array of small known and unknown Images they have to mark in succession.
    /// After all Images have been marked, they are sent off to the server, which then evaluates the known images to send back a Score
    /// </summary>
    public class PuzzleModeClient : MonoBehaviour
    {
		#region Serialized Fields
		/// <summary>
		/// How many Images will be shown to the player?
        /// Keep this Value Reasonable high. Remember there need to be known images mixed in.
		/// </summary>
        [SerializeField, FormerlySerializedAs("_puzzleSize"), InfoBox("How many Images will be shown to the player?\nKeep this Value Reasonable high. Remember there need to be known images mixed in.")]
		public int PuzzleSize = 5;
        /// <summary>
        /// A Reference to the Marking Panel Instance, where the individual Images will be shown and marked
        /// </summary>
        [SerializeField, FormerlySerializedAs("_markingPanel"), Required]
        public MarkingPanel MarkingPanel;
        [Serializable]
        public class PuzzleDoneEvent : UnityEvent<PuzzleScore>
        { }
		/// <summary>
		/// This Event will be invoked once all puzzles have been completed and a score was calculated.
		/// A <c>PuzzleScore</c> Object is sent as a parameter, to retrieve the score calculated by the Server.
		/// </summary>
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

        /// <summary>
        /// A Score Object holding the score data calculated by the server.
        /// It is sent via the <c>OnPuzzleDone</c> event
        /// </summary>
        [Serializable]
        public struct PuzzleScore
        {
            [Serializable]
            public struct AbsoluteResults
            {
                /// <summary>
                /// How many craters were in all known images
                /// </summary>
                int totalPossible;
                /// <summary>
                /// How many craters were correctly marked
                /// </summary>
                int totalHit;
                /// <summary>
                /// How many craters were left unmarked
                /// </summary>
				int totalMissed;
				/// <summary>
				/// How many marks were created in places where no crater was present
				/// </summary>
				int totalFalse;
            }
            /// <summary>
            /// Information about the absolute numbers of craters marked in all <b>known</b> images.
            /// It is not advised to show these numbers to the players.
            /// </summary>
            public AbsoluteResults absolute;
            [Serializable]
            public struct RelativeResults
            {
                /// <summary>
                /// Percentage (0-1) of craters marked vs the total amount of craters present in the image
                /// </summary>
                float hit;
				/// <summary>
				/// Percentage (0-1) of craters that were left unmarked
				/// </summary>
				float missed;
				/// <summary>
				/// Percentage (0-1) of marks placed without a crater being present
				/// </summary>
				float invalid;
            }
			/// <summary>
			/// Information about the relative numbers of craters marked in all <b>known</b> images.
			/// </summary>
			public RelativeResults relative;
            /// <summary>
            /// An accuracy score (0-1) calculated by the server. We should reward very high accuracy.
            /// </summary>
            public float accuracy;
        }
        #endregion
        #region Unity Functions
        #endregion
        #region Public Functions
        /// <summary>
        /// Requests a Puzzle List from the Server and opens the Puzzle Panel for marking. OnPuzzleDone will be called once marking of all puzzles is complete and the server responded with a score.
        /// </summary>
        public void StartPuzzle()
        {
            StartCoroutine(PuzzleCoroutine());
        }
    	#endregion

    	#region Private Functions
        IEnumerator PuzzleCoroutine()
        {
            MarkingPanel.Open();
            var puzzleRequest = new GetObjectJson<Puzzle>("puzzle", new[] {"user="+ServerConnection.Instance.UserName, "amount="+ PuzzleSize.ToString()});
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
                MarkingPanel.StartMarking(texture, (entries) =>
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
            MarkingPanel.Close();
            OnPuzzleDone.Invoke(score);
        }
    	#endregion
    }
}
