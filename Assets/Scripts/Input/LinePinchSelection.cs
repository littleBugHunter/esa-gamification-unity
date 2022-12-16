// Created by Krista Plagemann //
// Handles marking craters through calculating the middle point between 2 fingers as marker spots. //

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;


public class LinePinchSelection : MonoBehaviour
{
    private InputManager inputManager;  // The instance of our input manager script
    private Finger primaryFinger;       // First finger that touches the screen

    [Tooltip("Sprite or similar that highlights where the fingers touch the screen.")]
    [SerializeField]
    private GameObject fingerIndicatorPrefab;

    [Tooltip("Sprite or similar that highlights and marks the spot in between fingers.")]
    [SerializeField]
    private GameObject middleSpotIndicator;

    [Tooltip("Empty parent object that the crater markers will be children of.")]
    [SerializeField]
    private GameObject markerParent;

    [Tooltip("Objects that belong to pinch marking only to disable when in other mode.")]
    [SerializeField]
    private GameObject[] pinchSelectionObjects;

    private GameObject primaryFingerPosIndicator;       // stores the finger highlighters
    private GameObject secondaryFingerPosIndicator;

    private void Start()
    {
        inputManager = InputManager.Instance;
        GameHandler.Instance.OnStateChangedEvent += changeInput;
    }

    #region Input handling

    // Adds the new input to the inputManager when in the CraterPinchMark state and removes when not //
    private void changeInput(GameState state)
    {
        if (state == GameState.CraterPinchMark)
        {
            inputManager.OnPrimaryStartTouch += PrimaryStart;
            inputManager.OnPrimaryEndTouch += PrimaryEnd;
            inputManager.OnSecondaryStartTouch += LineStart;
            inputManager.OnSecondaryEndTouch += LineEnd;
            foreach (GameObject go in pinchSelectionObjects)
                go.SetActive(true);
        }
        else
        {
            inputManager.OnPrimaryStartTouch -= PrimaryStart;
            inputManager.OnPrimaryEndTouch -= PrimaryEnd;
            inputManager.OnSecondaryStartTouch -= LineStart;
            inputManager.OnSecondaryEndTouch -= LineEnd;
            foreach (GameObject go in pinchSelectionObjects)
                go.SetActive(false);
        }
    }

    private Coroutine primaryIndicatorCoroutine;

    /// On primary start touch > start coroutine to show indicator of primary finger.
    private void PrimaryStart(Finger finger)
    {
        primaryFinger = finger;
        if (this != null) primaryIndicatorCoroutine = StartCoroutine(primaryPosIndicator(finger));
    }

    /// On primary end touch is the last touch to end, so we destroy and end the coroutine of the finger that hasn't yet ended.
    private void PrimaryEnd(Finger finger)
    {       
        // If we haven't destroyed the secondary indicator, then we do so now and stop the coroutine. Else we do so with the other.
        if (secondaryFingerPosIndicator == null)
        {
            StopCoroutine(primaryIndicatorCoroutine);
            Destroy(primaryFingerPosIndicator);
        }
        else
        {
            StopCoroutine(secondaryIndicatorCoroutine);
            Destroy(secondaryFingerPosIndicator);
        }
        StopCoroutine(craterMarking); // Always stop the marking function for good measure :D
    }

    private Coroutine secondaryIndicatorCoroutine;
    private Coroutine craterMarking; // actually also the marking function, used to be for line only
    /// On secondary start touch > start coroutine to show indicator of secondary finger and show middle marker point.
    private void LineStart(Finger finger)
    {
        if (this != null) secondaryIndicatorCoroutine = StartCoroutine(secondaryPosIndicator(finger));
        if (this != null) craterMarking = StartCoroutine(drawMarkerFingers(finger));
    }
    /// On secondary end touch > stop coroutine to move indicator of the removed finger and destroy it.
    private void LineEnd(Finger finger)
    {
        if (finger.index == 0)  // we check if the index (in the list of fingers existing) of this finger is 0 (so the primary finger)
        {
            StopCoroutine(primaryIndicatorCoroutine);
            Destroy(primaryFingerPosIndicator);
        }
        else
        {
            StopCoroutine(secondaryIndicatorCoroutine);
            Destroy(secondaryFingerPosIndicator);
        }
        StopCoroutine(craterMarking);   // Also stop moving the marker ofc
    }

