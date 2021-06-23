using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour
{
    private Animator animator;
    private bool displayOnce = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Body"))
        {
            GameManager.instance.isGameOver = true;
            GameManager.instance.gotKilledbyShark = true;
            //DOTween.KillAll();
            animator.SetTrigger("Idle");

            StartCoroutine(CallAfterAtime(1.5f));

            if (displayOnce)
            {
                AudioManger.instance.Play("ImpactSharkGirl");
                displayOnce = false;
            }
            other.transform.parent.GetComponent<Controller>().DoRagdoll(true);
            //other.transform.parent.transform.SetParent(this.transform, false);
        }
    }

    IEnumerator CallAfterAtime(float timer)
    {
       
        yield return new WaitForSeconds(timer);
        GameManager.instance.GameState(true);
    }
}
