using ImageAnnotation.Marking.Visuals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

namespace ImageAnnotation.Marking
{
    [AddComponentMenu("Image Annotation/Marking/Crater Logger")]
    public class CraterLogger : MonoBehaviour
    {
        [SerializeField]
        CraterVisual _craterVisualPrefab;
        [SerializeField, FormerlySerializedAs("_targetTexture")]
        public RawImage TargetTexture;

        List<CraterEntry> _entries = new List<CraterEntry>();
        List<CraterVisual> _visuals = new List<CraterVisual>();
        List<CraterEntry> _overridingEntries = new List<CraterEntry>();

        public void LogCrater(Vector2 position, float radius)
        {
            LogCrater(new CraterEntry(position, radius));
        }
        public void LogCrater(CraterEntry entry)
        {
            if(_entries.Contains(entry)) {
                return;
            }
            UpdateOverridingCraters(entry);

            for (int i = _overridingEntries.Count - 1; i >= 0; i--)
            {
                RemoveCrater(_overridingEntries[i]);
            }

            _entries.Add(entry);
            Vector2 localPos = Vector2.Scale(entry.Position, new Vector2(1.0f/TargetTexture.texture.width, 1.0f/TargetTexture.texture.height)) - Vector2.one * 0.5f;
            localPos.y *= -1;
            var _visual = Instantiate(_craterVisualPrefab, TargetTexture.transform);
            _visual.transform.localPosition = localPos;
            _visual.transform.localScale = Vector3.one * entry.Radius / TargetTexture.texture.width;
            _visuals.Add(_visual);
        }
        public void RemoveClosest(Vector2 position, float maxDistance = float.MaxValue)
        {
            CraterEntry found = null;

            foreach(var entry in _entries)
            {
                float distance = Vector2.Distance(position, entry.Position);
                if (distance < maxDistance)
                {
                    maxDistance = distance;
                    found = entry;
                }
            }
            if(found != null)
            {
                RemoveCrater(found);
            }
        }

        public void RemoveCrater(CraterEntry entry)
        {
            var index = _entries.IndexOf(entry);
            _entries.RemoveAt(index);
            _visuals[index].Delete();
            _visuals.RemoveAt(index);
            _overridingEntries.Remove(entry);
        }

        public void Clear() {
            _entries.Clear();
            foreach(var mark in _visuals) {
                mark.Delete();
            }
            _visuals.Clear();
            _overridingEntries.Clear();
        }

        public IEnumerable<CraterEntry> GetCraters() {
            return _entries;
        }

        public void UpdateOverridingCraters(CraterEntry entry)
        {
            UpdateOverridingCraters(entry.Position, entry.Radius);
        }

        public void UpdateOverridingCraters(Vector2 position, float radius)
        {
            ClearOverridePreview();
            GetOverridingCraters(position, radius, ref _overridingEntries);
            foreach (var currentlyMarked in _overridingEntries)
            {
                var index = _entries.IndexOf(currentlyMarked);
                _visuals[index].SetPreviewDelete(true);
            }

        }

        internal void ClearOverridePreview()
        {
            foreach (var currentlyMarked in _overridingEntries)
            {
                var index = _entries.IndexOf(currentlyMarked);
                _visuals[index].SetPreviewDelete(false);
            }
            _overridingEntries.Clear();
        }

        private void GetOverridingCraters(CraterEntry entry, ref List<CraterEntry> outOverridingCraters)
        {
            GetOverridingCraters(entry.Position, entry.Radius, ref outOverridingCraters);
        }

        private void GetOverridingCraters(Vector2 position, float radius, ref List<CraterEntry> outOverridingCraters)
        {
            foreach (var existing in _entries)
            {
                var distance = Vector2.Distance(existing.Position, position);
                var minDistance = (existing.Radius + radius) / 4;
                if (distance < minDistance)
                {
                    outOverridingCraters.Add(existing);
                }
            }
        }
    }
}
