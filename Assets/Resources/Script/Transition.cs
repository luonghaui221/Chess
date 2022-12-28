using System;
using System.Collections;
using System.Collections.Generic;
using Resources.Script;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public Animator animator;
    public float transitionDelayTime = 1.0f;
    
    
    public void EndScene(bool modePve)
    {
        GlobalStorage.modePve = modePve;
        gameObject.SetActive(true);
        StartCoroutine(DelayStart());
        SceneManager.LoadScene(1);
    }

    public void StartScene()
    {
        StartCoroutine(DelayStart());
        gameObject.SetActive(false);
    }

    IEnumerator DelayEnd()
    {
        animator.SetTrigger("TriggerTransition");
        yield return new WaitForSeconds(transitionDelayTime);
    }
    
    IEnumerator DelayStart()
    {
        animator.SetTrigger("TriggerTransition");
        yield return new WaitForSeconds(transitionDelayTime);
    }
}
