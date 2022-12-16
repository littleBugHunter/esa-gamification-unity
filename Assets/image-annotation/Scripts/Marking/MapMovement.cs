using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImageAnnotation.Marking
{
    [AddComponentMenu("Image Annotation/Marking/Map Movement")]
    public class MapMovement : MonoBehaviour
    {
        Touch[] _lastTouches = new Touch[0];
        [SerializeField]
        RectTransform _parentView;
        [SerializeField]
        RectTransform _scrollingImage;

        private void Start()
        {
            if(_parentView == null)
            {
                _parentView = transform as RectTransform;
            }
        }

        void Update()
        {
            
            if(_lastTouches.Length == Input.touchCount)
            {
                switch (Input.touchCount)
                {
                    case 1:
                        Scroll();
                        break;
                    case 2:
                        Zoom();
                        break;
                }
            }
            _lastTouches = Input.touches;
        }

        private void Scroll()
        {
            Vector2 lastPos =  _lastTouches[0].position;
            Vector2 currentPos =  Input.touches[0].position;
            Vector2 deltaPos = currentPos-lastPos;

            _scrollingImage.Translate( deltaPos, Space.World );
        }

        private void Zoom()
        {
            Vector2 lastPos0 = _scrollingImage.worldToLocalMatrix.MultiplyPoint(_lastTouches[0].position);
            Vector2 currentPos0 = _scrollingImage.worldToLocalMatrix.MultiplyPoint(Input.touches[0].position);
            Vector2 lastPos1 = _scrollingImage.worldToLocalMatrix.MultiplyPoint(_lastTouches[1].position);
            Vector2 currentPos1 = _scrollingImage.worldToLocalMatrix.MultiplyPoint(Input.touches[1].position);

            float lastDist = (lastPos0 - lastPos1).magnitude;
            float currentDist = (currentPos0 - currentPos1).magnitude;
            float scaleFactor = currentDist / lastDist;

            Vector2 lastScaleCenter = (lastPos0 + lastPos1) * 0.5f;
            Vector2 currentScaleCenter = (currentPos0 + currentPos1) * 0.5f;

            Vector2 deltaPos0 = currentPos0 - lastPos0;
            Vector2 deltaPos1 = currentPos1 - lastPos1;


            //_scrollingImage.Translate((deltaPos0+deltaPos1)*0.5f, Space.World);
            void DrawCross(Vector3 pos, float size, Color color, float time)
            {
                Debug.DrawLine(pos + Vector3.left * size, pos + Vector3.right * size, color, time);
                Debug.DrawLine(pos + Vector3.up * size, pos + Vector3.down * size, color, time);
            }

            Vector2 beforeZoom0 = (Vector2)(_scrollingImage.localToWorldMatrix.MultiplyPoint(currentPos0));
            Vector2 beforeZoom1 = (Vector2)(_scrollingImage.localToWorldMatrix.MultiplyPoint(currentPos1));
            _scrollingImage.localScale *= scaleFactor;
            Vector2 afterZoom0 = (Vector2)(_scrollingImage.localToWorldMatrix.MultiplyPoint(lastPos0));
            Vector2 afterZoom1 = (Vector2)(_scrollingImage.localToWorldMatrix.MultiplyPoint(lastPos1));

            DrawCross(beforeZoom0, 10f, Color.grey, 1);
            DrawCross(afterZoom0, 10f, Color.blue, 1);

            Vector2 deltaZoom0 = beforeZoom0 - afterZoom0;
            Vector2 deltaZoom1 = beforeZoom1 - afterZoom1;

            _scrollingImage.Translate((deltaZoom0+deltaZoom1)*0.5f, Space.World);

        }

    }
}
