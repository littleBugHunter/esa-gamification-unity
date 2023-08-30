using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ImageAnnotation.Samples.DirectAnnotation
{
    [RequireComponent(typeof(StateGroup))]
    public class StateSwitcher : MonoBehaviour
    {
        #region Serialised Fields
        [SerializeField]
        private Animator _transitionAnimator;
        [SerializeField, AnimatorParam("_transitionAnimator", AnimatorControllerParameterType.Bool)]
        private int _transitionParameter;
        [SerializeField]
        private float _waitTime;
        [SerializeField]
        private UnityEvent _onTransition;
        #endregion

        #region Private Variables
        private StateGroup _stateGroup;
        private Coroutine _transitionCoroutine;
        #endregion

        #region Unity Functions
        private void Awake()
        {
            _stateGroup = GetComponent<StateGroup>();
        }
        #endregion

        #region Public Function
        public void SwitchToState(string name)
        {
            if(_stateGroup.m_states.Contains(name))
            {
                if(_transitionCoroutine != null)
                {
                    StopCoroutine(_transitionCoroutine);
                }
                _transitionCoroutine = StartCoroutine(TransitionTo(name));
            } else
            {
                throw new KeyNotFoundException($"Unable to find state {name} on StateGroup {_stateGroup.gameObject.name}");
            }
        }
        #endregion

        #region Private Functions
        private IEnumerator TransitionTo(string name)
        {
            _onTransition.Invoke();
            _transitionAnimator.SetBool(_transitionParameter, true);
            yield return new WaitForSeconds(_waitTime);
            _stateGroup.SelectState(name);
            _transitionAnimator.SetBool(_transitionParameter, false);
        }

        #endregion
    }
}
