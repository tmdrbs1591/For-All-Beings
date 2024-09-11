using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollaborationButton : MonoBehaviour
{
    Animator anim;

    public bool buttonOn;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("isButtonOn", true);
            buttonOn = true;
        }
    }

    private void OnTriggerExit(Collider other)  
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("isButtonOn", false);
            buttonOn =false;
        }
    }
}
