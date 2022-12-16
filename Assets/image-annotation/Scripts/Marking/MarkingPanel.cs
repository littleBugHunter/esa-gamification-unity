/* A C# Component
 * <author>Paul Nasdalack</author>
 */
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImageAnnotation.Marking
{
    public class MarkingPanel : MonoBehaviour
    {
        #region Serialized 
        [SerializeField, Required]
        private CraterLogger _craterLogger;
        [SerializeField]
        private GameObject _markingObject;
        #endregion
        #region Private Variables
        public delegate void SubmitCratersDelegate(CraterEntry[] entries);
        SubmitCratersDelegate _submitCraters;
        #endregion

        #region Unity Functions

        #endregion
        #region Public Functions
        public void Open()
        {
            gameObject.SetActive(true);
            _craterLogger.TargetTexture.texture = null;
            if(_markingObject != null)
            {
                _markingObject.SetActive(false);
            }
        }

        public void Close()
        {
            gameObject.SetActive(false);
            _craterLogger.TargetTexture.texture = null;
            if (_markingObject != null)
            {
                _markingObject.SetActive(false);
            }
        }

        public void StartMarking(Texture2D texture, SubmitCratersDelegate submitCraters)
        {
            _craterLogger.TargetTexture.texture = texture;
            _craterLogger.Clear();
            _submitCraters = submitCraters;
            if (_markingObject != null)
            {
                _markingObject.SetActive(true);
            }

        }

        public void FinishMarking()
        {
            if(_submitCraters != null)
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
        #endregion

        #region Private Functions

        #endregion
    }
}
