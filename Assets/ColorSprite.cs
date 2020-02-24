using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer { get { return GetComponent<SpriteRenderer>(); } }

    public void MakeGreen()
    {
        spriteRenderer.color = Color.green;
    }

    public void MakeRed()
    {
        spriteRenderer.color = Color.red;
    }
}
