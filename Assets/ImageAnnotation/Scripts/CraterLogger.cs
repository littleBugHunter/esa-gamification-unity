using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ImageAnnotation
{
    public class CraterLogger : MonoBehaviour
    {
        [SerializeField]
        RectTransform _craterMarkPrefab;
        [SerializeField]
        RawImage _targetTexture;

        List<CraterEntry> _entries = new List<CraterEntry>();
        List<GameObject> _marks = new List<GameObject>();

        public void LogCrater(Vector2 position, float radius)
        {
            LogCrater(new CraterEntry(position, radius));
        }
        public void LogCrater(CraterEntry entry)
        {
            if(_entries.Contains(entry)) {
                return;
            }
            _entries.Add(entry);
            Vector2 localPos = Vector2.Scale(entry.Position, new Vector2(1.0f/_targetTexture.texture.width, 1.0f/_targetTexture.texture.height)) - Vector2.one * 0.5f;
            localPos.y *= -1;
            var mark = Instantiate(_craterMarkPrefab, _targetTexture.transform).gameObject;
            mark.transform.localPosition = localPos;
            mark.transform.localScale = Vector3.one * entry.Radius / _targetTexture.texture.width;
            _marks.Add(mark);
        }
        public void Clear() {
            _entries.Clear();
            foreach(var mark in _marks) {
                Destroy(mark);
            }
            _marks.Clear();
        }

        public IEnumerable<CraterEntry> GetCraters() {
            return _entries;
        }

    }
}
