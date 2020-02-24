using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CowpokeController : MonoBehaviour
{
    bool hasKey;
    bool groundCheck;

    public GameObject Barricade;
    public GameObject Sheriff;
    public GameObject Light;

    public GameObject fadeAnimator;

    public GameObject levelOneLeave;
    public GameObject levelOneReturn;
    public GameObject levelOne;
    public GameObject levelTwoLeave;
    public GameObject levelTwoReturn;
    public GameObject levelThree;

    private SpriteRenderer characterSprite { get { return GetComponent<SpriteRenderer>(); } }

    private Animator playerAnimator { get { return GetComponent<Animator>(); } }

    private Rigidbody2D playerRB { get { return GetComponent<Rigidbody2D>(); } }

    private int movementSpeed = 100;
    private int maxMovementSpeed = 15;

    private bool firstPress = true;
    public bool canMove = true;

    private void Awake()
    {
        playerAnimator.SetTrigger("Asleep");
    }

    // Start is called before the first frame update
    void Start()
    {
        groundCheck = true;
        hasKey = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (firstPress)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                playerAnimator.SetTrigger("WakeUp");
                //starts timer to let player move.
                StartCoroutine(WakeUp());
            }
        }
        //won't play unless the player has fully woken up
        else
        {
            if (canMove)
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
                    GetComponent<Rigidbody2D>().AddForce(Vector2.right * movementSpeed);

                    playerAnimator.SetBool("Walking", true);
                    playerAnimator.SetBool("Sitting", false);

                    if (characterSprite != null)
                    {
                        characterSprite.flipX = true;
                    }
                }
                else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    GetComponent<Rigidbody2D>().AddForce(Vector2.left * movementSpeed);

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
                if (Input.GetKey(KeyCode.Q))
                {
                    playerAnimator.SetBool("Sunny", true);
                }
                else
                {
                    playerAnimator.SetBool("Sunny", false);

                }
            }
            else
            {
                playerAnimator.SetBool("Walking", false);
                playerAnimator.SetBool("Sitting", false);
                playerAnimator.SetBool("Sunny", false);
            }
        }

        //limits maximum movement speed
        if (Mathf.Abs(playerRB.velocity.x) > maxMovementSpeed)
        {
            playerRB.velocity = new Vector2(Mathf.Sign(playerRB.velocity.x) * maxMovementSpeed, playerRB.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("You closed the application.");
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
        if (other.CompareTag("Light"))
        {
            if (playerAnimator.GetBool("Sunny") == false)
            {
                Instantiate(Resources.Load("sunlightEffect"), other.transform.position, Quaternion.identity);
            }
            Debug.Log("AAAH OUCH THE SUN");
            Instantiate(Resources.Load("SizzleSound"), other.transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Light"))
        {
            Destroy(GameObject.FindGameObjectWithTag("Sizzle"));
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            groundCheck = true;
        }
        if (other.CompareTag("Light"))
        {
            if (playerAnimator.GetBool("Sunny") == true)
            {
                Light.GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                Light.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
        if (other.CompareTag("Sheriff"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                hasKey = true;
                Instantiate(Resources.Load("KeySound"), other.transform.position, Quaternion.identity);
            }
        }


        if (other.CompareTag("LevelOneLeave") && Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.position = levelOneReturn.transform.position;
        }
        if (other.CompareTag("LevelOneReturn") && Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.position = levelOne.transform.position;
        }
        if (other.CompareTag("LevelTwoLeave") && Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.position = levelTwoReturn.transform.position;
        }
        if (other.CompareTag("LevelTwoReturn") && Input.GetKeyDown(KeyCode.E))
        {
            gameObject.transform.position = levelTwoLeave.transform.position;
        }
        if (other.CompareTag("Barricade") && Input.GetKeyDown(KeyCode.E) && hasKey == true)
        {
            Destroy(other.gameObject);
            fadeAnimator.gameObject.SetActive(true);
        }
    }
}
