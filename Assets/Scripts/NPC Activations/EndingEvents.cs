using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingEvents : MonoBehaviour
{

    public GameObject BloodlustAnimator;
    public GameObject NeutralAnimator;
    public GameObject VampKillAnimator;
    public GameObject fadeAnimator;
    public GameObject BackgroundMusic;

   public void BloodlustEnd()
    {
        fadeAnimator.gameObject.SetActive(true);
        BloodlustAnimator.gameObject.SetActive(true);
        Destroy(BackgroundMusic);
        Instantiate(Resources.Load("VampireEndMusic"), transform.position, Quaternion.identity);
    }

    public void NeutralEnd()
    {
        fadeAnimator.gameObject.SetActive(true);
        NeutralAnimator.gameObject.SetActive(true);
        Destroy(BackgroundMusic);
        Instantiate(Resources.Load("NeutralEndMusic"), transform.position, Quaternion.identity);
    }

    public void VampKillEnd()
    {
        fadeAnimator.gameObject.SetActive(true);
        VampKillAnimator.gameObject.SetActive(true);
        Destroy(BackgroundMusic);
        Instantiate(Resources.Load("VillageEndMusic"), transform.position, Quaternion.identity);
    }
}
