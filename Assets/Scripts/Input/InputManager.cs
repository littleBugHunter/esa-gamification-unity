// Created by Krista Plagemann //
// Handles touch input. //

using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    #region Singleton

    /// <summary>
    /// Singleton to manage the input.
    /// </summary>
    public static InputManager Instance{ get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);

        EnhancedTouchSupport.Enable();
    }

    #endregion


    // Uses the new EnhancedTouch input system actions to add our own functionality to finger down/ finger up events.//
    private void OnEnable()
    {
        TouchSimulation.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += FingerUp;

    }

    private void OnDisable()
    {
        TouchSimulation.Disable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= FingerUp;
    }

    #region Touch types and delegates

    /////////////////////
    /// Primary Touch ///
    /////////////////////

    // We use delegates to divide input into primary and secondary start and end events.
    // This way we can easily subscribe a function to these events (OnPrimaryStartEvent in this case)
    // so it gets executed every time we start or end a touch with a specific finger
    public delegate void OnPrimaryStartEvent(Finger finger);
    public event OnPrimaryStartEvent OnPrimaryStartTouch;
    private void PrimaryStartTouch(Finger finger)
    {
        // check if there is anything in the delegate first (the? is a null check) and if so we execute
        OnPrimaryStartTouch?.Invoke(finger);
        //Debug.Log("Primary start touch");
    }

    public delegate void OnPrimaryEndEvent(Finger finger);
    public event OnPrimaryEndEvent OnPrimaryEndTouch;
    private void PrimaryEndTouch(Finger finger)
    {
        OnPrimaryEndTouch?.Invoke(finger);
        //Debug.Log("Primary end touch");
    }

    ///////////////////////
    /// Secondary Touch ///
    ///////////////////////

    public delegate void OnSecondaryStartEvent(Finger finger);
    public event OnSecondaryStartEvent OnSecondaryStartTouch;
    private void SecondaryStartTouch(Finger finger)
    {
        OnSecondaryStartTouch?.Invoke(finger);
        //Debug.Log("Secondary touch started.");
    }

    public delegate void OnSecondaryEndEvent(Finger finger);
    public event OnSecondaryEndEvent OnSecondaryEndTouch;
    private void SecondaryEndTouch(Finger finger)
    {
        OnSecondaryEndTouch?.Invoke(finger);
        //Debug.Log("Secondary touch ended.");
    }

    #endregion

    // When a finger touches the screen, we evaluate if it's the first or second and according to that execture the right delegate
    private void FingerDown(Finger finger)
    {
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].finger == finger)
        {
            PrimaryStartTouch(finger);
        }
        else
        {
            // We literally ignore any touches after the first two fingers ( you can make more ifs if you need them)
            if(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count <= 2)
            SecondaryStartTouch(finger);
        }
    }

    private void FingerUp(Finger finger)
    {
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count <= 1)
        {
            PrimaryEndTouch(finger);
        }
        else
        {
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count <= 2)
                SecondaryEndTouch(finger);
        }
    }

}
