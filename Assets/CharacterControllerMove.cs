using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerMove : MonoBehaviour
{

    CharacterController controller;
    Animator animator;

    float speed = 2.0f;
    float jumpSpeed = 8.0f;
    float gravity = 20.0f;

    Vector3 moveDirection = Vector3.zero;
    float xMove;
    float yMove;
    bool jump;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        animator.SetBool("Grounded", controller.isGrounded);

        if (controller.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes
            moveDirection = new Vector3(xMove, 0.0f, yMove);
            moveDirection *= speed;

            // Face in direction of movement.
            if (moveDirection.magnitude > float.Epsilon)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            if (jump)
            {
                moveDirection.y = jumpSpeed;
            }
            
            
        }
        //set moveSpeed before applying gravity
        animator.SetFloat("MoveSpeed", moveDirection.magnitude);

        moveDirection.y -= gravity * Time.fixedDeltaTime;

        Debug.Log(animator.GetFloat("MoveSpeed"));
        controller.Move(moveDirection * Time.fixedDeltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        xMove = Input.GetAxis("Horizontal");
        yMove = Input.GetAxis("Vertical");

        jump = Input.GetButton("Jump");

    }
}
