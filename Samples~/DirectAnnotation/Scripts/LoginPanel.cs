using ImageAnnotation.Client;
using ImageAnnotation.Samples.DirectAnnotation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ImageAnnotation.Samples.DirectMode
{
    public class LoginPanel : MonoBehaviour
    {
        [SerializeField]
        StateGroup stateGroup;
        [SerializeField]
        private string _mainMenuState;
        [SerializeField]
        Button _loginButton;

        private void Start()
        {
            string existingName = PlayerPrefs.GetString("UserName", "");
            ServerConnection.Instance.UserName = existingName;
            if(existingName.Length > 0)
            {
                stateGroup.SelectState(_mainMenuState);
            }
        }

        public void SetName(string name)
        {
            PlayerPrefs.SetString("UserName", name);
            ServerConnection.Instance.UserName = name;
            _loginButton.interactable = name.Length > 0;
        }
    }
}