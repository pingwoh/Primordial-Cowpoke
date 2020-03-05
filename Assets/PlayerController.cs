using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public float speed;
    //public float jumpForce;

    private float moveInput;
    private Rigidbody2D rb { get { return GetComponent<Rigidbody2D>(); } }
    private Vector2 projectedVelocity;
    private CapsuleCollider2D playerCollider {get{ return GetComponent<CapsuleCollider2D>(); }}

    public bool isGrounded;
    private LayerMask whatIsGround;

    //Probably won't end up using this
    private int extraJumps;
    public int extraJumpsValue;

    //Bool that tracks if the player is currently jumping (moving upwards while pressing the space button
    //public bool jumping;
    //bool that tracks if the player was jumping in the previous frame
    public bool wasJumping = false;

    //will use this variable to measure if character has hit a cieling be measuring if the character has stopped moving upwards
    private float prevJumpVelocity;

    //want to record location player last jumped off of and return then whem they take damage

    private Animator characterAnim { get { return GetComponent<Animator>(); } }

    private float walkSpeed = 0;
    private float previousWalkSpeed = 0;
    private float walkMaxSpeed = 20f;
    private float walkAcceleration = 30f;
    private float walkDeceleration = 100f;

    private Vector3 projectedPosition;

    private float jumpSpeed = 0;
    private float jumpMaxSpeed = 20f;
    private float jumpAcceleration = 100f;
    private float jumpDeceleration = 50f;
    //float used to measure how long the jump button has been held down. Used later to allow for higher and shorter jumps based on how long space bar is pressed
    private float jumpTime = 0;
    //Jump window is the maximum amount of time in seconds the player can hold down the spacebar and still move upwards. ".2" probably works well for jump window
    private float jumpWindow = .33f;
    private bool canJump;
    private bool firstJump;

    private Dictionary<string, InputValues> inputDictionary = CustomInputs.inputDictionary;

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        //might replace whatIsGround
        whatIsGround = LayerMask.GetMask("Ground");
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, playerCollider.size.y/2 + .1f, whatIsGround);
        bool jumping = false;
        bool moving = false;

        if (isGrounded == false)
        {
            //characterAnim.SetBool("Grounded", false);
        }
        if (isGrounded == true)
        {
            //characterAnim.SetBool("Jumping", false);
            //characterAnim.SetBool("Grounded", true);
            extraJumps = extraJumpsValue;
            canJump = true;
        }
        //need to add extra for double jump
        if ((Input.GetKey(inputDictionary["Jump"].defaultKey) || Input.GetKey(inputDictionary["Jump"].customKey)) && canJump && firstJump)
        {
            jumping = true;
            jumpTime += Time.fixedDeltaTime;
        }

        //this if statement measures if character has hit a ceiling and cancles their jump
        if (jumpTime < jumpWindow && projectedVelocity.y > 0 && rb.velocity.y == 0)
        {
            jumpTime = jumpWindow;
        }

        if (jumpTime < jumpWindow && jumping)
        {
            jumpSpeed += (jumpAcceleration * Time.fixedDeltaTime);
        }
        else if (jumpTime >= jumpWindow && jumping)
        {
            jumpSpeed -= jumpDeceleration * Time.fixedDeltaTime;
            if(jumpSpeed < 0)
            {
                jumpTime = 0;
                canJump = false;
                firstJump = false;
                jumping = false;
            }
        }
        if (!Input.GetKey(inputDictionary["Jump"].defaultKey) && !Input.GetKey(inputDictionary["Jump"].customKey))
        {
            jumpTime = 0;
            canJump = false;
            firstJump = true;
            jumping = false;
        }

        if (!jumping && !isGrounded)
        {
            if (isGrounded)
            {
                jumpSpeed = 0;
            }
            else
            {
                jumpSpeed -= (jumpAcceleration * Time.fixedDeltaTime);
            }
        }
        if(Mathf.Abs(jumpSpeed) > jumpMaxSpeed)
        {
            jumpSpeed = jumpMaxSpeed * Mathf.Sign(jumpSpeed);
        }
        if(projectedVelocity.y != rb.velocity.y)
        {
            jumpSpeed = 0;
        }

        //transform.Translate(Vector3.up * jumpSpeed);

        if (Input.GetKey(inputDictionary["Right"].defaultKey) || Input.GetKey(inputDictionary["Right"].customKey))
        {
            //characterAnim.SetTrigger("Right");
            if(previousWalkSpeed < 0)
            {
                walkSpeed += walkDeceleration * Time.fixedDeltaTime;
            }
            else
            {
                walkSpeed += walkAcceleration * Time.fixedDeltaTime;
            }
            moving = true;
        }
        if (Input.GetKeyUp(inputDictionary["Right"].defaultKey) || Input.GetKeyUp(inputDictionary["Right"].customKey))
        {
            //characterAnim.ResetTrigger("Right");
            moving = false;
        }
        if (Input.GetKey(inputDictionary["Left"].defaultKey) || Input.GetKey(inputDictionary["Left"].customKey))
        {
            //characterAnim.SetTrigger("Left");
            if(previousWalkSpeed > 0)
            {
                walkSpeed -= walkDeceleration * Time.fixedDeltaTime;
            }
            else
            {
                walkSpeed -= walkAcceleration * Time.fixedDeltaTime;
            }
            moving = true;
        }
        if (Input.GetKeyUp(inputDictionary["Left"].defaultKey) || Input.GetKeyUp(inputDictionary["Left"].customKey))
        {
            //characterAnim.ResetTrigger("Left");
            moving = false;
        }
        if (Input.GetKey(inputDictionary["Right"].defaultKey) && Input.GetKey(inputDictionary["Left"].defaultKey) || Input.GetKey(inputDictionary["Right"].customKey) && Input.GetKey(inputDictionary["Left"].customKey))
        {
            //characterAnim.ResetTrigger("Right");
            //characterAnim.ResetTrigger("Left");
            moving = false;
            if (walkSpeed > 0)
            {
                //characterAnim.SetTrigger("Right");
            }
            if (walkSpeed < 0)
            {
                //characterAnim.SetTrigger("Left");
            }
        }

        if(Mathf.Abs(walkSpeed) > walkMaxSpeed)
        {
            if(walkSpeed - (walkDeceleration * Mathf.Sign(walkSpeed) * Time.fixedDeltaTime) <= walkMaxSpeed)
            {
                walkSpeed = walkMaxSpeed * Mathf.Sign(walkSpeed);
            }
            else
            {
                walkSpeed -= walkDeceleration * Mathf.Sign(walkSpeed) * Time.fixedDeltaTime;
            }
        }
        if (!moving)
        {
            walkSpeed -= walkDeceleration * Mathf.Sign(walkSpeed) * Time.fixedDeltaTime;
            if ((walkSpeed >= 0 && previousWalkSpeed <= 0) || (walkSpeed <= 0 && previousWalkSpeed >= 0))
            {
                walkSpeed = 0;
            }
        }

        previousWalkSpeed = walkSpeed;

        //Cancels movement if pusing agains a wall. Also checks if player is just moving up a slop or pushing against wall.
        //might stop working when I start using gravity
        if(Mathf.Abs(rb.velocity.x) / Mathf.Abs(projectedVelocity.x) < .5f)
        {
            //characterAnim.Bool("Push");
            walkSpeed = 0;
        }

        //moves the player
        //transform.Translate(Vector3.right * walkSpeed);
        rb.velocity = new Vector2(walkSpeed, jumpSpeed);


        if (walkSpeed != 0)
        {
            characterAnim.SetBool("Walking", true);
        }
        else
        {
            characterAnim.SetBool("Walking", false);

        }



        if (Input.GetKey(KeyCode.Space) && jumpTime <= jumpWindow && jumping == true)
        {
            characterAnim.SetBool("Jumping", true);
            //adds upwards velocity to the character for a jump and keeps the character's x velocity intact.
            //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpTime += Time.fixedDeltaTime;
        }
        else
        {
            characterAnim.SetBool("Jumping", false);
            jumping = false;
        }

        if(!jumping && wasJumping)
        {

        }

        wasJumping = jumping;
        projectedPosition = transform.position;
        projectedVelocity = rb.velocity;
    }
}
