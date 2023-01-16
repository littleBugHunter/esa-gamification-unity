using ImageAnnotation.Marking.Visuals;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

namespace ImageAnnotation.Marking
{
    /// <summary>
    /// The CraterLogger temporarly holds the marked craters before they are submitted to the server.
    /// The Logger also handles displaying the Circles around marked Craters.
    /// It is used by the Marking Panel and the Marker Components to exchange Marked Crater Data
    /// </summary>
    [AddComponentMenu("Image Annotation/Marking/Crater Logger")]
    public class CraterLogger : MonoBehaviour
    {
        [SerializeField, InfoBox("A UI Object that visualizes a marked Crater. Should be a Circle with radius one")]
        CraterVisual _craterVisualPrefab;
        [SerializeField, FormerlySerializedAs("_targetTexture"), InfoBox("The RawTexture UI Component that shows the Marking Image")]
        public RawImage TargetTexture;

        List<CraterEntry> _entries = new List<CraterEntry>();
        List<CraterVisual> _visuals = new List<CraterVisual>();
        List<CraterEntry> _overridingEntries = new List<CraterEntry>();

        /// <summary>
        /// Adds a Crater to the Log
        /// </summary>
        /// <param name="position">Position in Pixel Coordinates</param>
        /// <param name="radius">The Radius in Pixels</param>
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
        /// <summary>
        /// Removes the Closest Crater to the specified position
        /// </summary>
        /// <param name="position">The Position in Pixels</param>
        /// <param name="maxDistance">The Maximum Distance for deletion in Pixels</param>
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
        /// <summary>
        /// Removes the specified Entry
        /// </summary>
        /// <param name="entry">The Entry to remove</param>
        public void RemoveCrater(CraterEntry entry)
        {
            var index = _entries.IndexOf(entry);
            _entries.RemoveAt(index);
            _visuals[index].Delete();
            _visuals.RemoveAt(index);
            _overridingEntries.Remove(entry);
        }

        /// <summary>
        /// Removes all entries from the Log
        /// </summary>
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

        /// <summary>
        /// Displays all CraterVisuals that can be overridden by placing a crater at the specified entry position in their markedForDelete State
        /// </summary>
        /// <param name="entry">The Crater Entry to be placed</param>
        public void UpdateOverridingCraters(CraterEntry entry)
        {
            UpdateOverridingCraters(entry.Position, entry.Radius);
        }
		/// <summary>
		/// Displays all CraterVisuals that can be overridden by placing a crater at the specified entry position in their markedForDelete State
		/// </summary>
		/// <param name="position">The Position of the new Entry in Pixel Coordinates</param>
		/// <param name="radius">The Radius of the new Entry in Pixels</param>
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

		/// <summary>
		/// Clears the markedForDelete State of all CraterVisuals
		/// </summary>
		internal void ClearOverridePreview()
        {
            foreach (var currentlyMarked in _overridingEntries)
            {
                var index = _entries.IndexOf(currentlyMarked);
                _visuals[index].SetPreviewDelete(false);
            }
            _overridingEntries.Clear();
        }
        /// <summary>
        /// Get All CraterEntries that will be overridden if the new entry is placed
        /// </summary>
        /// <param name="entry">The entry to be placed</param>
        /// <param name="outOverridingCraters">A List to be filled with entries that will be overridden by placing the crater</param>
        private void GetOverridingCraters(CraterEntry entry, ref List<CraterEntry> outOverridingCraters)
        {
            GetOverridingCraters(entry.Position, entry.Radius, ref outOverridingCraters);
        }
		/// <summary>
		/// Get All CraterEntries that will be overridden if the new entry is placed
		/// </summary>
		/// <param name="position">The Position of the new Entry in Pixel Coordinates</param>
		/// <param name="radius">The Radius of the new Entry in Pixels</param>
		/// <param name="outOverridingCraters">A List to be filled with entries that will be overridden by placing the crater</param>
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
