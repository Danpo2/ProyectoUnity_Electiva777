using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GesturesController : MonoBehaviour
{
    private Vector2 startTouchPos;
    private float initialDistance;
    private Vector3 initialScale;

    void Update()
    {
        // 1. Rotación con un dedo (mover horizontalmente)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float rotX = touch.deltaPosition.y * 0.1f; // Arrastre vertical rota en X
                float rotY = -touch.deltaPosition.x * 0.1f; // Arrastre horizontal rota en Y
                transform.Rotate(rotX, rotY, 0, Space.World);
            }
        }

        // 2. Zoom con dos dedos (pinch)
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Distancia entre los dedos
            float currentDist = Vector2.Distance(touch0.position, touch1.position);

            if (touch1.phase == TouchPhase.Began)
            {
                initialDistance = currentDist;
                initialScale = transform.localScale;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                if (Mathf.Approximately(initialDistance, 0)) return;

                float factor = currentDist / initialDistance;
                transform.localScale = initialScale * factor; // Escalar el objeto
            }
        }

        // 3. Subir / bajar objeto en eje Y (arrastre vertical con un dedo)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float moveY = touch.deltaPosition.y * 0.01f;
                transform.Translate(0, moveY, 0, Space.World);
            }
        }
    }
}

