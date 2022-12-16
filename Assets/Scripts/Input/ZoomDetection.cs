// Created by Krista Plagemann //
// Handles zooming and moving of the map. //

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class ZoomDetection : MonoBehaviour
{
    private TouchControls controls;     // Automatically generated script with input settings
    private InputManager inputManager;  // The instance of our input manager script
    private Coroutine zoomCoroutine;    // A coroutine to zoom the map.
    private Coroutine moveCoroutine;    // And a coroutine to move the map
    private Camera mainCamera;          // To store the main camera

    [Tooltip("How fast we can zoom in and out of the map.")]
    [SerializeField]
    private float zoomSpeed = 2f;
    [Tooltip("How fast we can move the map around.")]
    [SerializeField]
    private float moveSpeed = 2f;

    [Tooltip("The object we want to be able to zoom. We will use this for moving.")]
    [SerializeField]
    private RectTransform zoomObject;
    [Tooltip("The parent of the object we want to be able to zoom. We will use this to zoom.")]
    [SerializeField]
    private RectTransform zoomParent;

    private void Awake()
    {
        if(zoomObject != null)
            zoomParent = zoomObject.transform.parent.GetComponent<RectTransform>();
        controls = new TouchControls();
        mainCamera = Camera.main;
        inputManager = InputManager.Instance;
        GameHandler.Instance.OnStateChangedEvent += ChangeInput;    // To change the input on state change.
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        controls.Disable();
    }

    /// <summary>
    /// Add or remove zoom and move input according to state.
    /// </summary>
    /// <param name="state">Next game state.</param>
    private void ChangeInput(GameState state)
    {
        if (state == GameState.CraterRadarMark)
        {
            inputManager.OnPrimaryStartTouch += MoveStart;
            inputManager.OnPrimaryEndTouch += MoveEnd;
            inputManager.OnSecondaryStartTouch += ZoomStart;
            inputManager.OnSecondaryEndTouch += ZoomEnd;
        }
        else
        {
            inputManager.OnPrimaryStartTouch -= MoveStart;
            inputManager.OnPrimaryEndTouch -= MoveEnd;
            inputManager.OnSecondaryStartTouch -= ZoomStart;
            inputManager.OnSecondaryEndTouch -= ZoomEnd;
        }
    }

    private Finger primaryFinger;   // to store the primary finger while one finger is touching

    #region MOVING

    /// On primary start touch > start moving object.
    private void MoveStart(Finger finger)
    {
        primaryFinger = finger;
        if(this != null)    moveCoroutine = StartCoroutine(Moving(finger));
    }
    /// On primary touch end > Stop moving object.
    private void MoveEnd(Finger finger)
    {
        primaryFinger = null;
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
    }

    /// <summary>
    /// Moves the zoom object according to finger movement as long as this coroutine is running.
    /// </summary>
    IEnumerator Moving(Finger finger)
    {
        Vector2 currentPosition = finger.screenPosition;    // take the first position from the finger position
        Vector2 newPosition;    // for the next position
        // Record the offset from where you're moving the finger to where the transform middle of the image is //
        Vector2 offset = currentPosition - new Vector2(zoomObject.position.x, zoomObject.position.y);

        while (true)    // Could also use a boolean for this instead
        {
            newPosition = finger.screenPosition;    // get the new position from the finger position constantly
            // then we move the object position smoothly to the new position (with the offset calculated, so the object
            // doesn't jump to the finger position but moves only in the direction the finger moves in
            zoomObject.position = Vector2.Lerp(zoomObject.position, newPosition - offset, Time.deltaTime * moveSpeed);
            yield return null;
        }
    }

    #endregion

    #region ZOOMING


    /// On secondary finger start touch > start zooming object.
    private void ZoomStart(Finger finger)
    {
        StopCoroutine(moveCoroutine);
        if (this != null) zoomCoroutine = StartCoroutine(Zooming(primaryFinger, finger));
    }

    /// On secondary finger stop touch > stop zooming object.
    private void ZoomEnd(Finger finger)
    {
        StopCoroutine(zoomCoroutine);
    }

    /// <summary>
    /// Scales the parent object of the sprite according to pinch gesture.
    /// </summary>
    IEnumerator Zooming(Finger primaryFinger, Finger secondaryFinger)
    {
        // get the first distance to start with
        float previousDistance = Vector2.Distance(primaryFinger.screenPosition, secondaryFinger.screenPosition);

        while (true)
        {
            // this fixes a bug lel
            if (primaryFinger == null)
                yield break;
            // calculate the current distance of the 2 fingers
            float distance = Vector2.Distance(primaryFinger.screenPosition, secondaryFinger.screenPosition);
          
            //float change = Mathf.Abs(distance - previousDistance) / 10; // Should calculate the change to zoom faster or slower if greater change (a bit wonky, maybe different approach)

            // Zoom out
            if(distance < previousDistance) // if the fingers get closer to each other, we zoom out
            {
                Vector3 targetScale = zoomParent.transform.localScale;  // get the current scale 
                targetScale.x -= 1;
                targetScale.y -= 1;
                // Scale the parent object smoothly from the current value to the new one.
                // Uses zoomSpeed to control overall speed and using the local scaleshould maybe even out the really big and really small scale values... mabye
                if (targetScale.y > -0.99)  // the values confused me but this helps to not make it do weird stuff lmao sorry
                    zoomParent.localScale = Vector3.Slerp(zoomParent.localScale, targetScale, Time.deltaTime * zoomSpeed * zoomParent.localScale.x);
            }
            // Zoom in
            else if(distance > previousDistance)    // if fingers are further apart than before, zoom in
            {
                // same thing, but +
                Vector3 targetScale = zoomParent.transform.localScale;
                targetScale.x += 1;
                targetScale.y += 1;
                zoomParent.localScale = Vector3.Slerp(zoomParent.localScale, targetScale, Time.deltaTime * zoomSpeed * zoomParent.localScale.x);
                
            }
            // this just evens out the z scale because it shouldn't change, but it does grr
            zoomParent.localScale = new Vector3(zoomParent.localScale.x, zoomParent.localScale.y, 1);

            previousDistance = distance;    // and record the current distance again as the previous distance
            yield return null;
        }
    }

    #endregion
}
