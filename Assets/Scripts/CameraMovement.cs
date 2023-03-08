using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] public float panSpeed;
    [SerializeField] private float panMultiplier;
    [SerializeField] private MasterController masterController;
    
    private Vector3 initialPosition;
    private float shakeDuration = 0.3f;
    private float currentShakeTime = 0f;
    public float currentPanTime = 0f;
    private float shakeMagnitude = 0.7f;
    private float dampingSpeed = 1.0f;
    private Camera cam;
    public float screenWidth;
    public float screenHeight;
    private bool nextImageSet = false;
    public Vector3 panningEndPoint;
    private bool doPanUp;
    public float panAdjDist;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        SetInitialShakePos();

        cam = GetComponent<Camera>();

        var screenBottomLeft = cam.ViewportToWorldPoint(new Vector2(0, 0));
        var screenTopRight = cam.ViewportToWorldPoint(new Vector2(1, 1));
        
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        screenHeight = screenTopRight.y - screenBottomLeft.y;

        panningEndPoint = transform.position;
        panAdjDist = panMultiplier * screenHeight;
    }

    private void Update()
    {
        if(currentShakeTime != 0f) {
            Shake();
        }

        if(doPanUp) {
            PanUp();
        }
    }

    public void TriggerShake()
    {
        currentShakeTime = shakeDuration;
    }

    private void Shake()
    {
        if(currentShakeTime > 0) {
            transform.position = initialPosition + (Random.insideUnitSphere * shakeMagnitude);
            currentShakeTime -= Time.deltaTime * dampingSpeed;
        } else {
            currentShakeTime = 0f;
            transform.position = initialPosition;
        }
    }

    public void TriggerPan()
    {
        doPanUp = true;
        panningEndPoint = new Vector3(transform.position.x, transform.position.y + panAdjDist, transform.position.z);
    }

    private void PanUp()
    {
        float step = panSpeed * Time.deltaTime;

        if(panningEndPoint != transform.position) {
            transform.position = Vector3.MoveTowards(transform.position, panningEndPoint, step);
        } else {
            doPanUp = false;
            panningEndPoint = transform.position;
            masterController.EndScrollPhase();
        }
    }

    public void SetInitialShakePos()
    {
        initialPosition = transform.position;
    }

    public Vector3 GetCurrentCorner(string corner)
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
