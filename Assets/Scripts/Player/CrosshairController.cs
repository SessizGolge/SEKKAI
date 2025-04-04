using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    private bool facingRight = true;
    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        transform.position = mousePos;

        if (player == null) return;

        bool shouldFaceRight = transform.position.x > player.transform.position.x;

        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
        }
    }

    void FlipCharacter()
    {
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("AkaiRight", facingRight);
            anim.SetBool("AkaiLeft", !facingRight);
        }
    }
}