    #endregion

    #region Finger Indicator Position

    /// Instantiates the finger indicator prefab and makes it follow the finger while touching.
    private IEnumerator primaryPosIndicator(Finger finger)
    {
        if (primaryFingerPosIndicator == null) // makes a new one only if there isn't one already (there shouldn't be but... yeah)
            // Spawn the indicator at finger position
            primaryFingerPosIndicator = Instantiate(fingerIndicatorPrefab, finger.screenPosition, Quaternion.identity, transform);
        while (true)
        {
            // then we just make it follow the finger.
            if (primaryFingerPosIndicator != null) primaryFingerPosIndicator.transform.position = finger.screenPosition;
            yield return null;
        }
    }

    /// Instantiates another finger indicator prefab and makes it follow the second finger while touching.
    private IEnumerator secondaryPosIndicator(Finger finger)
    {
        // same same
        if( secondaryFingerPosIndicator == null)
            secondaryFingerPosIndicator = Instantiate(fingerIndicatorPrefab, finger.screenPosition, Quaternion.identity, transform);
        while (true)
        {
            if (secondaryFingerPosIndicator != null) secondaryFingerPosIndicator.transform.position = finger.screenPosition;
            yield return null;
        }
    }

    #endregion

    #region Middle marker and line attempt

    private GameObject middleMarker;    // just to store the crater marker that follows the middle of the fingers

    //[SerializeField]
    //private GameObject lineConnection;
    //[SerializeField]
    //private LineRenderer lineRenderer;


    // Using a line renderer to make a line between the 2 fingers doesn't quite work like this because
    // line renderer will not render over a canvas that is set to Screen Space - Overlay.
    // One idea is using a 3D object instead of the line renderer. Another thing one could try is using a shader to adjust
    // the render queue, but not sure, have to try some things ;-;

    // This attempt does draw the line correctly I think (or did at some point), but it does not get rendered over the images... ;-;

    private IEnumerator drawMarkerFingers(Finger secondFinger)
    {
        // this object was for an attempt at making a 3D bar in the middle. It rotates funnily rn, but that could be a way if fixed.
        // GameObject angleBar = Instantiate(lineConnection, primaryFinger.screenPosition, Quaternion.identity);

        // Instantiates a marker for the crater using the markerParent as a parent object.
        middleMarker = Instantiate(middleSpotIndicator, markerParent.transform);
        while (true)
        {
            // Positions the marker in the middle of the two fingers (Lerp basically gives you smooth middle values between two objects,
            // ranging from 0 to 1. So by lerping to 0.5 between the two fingers, we get the middle vector position.
            middleMarker.transform.position = Vector2.Lerp(primaryFinger.screenPosition, secondFinger.screenPosition, 0.5f);

            // This is basically all that is needed to make the line renderer work in theory (but it's under the canvas)
            /* 
            lineRenderer.SetPosition(0, primaryFinger.screenPosition);  // one end of the line is the first finger
            lineRenderer.SetPosition(1, secondFinger.screenPosition);   // the other is the second finger
            */


            /*
            // Calculate the angle of the bar (ok this does NOT work at all lmao but at least you know it does not work like this((I copied this off something lel)) )
            Vector2 difference = secondFinger.screenPosition - primaryFinger.screenPosition;
            float sign = (secondFinger.screenPosition.y < primaryFinger.screenPosition.y) ? -1.0f : 1.0f;
            float angle = Vector2.Angle(Vector2.right, difference) * sign;
            angleBar.transform.Rotate(0, 0, angle);

            // Calculate length of bar (this works i think)
            float height = 5;
            float width = Vector2.Distance(primaryFinger.screenPosition, secondFinger.screenPosition);
            angleBar.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

            // Calculate midpoint position
            float newposX = primaryFinger.screenPosition.x + (secondFinger.screenPosition.x - primaryFinger.screenPosition.x) / 2;
            float newposY = primaryFinger.screenPosition.y + (secondFinger.screenPosition.y - primaryFinger.screenPosition.y) / 2;
            angleBar.transform.position = new Vector3(newposX, newposY, 0);
            *
            // Set parent to this object
            angleBar.transform.SetParent(transform, true);
            */
            yield return null;
        }
    }

    #endregion

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
