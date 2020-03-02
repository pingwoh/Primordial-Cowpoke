using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardGun : MonoBehaviour
{
    private Camera cam { get{ return FindObjectOfType<Camera>(); } }
    private float movementSpeed = 5;


    private Dictionary<string, InputValues> inputDictionary = CustomInputs.inputDictionary;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(inputDictionary["Right"].defaultKey) || Input.GetKey(inputDictionary["Right"].customKey))
        {
            transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(inputDictionary["Left"].defaultKey) || Input.GetKey(inputDictionary["Left"].customKey))
        {
            transform.Translate(Vector3.right * -movementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(inputDictionary["Up"].defaultKey) || Input.GetKey(inputDictionary["Up"].customKey))
        {
            transform.Translate(Vector3.up * movementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(inputDictionary["Down"].defaultKey) || Input.GetKey(inputDictionary["Down"].customKey))
        {
            transform.Translate(Vector3.up * -movementSpeed * Time.deltaTime);
        }
        Debug.DrawLine(new Vector3(transform.position.x - .5f, transform.position.y, transform.position.z), new Vector3(transform.position.x + .5f, transform.position.y, transform.position.z), Color.red);
        if (Input.GetKey(inputDictionary["Interact"].defaultKey) || Input.GetKey(inputDictionary["Interact"].customKey))
        {
            int layerMask = 1 << 9;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, .5f, Vector2.zero, 0, layerMask);
            foreach(RaycastHit2D hit in hits)
            {
                GameObject hitObject = hit.transform.gameObject;
                if(hitObject.tag == "EnemyBody")
                {
                    Debug.Log("Hit Enemy Body " + hitObject.name);
                }
            }
            /*
            Vector2 screenPos = cam.WorldToScreenPoint(transform.position);
            //Ray ray = cam.ScreenPointToRay(screenPos);
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(screenPos), Vector2.zero, Mathf.Infinity, layerMask);
            if(hit)
            {
                GameObject hitObject = hit.transform.gameObject;
                if(hitObject.tag == "EnemyBody")
                {
                    Debug.Log("Hit Enemy Body");
                }
            }
            */
        }

    }
}
