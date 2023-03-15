/* Click Drag Marker
 * Places new Craters by Clicking the Center and Dragging outwards to create a radius
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
	/// <summary>
	/// Two finger Annotation Marker
	/// </summary>
	[AddComponentMenu("Image Annotation/Marking/Click Drag Marker")]
	public class ClickDragMarker : MonoBehaviour
	{
		#region Serialized Fields
		[SerializeField]
		private CraterLogger _craterLogger;

		[InfoBox("The Visuals will be spawned on and between the fingers to preview the placing action. Some of them will be scaled up and down, so make sure to read the Tooltips")]
		[SerializeField, Foldout("Visuals")]
		private RawImage _targetImage;
		[InfoBox("A Prefab that preview the crater Circle. It will appear at the center between the fingers and scale depending on the finger distance. That's why it should have a radius of 1.0 (width and height of 2.0)")]
		[SerializeField, Foldout("Visuals")]
		private Transform _craterVisualPrefab;

		[Range(0, 1)]
		private float _minRadius = 0.1f;
		#endregion
		#region Private Variables
		private Finger _finger;
		private Transform _craterVisual;
		private Vector2 _center;

		private CraterEntry _craterEntry = new CraterEntry(Vector2.zero, 0.0f);

		#endregion

		#region Unity Functions
		private void OnEnable()
		{
			EnhancedTouchSupport.Enable();
			if (_craterLogger == null)
			{
				_craterLogger = FindObjectOfType<CraterLogger>();
			}
			UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
			UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += FingerUp;
		}

		private void OnDisable()
		{
			UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
			UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= FingerUp;
			if (_craterVisual != null)
			{
				Destroy(_craterVisual.gameObject);
				_craterVisual = null;
			}
		}
		private void Update()
		{
			if(_finger == null)
			{
				_craterLogger.ClearOverridePreview();
				return;
			}
			var position = GetFingerPosition();
			var radius = Vector2.Distance(position, _center);
			if(_craterVisual != null)
			{
				_craterVisual.localScale= new Vector3(radius, radius, radius);
			}
			UpdatetCraterEntry(_center, radius);
			_craterLogger.UpdateOverridingCraters(_craterEntry);
		}
		#endregion
		#region Public Functions

		#endregion

		#region Event Handlers

		private void FingerDown(Finger finger)
		{
			// we only care about two _fingers
			if (_finger != null)
				return;
			if (_finger == finger)
				return;
			_finger = finger;
			_center = GetFingerPosition();
			if (_craterVisualPrefab != null)
			{
				_craterVisual = Instantiate(_craterVisualPrefab, _targetImage.transform);
				_craterVisual.localPosition = _center;
				_craterEntry.Position = _center;
			}
		}

		private Vector2 GetFingerPosition()
		{
			return _targetImage.transform.InverseTransformPoint(_finger.screenPosition);
		}

		private void FingerUp(Finger finger)
		{ 
			if(_finger == finger)
			{
				if (finger.currentTouch.isTap)
				{
					DeleteCrater();
				} else
				{
					MarkCrater();
				}
				_finger = null;
				Destroy(_craterVisual.gameObject);
				_craterVisual = null;
			}
		}
		#endregion

		#region Private Functions

		private void MarkCrater()
		{
			_craterLogger.LogCrater(_craterEntry.Clone());
		}

		private void DeleteCrater()
		{
			Vector2 pos = GetFingerPosition() + Vector2.one * 0.5f;
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
