using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGroupByTouch : MonoBehaviour
{
    public Transform[] targets; // Balok dan Cube
    public float rotationSpeed = 0.2f;

    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                if (IsTouchingAnyTarget(touchPos))
                {
                    lastTouchPosition = touchPos;
                    isDragging = true;
                }
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touchPos - lastTouchPosition;
                RotateTargets(delta);
                lastTouchPosition = touchPos;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }

        // Mouse (Editor)
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            if (IsTouchingAnyTarget(mousePos))
            {
                lastTouchPosition = mousePos;
                isDragging = true;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 delta = mousePos - lastTouchPosition;
            RotateTargets(delta);
            lastTouchPosition = mousePos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void RotateTargets(Vector2 delta)
    {
        float rotX = delta.y * rotationSpeed;
        float rotY = -delta.x * rotationSpeed;

        foreach (Transform target in targets)
        {
            target.Rotate(Vector3.up, rotY, Space.World);
            target.Rotate(Vector3.right, rotX, Space.World);
        }
    }

    bool IsTouchingAnyTarget(Vector2 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            foreach (Transform t in targets)
            {
                if (hit.transform == t)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
