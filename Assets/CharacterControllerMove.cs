using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerMove : MonoBehaviour
{

    CharacterController controller;
    Animator animator;

    float speed = 8.0f;
    float jumpSpeed = 10.0f;
    float gravity = 25.0f;

    float groundedBuffer;//a timer for how long player has been "ungrounded". Used because sometimes player rotate uncontrollably when leaving ground for a split second


    Vector3 moveDirection = Vector3.zero;
    float xMove;
    float yMove;
    bool jump;

    //allows for some steering while player is in-air for a more fluid motion
    Vector3 jumpDirection;
    private float strafeSpeed = 5.0f;//speed of player in the air

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        moveDirection = new Vector3();
        xMove = 0f;
        yMove = 0f;
        groundedBuffer = 0;
    }

    private void FixedUpdate()
    {

        animator.SetBool("Grounded", controller.isGrounded);
        groundedBuffer += Time.fixedDeltaTime;
        if (controller.isGrounded)
        {
            groundedBuffer = 0;
            // We are grounded, so recalculate
            // move direction directly from axes
            moveDirection = new Vector3(xMove, 0.0f, yMove);
            moveDirection *= speed;

            //set moveSpeed before appending y-movement
            animator.SetFloat("MoveSpeed", moveDirection.magnitude);

            if (jump)
            {
                jumpDirection = 1/speed*moveDirection;//record direction when play jumps
                moveDirection.y = jumpSpeed;
            }


        }
        if(!controller.isGrounded && groundedBuffer>0.1f)//player is in the air for longer than 0.1s
        {
            //allows for some steering in air (leave y-direction unchanged)
            moveDirection = new Vector3(speed*jumpDirection.x+(xMove-jumpDirection.x)*strafeSpeed, moveDirection.y, speed * jumpDirection.z + (yMove-jumpDirection.z) * strafeSpeed);
        }

        animator.SetFloat("GroundedBuffer", groundedBuffer);

        // Face in direction of movement.
        if (Mathf.Abs(moveDirection.x) > float.Epsilon || Mathf.Abs(moveDirection.z) > float.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
        }

        moveDirection.y -= gravity * Time.fixedDeltaTime;

        
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
