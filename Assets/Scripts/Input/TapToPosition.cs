using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class TapToPosition : MonoBehaviour
{
    private InputManager inputManager;
    private Camera cameraMain;

    [SerializeField]
    private RectTransform objectToPosition;

    void Awake()
    {
        inputManager = InputManager.Instance;
        cameraMain = Camera.main;
    }

    private void OnEnable()
    {
        inputManager.OnPrimaryStartTouch += Move;
    }

    private void OnDisable()
    {
        inputManager.OnPrimaryStartTouch -= Move;
    }

    public void Move(Finger finger)
    {
        Vector3 screenCoordinates = new Vector3(finger.screenPosition.x, finger.screenPosition.y, cameraMain.nearClipPlane);
        objectToPosition.position = screenCoordinates;
    }

}
