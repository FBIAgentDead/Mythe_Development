﻿//////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////
/////// This script has been build to be the movement for the game, cool right. ;) ///////
//////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////// -Sjors

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] [Range(0, 2)] float speed = 1f, /*ForceJump = 5f,*/ castLenght = 1.1f;
    [SerializeField] string ropeTag = "Rope";
    PlayerInput playerInput;

    Rigidbody2D rb;
    Collider2D col;
    RaycastHit2D sideL, sideR;
    int layerMask = ~(1 << 9); //Give values with what the raycasts can interract(in this case excluding player layer)

    //bool Jumping = true;

    [ExecuteInEditMode]
    void Awake()
    {
        //Get all the required components, and lock the rigidbody's rotations
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerInput = GetComponent<PlayerInput>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void FixedUpdate()
    {
        //Run functions
        Move();
        CheckCasts();
        //Jump(); //Jump has been disabled but has been asked to stay in here for whatever reason, sorry!
    }

    void Move()
    {
        //Check player joystick input, and move it depending on the values and if the raycast hits anything.
        Vector2 movementInput = playerInput.JoystickMove;
        if (((sideL.collider == null && movementInput.x < 0) || (sideR.collider == null && movementInput.x > 0)))
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            transform.Translate(movementInput.x * (speed * 10) * Time.deltaTime, 0, 0);
        }
        /*
        else if (sideL || sideR)
        {
            if (sideL.collider.gameObject.tag == ropeTag)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                transform.position = sideL.point;
            }
            if (sideR.collider.gameObject.tag == ropeTag)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                transform.position = sideR.point;
            }
        }
        */
    }

    /* 
	void Jump () {
        if (playerInput.AB && !Jumping)
        {
            Jumping = true;
            rb.AddForce((Vector2.up * (ForceJump * 5000)) * Time.deltaTime);
        }
	}
    */

    void CheckCasts()
    {
        /*
        RaycastHit2D downWard = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - transform.lossyScale.y / 2), Vector2.down, (castLenght / 10), layerMask);
        if(downWard.collider == null)
        {
            Jumping = true;
        }
        else
        {
            Jumping = false;
        }
        */

        //See where colliders are at the sides by taking the size of the player, and basing it off that with a lenght distance. (math aka magic)
        sideL = Physics2D.Raycast(transform.position, Vector2.left, (castLenght * col.bounds.size.x / 2), layerMask);
        sideR = Physics2D.Raycast(transform.position, Vector2.right, (castLenght * col.bounds.size.x / 2), layerMask);
        Debug.DrawLine(transform.position, transform.position + new Vector3(-((castLenght * col.bounds.size.x / 2)), 0));
        Debug.DrawLine(transform.position, transform.position + new Vector3((castLenght * col.bounds.size.x / 2), 0));

    }

}