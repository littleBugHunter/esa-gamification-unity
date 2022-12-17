/* Two finger Annotation Marker
 * Places new Craters at the Center point between two _fingers
 * <author>Paul Nasdalack</author>
 */

using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace ImageAnnotation.Marking
{
    [AddComponentMenu("Image Annotation/Marking/Two Finger Marker")]
    public class TwoFingerMarker : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField]
        private CraterLogger _craterLogger;

        [InfoBox("The Visuals will be spawned on and between the fingers to preview the placing action. Some of them will be scaled up and down, so make sure to read the Tooltips")]
        [SerializeField, Foldout("Visuals")]
        private RawImage _targetImage;
        [Tooltip("A Prefab that will be spawned around the Finger Positions")]
        [SerializeField, Foldout("Visuals")]
        private Transform _fingerVisualPrefab;
        [Tooltip("A Prefab that will be Between the Fingers. It should have a height of 1.0, as it will be scaled")]
        [SerializeField, Foldout("Visuals")]
        private Transform _lineVisualPrefab;
        [Tooltip("A Prefab that preview the crater Circle. It will appear at the center between the fingers and scale depending on the finger distance. That's why it should have a radius of 1.0 (width and height of 2.0)")]
        [SerializeField, Foldout("Visuals")]
        private Transform _centerVisualPrefab;

        [SerializeField, Foldout("Interaction")]
        [Range(0, 1)]
        private float _minFingerDistance = 0.1f;
        [SerializeField, Foldout("Interaction")]
        [Range(0,3)]
        private float _maxFingerDistance = 1.0f;
        [SerializeField, Foldout("Interaction")]
        private float _radiusPower = 2.0f;
        #endregion
        #region Private Variables
        private List<Finger> _fingers = new List<Finger>();

        private List<Transform> _fingerVisuals = new List<Transform>(2);
        private List<Transform> _lineVisuals = new List<Transform>(2);
        private Transform       _centerVisual;

        private CraterEntry _craterEntry = new CraterEntry(Vector2.zero, 0.0f);

        #endregion

        #region Unity Functions
        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            if(_craterLogger == null)
            {
                _craterLogger = FindObjectOfType<CraterLogger>();
            }
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp   += FingerUp;
        }

        private void OnDisable()
        {
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp   -= FingerUp;
            foreach (var fingerVisual in _fingerVisuals)
            {
                Destroy(fingerVisual.gameObject);
            }
            _fingerVisuals.Clear();
            foreach (var lineVisual in _lineVisuals)
            {
                Destroy(lineVisual.gameObject);
            }
            _lineVisuals.Clear();
            if (_centerVisual != null)
            {
                Destroy(_centerVisual.gameObject);
                _centerVisual = null;
            }
        }
        private void Update()
        {
            switch(_fingers.Count)
            {
                case 1:
                    UpdateOneFinger();
                    break;
                case 2:
                    UpdateCenterMarking();
                    break;
            }
            if(_fingers.Count != 2)
            {
                _craterLogger.ClearOverridePreview();
            }
        }
        #endregion
        #region Public Functions

        #endregion

        #region Event Handlers

        private void FingerDown(Finger finger)
        {
            // we only care about two _fingers
            if(_fingers.Count > 2)
                return;
            if(_fingers.Contains(finger))
                return;
            _fingers.Add(finger);
            if (_fingerVisualPrefab != null)
            {
                _fingerVisuals.Add(Instantiate(_fingerVisualPrefab, _targetImage.transform));
            }
            if(_fingers.Count == 2)
            {
                if(_lineVisualPrefab != null)
                {
                    _lineVisuals.Add(Instantiate(_lineVisualPrefab, _targetImage.transform));
                    _lineVisuals.Add(Instantiate(_lineVisualPrefab, _targetImage.transform));
                }
                if(_centerVisualPrefab != null)
                {
                    _centerVisual = Instantiate(_centerVisualPrefab, _targetImage.transform);
                }
            }
        }
        private void FingerUp(Finger finger)
        {
            var index = _fingers.IndexOf(finger);
            if (index >= 0)
            {
                if(_fingers.Count == 2)
                {
                    MarkCrater();
                }
                if(_fingers.Count == 1)
                {
                    if (_fingers[0].currentTouch.isTap)
                    {
                        DeleteCrater();
                    }
                }
                _fingers.RemoveAt(index);
                if (_fingerVisualPrefab != null)
                {
                    Destroy(_fingerVisuals[index].gameObject);
                    _fingerVisuals.RemoveAt(index);
                }
                if(_fingers.Count == 1)
                {
                    if (_lineVisualPrefab != null)
                    {
                        foreach(var lineVisual in _lineVisuals)
                        {
                            Destroy(lineVisual.gameObject);
                        }
                        _lineVisuals.Clear();
                    }
                    if (_centerVisualPrefab != null)
                    {
                        Destroy(_centerVisual.gameObject);
                        _centerVisual = null;
                    }
                }
            }
        }
        #endregion

        #region Private Functions

        private void UpdateOneFinger()
        {
            var scale = 1.0f / _targetImage.transform.localScale.x;
            var position = _targetImage.transform.InverseTransformPoint((_fingers[0].screenPosition));
            Transform visual = _fingerVisuals[0];
            visual.localPosition = position;
            visual.localScale = Vector2.one * scale;
        }
        private void UpdateCenterMarking()
        {
            var positions = new Vector2[] {
                _targetImage.transform.InverseTransformPoint((_fingers[0].screenPosition)),
                _targetImage.transform.InverseTransformPoint((_fingers[1].screenPosition)),
            };
            if (positions[0].magnitude == float.NaN || positions[1].magnitude == float.NaN)
                return;
            var direction = (positions[1] - positions[0]);
            var scale = 1.0f / _targetImage.transform.localScale.x;

            var distance = direction.magnitude;
            direction /= distance;

            var radius = Mathf.Pow(Mathf.Min((distance + _minFingerDistance) / (_maxFingerDistance), 1.0f), _radiusPower) * distance / 2.0f;
            Vector2 center = positions[0] + direction * distance / 2;

            var rotations = new Quaternion[] {
                Quaternion.Euler(0,0,Vector2.SignedAngle(Vector2.up, direction)),
                Quaternion.Euler(0,0,Vector2.SignedAngle(Vector2.up, direction)+180),
            };

            if (_fingerVisualPrefab != null)
            {
                for (int i = 0; i < _fingerVisuals.Count; i++)
                {
                    Transform visual = _fingerVisuals[i];
                    visual.localPosition = positions[i];
                    visual.localRotation = rotations[i];
                    visual.localScale = Vector2.one * scale;
                }
            }
            if (_lineVisualPrefab != null)
            {
                for (int i = 0; i < _fingers.Count; i++)
                {
                    Transform visual = _lineVisuals[i];
                    visual.localPosition = positions[i];
                    visual.localRotation = rotations[i];
                    visual.localScale = new Vector3(scale, distance / 2 - radius, scale);
                }
            }
            if (_centerVisual != null)
            {
                _centerVisual.localPosition = center;
                _centerVisual.localRotation = rotations[0];
                _centerVisual.localScale = Vector3.one * radius;
            }
            UpdatetCraterEntry(center, radius);
            _craterLogger.UpdateOverridingCraters(_craterEntry);
        }

        private void MarkCrater()
        {
            _craterLogger.LogCrater(_craterEntry.Clone());
        }

        private void DeleteCrater()
        {
            Vector2 pos = (Vector2)_targetImage.transform.InverseTransformPoint((_fingers[0].screenPosition)) + Vector2.one * 0.5f;
            pos.y = 1 - pos.y;
            pos.x *= _targetImage.texture.width;
            pos.y *= _targetImage.texture.height;
            _craterLogger.RemoveClosest(pos, _targetImage.texture.width * 0.1f);
        }


        private void UpdatetCraterEntry(Vector2 position, float radius)
        {
            Vector2 pos = position + Vector2.one * 0.5f;
            pos.y = 1 - pos.y;
            pos.x *= _targetImage.texture.width;
            pos.y *= _targetImage.texture.height;
            _craterEntry.Position = pos;
            _craterEntry.Radius = radius * _targetImage.texture.width * 2;
        }
        #endregion
    }
}
