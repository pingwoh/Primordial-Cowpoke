using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingEvents : MonoBehaviour
{

    public GameObject BloodlustAnimator;
    public GameObject NeutralAnimator;
    public GameObject VampKillAnimator;

   public void BloodlustEnd()
    {
        BloodlustAnimator.gameObject.SetActive(true);
    }

    public void NeutralEnd()
    {
        NeutralAnimator.gameObject.SetActive(true);
    }

    public void VampKillEnd()
    {
        VampKillAnimator.gameObject.SetActive(true);
    }
}
