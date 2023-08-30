using ImageAnnotation.Client.Requests;
using ImageAnnotation.Client;
using ImageAnnotation.Marking;
using ImageAnnotation.Samples.DirectAnnotation;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ImageAnnotation.Samples.DirectMode
{
    public class Highscore : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField]
        private Transform _highscorePanel;
        [SerializeField]
        private HighscoreEntry _highscoreEntry;
        #endregion
        #region Private Variables

        #endregion
        #region Structs
        [Serializable]
        struct HighscoreData
        {
            [Serializable]
            public struct Entry
            {
                public string name;
                public int picturesMarked;
            }
            public Entry[] entries;
        }
        #endregion
        #region Unity Functions
        private void OnEnable()
        {
            UpdateHighscore();
        }
        #endregion
        #region Public Functions
        private void UpdateHighscore()
        {
            StartCoroutine(UpdateHighscoreCoroutine());
        }
        #endregion


        #region Private Functions
        IEnumerator UpdateHighscoreCoroutine()
        {
            var highscoreRequest = new GetObjectJson<HighscoreData>("direct-highscore");
            yield return new WaitUntil(() => highscoreRequest.isDone);
            var highscoreData = highscoreRequest.GetResult();
            for(int i = 0; i < _highscorePanel.childCount; ++i)
            {
                var child = _highscorePanel.GetChild(i);
                Destroy(child.gameObject);
            }
            for (int i = 0; i < highscoreData.entries.Length; ++i)
            {
                var dataEntry = highscoreData.entries[i];
                var entry = Instantiate(_highscoreEntry, _highscorePanel);
                entry.Rank.text = "#" + (i+1).ToString("D3");
                entry.Name.text = dataEntry.name;
                entry.PicturesMarked.text = dataEntry.picturesMarked.ToString();
            }
        }
        #endregion
    }
}