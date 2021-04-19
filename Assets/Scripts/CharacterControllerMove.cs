using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerMove : MonoBehaviour
{

    CharacterController controller;
    Animator animator;


    //variables related to regular character movement
    float speed = 8.0f;
    float jumpSpeed = 10.0f;
    float gravity = 25.0f;
    float groundedBuffer;//a timer for how long player has been "ungrounded". Used because sometimes player rotate uncontrollably when leaving ground for a split second
    Vector3 moveDirection = Vector3.zero;
    float xMove;
    float yMove;
    bool jump;
    Vector3 jumpDirection;//allows for some steering while player is in-air for a more fluid motion
    private float strafeSpeed = 5.0f;//speed of player in the air


    //variables used to set player on fire
    public bool onFire;//used to interact with campfire
    [SerializeField]GameObject fire;//particle effect used to spawn fire on head lol
    GameObject flame;//actual flame prefab that is spawned ontop of Player
    float flameOffset=1.3f;//y-offset of where the flame will be
    int burnTimer;//used to keep track of how long player is burning
    float burningSpeed=10f;//speed of player when player is on fire
    Vector3 burningMove;//equivalent to moveDirection but for when player is on fire


    //variables used to kick player
    bool kicked;//indicates if player was kicked by mob
    Vector3 kickMove;//direction player will be kicked to
    float kickDuration;//duration player is kicked for

 



    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        moveDirection = new Vector3();
        xMove = 0f;
        yMove = 0f;
        groundedBuffer = 0;
        kicked = false;
        burnTimer = -1;
        burningMove = new Vector3();
        kickDuration = 0;
    }

    private void FixedUpdate()
    {
        /*****************************
         *        Kicked by Mob      *
         *****************************/

        if (kicked)//if player got kicked by mob
        {
            
            kickDuration -= Time.fixedDeltaTime;//decrement kicktimer
            if (kickDuration <= 0)//stop being kickd once kick duration has elapsed
            {
                kicked = false;
            }

            // Face in direction of movement.
            if (Mathf.Abs(kickMove.x) > float.Epsilon || Mathf.Abs(kickMove.z) > float.Epsilon)
            {
                transform.rotation = Quaternion.LookRotation(new Vector3(kickMove.x, 0f, kickMove.z));
            }

            //gravity
            kickMove.y -= gravity * Time.fixedDeltaTime;

            controller.Move(kickMove);

        }

         /*****************************
         *        Player on Fire      *
         *****************************/

        if (onFire)//if player is currently on fire
        {
            animator.SetBool("Grounded", controller.isGrounded);//always running when on fire
            if (flame==null)//just caught on fire
            {
                //instantiate flame prefab
                flame = Instantiate(fire, new Vector3(this.transform.position.x, this.transform.position.y + flameOffset, this.transform.position.z), Quaternion.identity);
                burnTimer = 50 * 4;//FixedUpdate is called 50times per second, therefore burns for 4s
            }
            //makes flame follow Player
            flame.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + flameOffset, this.transform.position.z);

            if (burnTimer <= 0)//when timer runs out
            {
                onFire = false;//reset variables
                burnTimer = 0;
                Destroy(flame);
            }
            if (burnTimer % 60 == 0)//every 60 frames (3 times in 200 frames)
            {

                do//don't allow both x,y to be 0, or else player will just stand still
                {
                    burningMove.x = Random.Range(-1, 2);//go in random direction (every 60 frames)
                    burningMove.z = Random.Range(-1, 2);//random either -1, 0, or 1
                } while (burningMove.x == 0 && burningMove.z == 0);
            }
            if (burnTimer % 50 == 0)//make player jump every 2 seconds
            {
                burningMove.y = jumpSpeed*0.7f;//make character jump
            }

            //move player
            animator.SetFloat("RunAnimationSpeed", 5f);//speed up running animation speed

            // Face in direction of movement.
            if (Mathf.Abs(burningMove.x) > float.Epsilon || Mathf.Abs(burningMove.z) > float.Epsilon)
            {
                transform.rotation = Quaternion.LookRotation(new Vector3(burningMove.x, 0f, burningMove.z));
            }

            //gravity
            burningMove.y -= gravity * Time.fixedDeltaTime;

            controller.Move(burningMove * burningSpeed * Time.fixedDeltaTime);


            burnTimer--;//decremenet burnTimer
            return;//skip everything below
        }


        /********************************
         *        Regular Movement      *
         ********************************/

        animator.SetFloat("RunAnimationSpeed", 1f);


        animator.SetBool("Grounded", controller.isGrounded);//updated animation parameter
        groundedBuffer += Time.fixedDeltaTime;//keeps track of time player is not grounded
        if (controller.isGrounded)
        {
            groundedBuffer = 0;//set buffer to 0 if player is grounded

            // move direction directly from axes
            moveDirection = new Vector3(xMove, 0.0f, yMove);
            moveDirection *= speed;

            //set moveSpeed before appending y-movement
            animator.SetFloat("MoveSpeed", moveDirection.magnitude);

            if (jump)//if player jumps
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

        animator.SetFloat("GroundedBuffer", groundedBuffer);//update animation parameter

        // Face in direction of movement.
        if (Mathf.Abs(moveDirection.x) > float.Epsilon || Mathf.Abs(moveDirection.z) > float.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
        }

        //implement gravity
        moveDirection.y -= gravity * Time.fixedDeltaTime;

        //move character in moveDirection
        controller.Move(moveDirection * Time.fixedDeltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        xMove = Input.GetAxis("Horizontal");
        yMove = Input.GetAxis("Vertical");

        jump = Input.GetButton("Jump");


        if (this.transform.position.y <=-4f)//player have fallen off the map
        {
            this.transform.position = new Vector3(1.805f, 4f, -6.66f);//return to spawn
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Agent")
        {

            //set magnitude and direction of kick
            kickMove = (this.transform.position - collision.transform.position);//direction to be kicked to
            kickMove.y = 0;//disregard y position of contact
            kickMove = kickMove.normalized * 0.5f;
            kickMove.y = jumpSpeed*0.2f;

            kicked = true;
            kickDuration = 0.3f;
        }
    }
}
