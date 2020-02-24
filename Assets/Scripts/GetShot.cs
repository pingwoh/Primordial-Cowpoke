using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetShot : MonoBehaviour
{
    private Animator vampAnimator { get { return GetComponent<Animator>(); } }
    private Animator playerAnimator { get { return FindObjectOfType<CowpokeController>().GetComponent<Animator>(); } }

    public AudioManager audioManager { get { return FindObjectOfType<AudioManager>(); } }

    public void TriggerShot()
    {
        StartCoroutine(ShotTimer());
        audioManager.PlayAudio("GUNSHOT");
    }
    IEnumerator ShotTimer()
    {
        yield return new WaitForSeconds(.2f);
        playerAnimator.SetTrigger("Shoot");
        yield return new WaitForSeconds(.2f);
        vampAnimator.SetTrigger("Die");
    }
}
