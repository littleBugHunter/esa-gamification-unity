/* A C# Component
 * <author>Paul Nasdalack</author>
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

namespace ImageAnnotation.Marking.Visuals
{
    [AddComponentMenu("Image Annotation/Marking/Visual/Crater Visual")]
    public class CraterVisual : MonoBehaviour, IDeletableCraterVisual
    {
        #region Serialized Fields
        private enum Mode
        {
            Simple,
            SpriteSwap,
            Animator
        }
        [SerializeField]
        private Mode _mode;

        [SerializeField, ShowIf("_mode", Mode.SpriteSwap)]
        private Image _targetImage;
        [SerializeField, ShowIf("_mode", Mode.SpriteSwap)]
        private Sprite _normal;
        [SerializeField, ShowIf("_mode", Mode.SpriteSwap)]
        private Sprite _markedForDelete;


        [SerializeField, ShowIf("_mode", Mode.Animator)]
        private Animator _animator;
        [SerializeField, ShowIf("_mode", Mode.Animator), AnimatorParam("_animator", AnimatorControllerParameterType.Bool)]
        private int _markedForDeleteParameter;
        [SerializeField, ShowIf("_mode", Mode.Animator), AnimatorParam("_animator", AnimatorControllerParameterType.Trigger)]
        private int _deleteTriggerParameter;
        [SerializeField, ShowIf("_mode", Mode.Animator)]
        private float _deleteDelay = 1.0f;

        #endregion
        #region Private Variables

        #endregion

        #region Unity Functions
        private void Start()
        {
            if(_targetImage == null)
            {
                _targetImage = GetComponent<Image>();
            }
        }
        #endregion
        #region Public Functions
        public void SetPreviewDelete(bool delete)
        {
            switch(_mode)
            {
                case Mode.SpriteSwap:
                    _targetImage.sprite = delete ? _markedForDelete : _normal;
                    break;
                case Mode.Animator:
                    _animator.SetBool(_markedForDeleteParameter, delete);
                    break;
            }
        }
        public void Delete()
        {
            switch(_mode)
            {
                case Mode.Simple:
                case Mode.SpriteSwap:
                    Destroy(gameObject);
                    break;
                case Mode.Animator:
                    Destroy(gameObject, _deleteDelay);
                    break;
            }
        }
        #endregion

        #region Private Functions

        #endregion
    }
}
