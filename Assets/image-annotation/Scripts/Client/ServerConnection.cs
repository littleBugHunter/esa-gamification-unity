/* Singleton to manage Server Communication
 * <author>Paul Nasdalack</author>
 */
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace ImageAnnotation.Client
{
    public class ServerConnection : MonoBehaviour
    {
        #region Singleton

        /// <summary>
        /// Singleton to manage Server Communication.
        /// </summary>
        public static ServerConnection Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
                Destroy(gameObject);
        }

        #endregion
        #region Serialized Fields
        [SerializeField]
        private bool _useLocalServer;
        [SerializeField, DisableIf("_useLocalServer")]
        private string _url = "localhost";
        [SerializeField, DisableIf("_useLocalServer")]
        private int _port = 80;
        [SerializeField, EnableIf("_useLocalServer")]
        private int _localPort = 3000;
        public string BasePath => basePath;
        private string basePath;
        [SerializeField]
        private string _applicationId = "";
        [SerializeField]
        private bool _setUserNameFromScript = false;
        [SerializeField, DisableIf("_setUserNameFromScript")]
        public string UserName = "";
        #endregion
        #region Private Variables

        #endregion

        #region Unity Functions
        private void Start()
        {
            if(_useLocalServer)
            {
                _url = "localhost";
                _port = _localPort;
            }
            if(_port != 80)
            {
                basePath = _url + ":" + _port.ToString("D4") + "/";
            } else
            {
                basePath = _url + "/";
            }
            if(_applicationId.Length == 0)
            {
                Debug.LogError("Please set an ApplicationID in the Server Settings (example: 'MoonKingdom')");
            }
            if(UserName.Length == 0)
            {
                if(!_setUserNameFromScript)
                {
                    Debug.LogWarning("User Name is not set! Make sure to set it from a Script or via the UI");
                }
                var uniqueId = SystemInfo.deviceUniqueIdentifier;
                MD5 md5Hasher = MD5.Create();
                var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(uniqueId));
                var seed = BitConverter.ToInt32(hashed, 0);
                UserName = NameGenerator.Generate(seed);
            }
        }
        #endregion
        #region Public Functions

        #endregion

        #region Private Functions

        #endregion
    }
}
