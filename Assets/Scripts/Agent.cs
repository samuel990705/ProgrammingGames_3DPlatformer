using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    Animator animator;

    float speed;//estimates speed of agent for animation purposes;
    Vector3 lastPosition;//used to estimate speed

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        speed = 0;
        lastPosition = new Vector3();
        animator.SetBool("CollidedPlayer", false);
    }

    // Update is called once per frame
    void Update()
    {
        //estimates the speed of agent (distaince traveled over time)
        speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = this.transform.position;
        animator.SetFloat("MoveSpeed", speed);



        //if flip animation has played once and agent was in flipping animation (0.75f is animation time for 1 flip, and the flip animation has tagHash 986467179)
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75f && animator.GetCurrentAnimatorStateInfo(0).tagHash== 986467179)
        {
            animator.SetBool("CollidedPlayer", false);
        }
        
        agent.SetDestination(player.transform.position);//makes agent follow player
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            animator.SetBool("CollidedPlayer", true);//set animation parameter (do flip)
        }
    }
}
