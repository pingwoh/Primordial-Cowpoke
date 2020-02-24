using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothTime = .1f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        transform.position = player.position + new Vector3(0, 0, -10);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition = player.position + new Vector3(0, 0, -10);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
