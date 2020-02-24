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

    public SpriteRenderer characterSprite;

    // Start is called before the first frame update
    void Start()
    {
        groundCheck = true;
        hasKey = false;
        endState = false;
    }

    // Update is called once per frame
    void Update()
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
            GetComponent<Rigidbody2D>().AddForce(Vector2.right * 20);
            if (characterSprite != null)
            {
                characterSprite.flipX = true;
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.left * 20);
            if (characterSprite != null)
            {
                characterSprite.flipX = false;
            }
        }
        else if (GetComponent<Rigidbody2D>().velocity.y == 0)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
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
