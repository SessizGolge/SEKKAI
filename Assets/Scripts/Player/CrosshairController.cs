using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    // [SerializeField] ObjectController objectController;

    void FixedUpdate()
    {
        Cursor.visible = false;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        transform.position = mousePos;
        // if (!objectController.isOpened) 
        // {
        // }
    }
}

