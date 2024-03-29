/* A UI Component for a Panel that is used for Marking the Craters
 * <author>Paul Nasdalack</author>
 */
using ImageAnnotation.Client;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImageAnnotation.Marking
{
    /// <summary>
    /// A UI Component that is used for marking Images.
    /// </summary>
    public class MarkingPanel : MonoBehaviour
    {
        #region Serialized
        [SerializeField, Required]
        private CraterLogger _craterLogger;
        [SerializeField, InfoBox("Reference to a GameObject with Components that handle Marking (e.g. TwoFingerMarker)")]
        private GameObject _markingObject;
        #endregion
        #region Private Variables
        public delegate void SubmitCratersDelegate(CraterEntry[] entries);
		SubmitCratersDelegate _submitCraters;
		public delegate void SkipImageDelegate(SkipReason reason);
		SkipImageDelegate _skipImage;
		#endregion

		#region Unity Functions

		#endregion
		#region Public Functions
		/// <summary>
		/// Shows the Panel and clears the Marking Texture
		/// </summary>
		public void Open()
        {
            gameObject.SetActive(true);
            _craterLogger.TargetTexture.texture = null;
            if(_markingObject != null)
            {
                _markingObject.SetActive(false);
            }
        }

		/// <summary>
		/// Closes the Panel
		/// </summary>
		public void Close()
        {
            gameObject.SetActive(false);
            _craterLogger.TargetTexture.texture = null;
            if (_markingObject != null)
            {
                _markingObject.SetActive(false);
            }
        }

		/// <summary>
		/// Shows the Marking Texture, Clears the Crater Logger and enables the Marking 
		/// </summary>
        /// <param name="texture">The Texture that will be shown for marking</param>
        /// <param name="submitCraters">A Delegate that will be called once the logging is complete in order to log all marked craters</param>
		public void StartMarking(Texture2D texture, SubmitCratersDelegate submitCraters, SkipImageDelegate skipImage = null)
        {
            _craterLogger.TargetTexture.texture = texture;
            _craterLogger.Clear();
			_submitCraters = submitCraters;
			_skipImage = skipImage;
			if (_markingObject != null)
            {
                _markingObject.SetActive(true);
            }

		}

		/// <summary>
		/// Hides the Marking Texture, Submits the Logged Craters and disables the Marking Object
		/// </summary>
		public void FinishMarking()
		{
			if (_submitCraters != null)
			{
				_submitCraters(_craterLogger.GetCraters().ToArray());
				_submitCraters = null;
			}
			_craterLogger.Clear();
			_craterLogger.TargetTexture.texture = null;
			if (_markingObject != null)
			{
				_markingObject.SetActive(false);
			}
		}

		/// <summary>
		/// Skips the currently shown image, given the reason
		/// </summary>
		/// <param name="reason">The Reason for the Skip. The Server will only accept values from this enum.</param>
		public void SkipImage(SkipReason reason)
		{
			if (_skipImage == null)
			{
				Debug.LogWarning("Cannot Skip! No skip delegate was passed in StartMarking()");
				return;
			}
			_skipImage(reason);
			_submitCraters = null;
			_craterLogger.Clear();
			_craterLogger.TargetTexture.texture = null;
			if (_markingObject != null)
			{
				_markingObject.SetActive(false);
			}
		}
		/// <summary>
		/// Skips the currently shown image, given the reason
		/// </summary>
		/// <param name="reasonString">The SkipReason as a string to work with UnityEvents Must be one of the <c>SkipReason</c> Enum Values</param>
		public void SkipImage(string reasonString)
		{
			var reason = Enum.Parse<SkipReason>(reasonString);
			SkipImage(reason);
		}
		#endregion

		#region Private Functions

		#endregion
	}
}
