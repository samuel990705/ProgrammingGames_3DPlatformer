using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Collide");
            CharacterControllerMove c = collision.GetComponent<CharacterControllerMove>();
            if (!c.onFire)//if not already on fire
                c.onFire=true;//set player onFire to true
        }
    }
}
