using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private BoxCollider2D platform;
    private bool stay = false;
    private bool topHit;

    void Start()
    {
        platform = gameObject.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        //need to fix how player can sometimes not be able to jump off of platform
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.DownArrow) && stay == true)
        {
            platform.isTrigger = true;
            gameObject.layer = 9;//9th layer is "Inactive". Prevents player from jumping in platform while falling through
        }else if(stay == false)
        {
            platform.isTrigger = false;
            gameObject.layer = 9;//9th layer is "Inactive". Prevents player from jumping in platform while falling through
        }
        //Only runs when the player has connected with the top of the platform. Prevents jumping while in, but not on top of the platform.
        if (topHit == true)
        {
            platform.isTrigger = false;
            gameObject.layer = 8;//8th layer is "Ground". Allows player script to detect that the player is grounded and can jump
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        stay = true;
        topHit = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Returns the layer to "Inactive" when they player stops making contact
        stay = false;
        //This is just a precaution as occasionally the collisionStay won't register properly
        topHit = false;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        //Turns off the trigger function for the collider once the player has passed all the way throuh it, allowing the player to collide with it again.
        stay = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Detects when the player collides with the enabled points on the object collider (only the points on the top)
        //Essentially allows us to see when the player has officially landed on top of the platform without triggering while "inside" of the platform.
        topHit = collision.enabled;
    }
}
