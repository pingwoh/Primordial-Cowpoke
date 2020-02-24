using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CowpokeController : MonoBehaviour
{
    public int health;

    public bool hasKey;
    bool groundCheck;
    bool endState;
    public GameObject Barricade;
    public GameObject Sheriff;

    public Text healthText;
    public Text coins;

    public GameObject teleporterOne;
    public GameObject teleporterTwo;

    private SpriteRenderer characterSprite {get { return GetComponent<SpriteRenderer>(); } }

    private Animator playerAnimator {get { return GetComponent<Animator>(); } }

    private Rigidbody2D playerRB {get { return GetComponent<Rigidbody2D>(); } }

    private int movementSpeed = 15;
    private int maxMovementSpeed = 15;

    private bool firstPress = true;

    private void Awake()
    {
        playerAnimator.SetTrigger("Asleep");
    }

    // Start is called before the first frame update
    void Start()
    {
        groundCheck = true;
        hasKey = false;
        endState = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (firstPress)
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                playerAnimator.SetTrigger("WakeUp");
                //starts timer to let player move.
                StartCoroutine(WakeUp());
            }

        }
        //won't play unless the player has fully woken up
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (groundCheck == true)
                {
                    GetComponent<Rigidbody2D>().AddForce(Vector2.up * 300);
                }
                groundCheck = false;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                GetComponent<Rigidbody2D>().AddForce(Vector2.right * 100);

                playerAnimator.SetBool("Walking", true);
                playerAnimator.SetBool("Sitting", false);

                if (characterSprite != null)
                {
                    characterSprite.flipX = true;
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                GetComponent<Rigidbody2D>().AddForce(Vector2.left * 100);

                playerAnimator.SetBool("Walking", true);
                playerAnimator.SetBool("Sitting", false);

                if (characterSprite != null)
                {
                    characterSprite.flipX = false;
                }
            }
            else
            {
                playerAnimator.SetBool("Walking", false);
                playerAnimator.SetBool("Sitting", false);
            }
        }

        //limits maximum movement speed
        if(Mathf.Abs(playerRB.velocity.x) > maxMovementSpeed)
        {
            playerRB.velocity = new Vector2(Mathf.Sign(playerRB.velocity.x) * maxMovementSpeed, playerRB.velocity.y);
        }

        /*if (GetComponent<Rigidbody2D>().velocity.y == 0)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }*/
    }
    private IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(1.5f);
        firstPress = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Sheriff"))
        {
            hasKey = true;
            Destroy(other.gameObject);
            Instantiate(Resources.Load("Particle System"), other.transform.position, Quaternion.identity);

        }
        if (other.CompareTag("Barricade") && hasKey == true)
        {
            Destroy(other.gameObject);
            Instantiate(Resources.Load("ChestSound"), other.transform.position, Quaternion.identity);
            endState = true;
        }
        if (other.CompareTag("Light"))
        {
            Instantiate(Resources.Load("sunlightEffect"), other.transform.position, Quaternion.identity);
            Debug.Log("AAAH OUCH THE SUN");

        }
       
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Teleport") && Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.position = teleporterTwo.transform.position;
        }
        if (other.CompareTag("Teleport2") && Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.position = teleporterOne.transform.position;
        }
        //if (other.CompareTag("EXIT") && Input.GetKeyDown(KeyCode.E) && (endState == true))
       // {
       //     SceneManager.LoadScene(1);
       // }
        if (other.CompareTag("Ground"))
        {
            groundCheck = true;
        }
        if (other.CompareTag("Light"))
        {
          
        }
    }
}
