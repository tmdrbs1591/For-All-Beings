using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialChar : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TutorialPoint"))
        {
            anim.SetBool("isWalk", false);
            TutorialManager.instance.MoveTutorial();
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("EventBox"))
        {
            Debug.Log("asdasd");
            TutorialManager.instance.EventTutorial();
            Destroy(other.gameObject);
        }

    }

}
