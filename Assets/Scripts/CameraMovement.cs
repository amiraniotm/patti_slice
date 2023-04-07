using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Camera movement mainly controls two things: upwards displacement between phases, and shaking on flipbox use

    // Editor vars for upwards displacement
    [SerializeField] public float panSpeed, panMultiplier;
    [SerializeField] private MasterController masterController;
    // Editor vars for screen shaking
    [SerializeField] private float shakeDuration, shakeMagnitude;
    // Runtime vars for movement control and camera dimensions
    public float panCount, shakeCount, screenHeight, screenWidth, panAdjDistance;
    private Vector3 initialPosition;
    private Camera cam;
    public Vector3 panningEndPoint;
    private bool doPanUp;

    private void Start()
    {
        // Preserving object and getting initial settings
        DontDestroyOnLoad(gameObject);
        SetInitialShakePos();
        SetCameraDimensions();
        // Setting pan adjusted distance: how far will the screen pan
        //panningEndPoint = transform.position;
        panAdjDistance = panMultiplier * screenHeight;
    }

    private void SetCameraDimensions()
    {
        cam = GetComponent<Camera>();
        var screenBottomLeft = cam.ViewportToWorldPoint(new Vector2(0, 0));
        var screenTopRight = cam.ViewportToWorldPoint(new Vector2(1, 1));
        
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        screenHeight = screenTopRight.y - screenBottomLeft.y;
    }

    public void SetInitialShakePos()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Shake triggered when counter is set different than zero
        if(shakeCount != 0f) {
            Shake();
        }
        // Pan is not timed but triggered by boolean on position conditions
        if(doPanUp) {
            PanUp();
        }
    }
    // Kickstarting the shake counter
    public void TriggerShake()
    {
        shakeCount = shakeDuration;
    }

    private void Shake()
    {
        // While counter is on, shake screen inside magnitude value, and reduce counter
        if(shakeCount > 0) {
            transform.position = initialPosition + (Random.insideUnitSphere * shakeMagnitude);
            shakeCount -= Time.deltaTime;
        // When counter runs out, stabilize screen to initial position
        } else {
            shakeCount = 0f;
            transform.position = initialPosition;
        }
    }
    // Kickstarting pan and marking end point of displacement
    public void TriggerPan()
    {
        doPanUp = true;
        panningEndPoint = new Vector3(transform.position.x, transform.position.y + panAdjDistance, transform.position.z);
    }

    private void PanUp()
    {
        float step = panSpeed * Time.deltaTime;
        // Pans up until position is equal to displacement end point
        if(panningEndPoint != transform.position) {
            transform.position = Vector3.MoveTowards(transform.position, panningEndPoint, step);
        // When position reaches endpoint, end displacement
        } else {
            doPanUp = false;
            panningEndPoint = transform.position;
            masterController.EndScrollPhase();
        }
    }

    public Vector3 GetCorner(string corner)
    {
        Vector3 cornerPos = Vector3.zero;

        if(corner == "lowerleft") {
            cornerPos = new Vector3(transform.position.x - (screenWidth / 2),
                                    transform.position.y - (screenHeight / 2),
                                    transform.position.z);
        } else {
            cornerPos = new Vector3(transform.position.x + (screenWidth / 2),
                                    transform.position.y + (screenHeight / 2),
                                    transform.position.z);
        }

        return cornerPos;
    } 
}
