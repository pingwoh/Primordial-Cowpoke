using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetShot : MonoBehaviour
{
    private Animator vampAnimator { get { return GetComponent<Animator>(); } }
    private Animator playerAnimator { get { return FindObjectOfType<CowpokeController>().GetComponent<Animator>(); } }

    public void TriggerShot()
    {
        StartCoroutine(ShotTimer());
    }
    IEnumerator ShotTimer()
    {
        yield return new WaitForSeconds(.2f);
        playerAnimator.SetTrigger("Shoot");
        yield return new WaitForSeconds(.2f);
        vampAnimator.SetTrigger("Die");
    }
}
