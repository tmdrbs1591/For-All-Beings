using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollaborationButtonManager : MonoBehaviour
{

    [SerializeField] GameObject Button1;
    [SerializeField] GameObject Button2;

    [SerializeField] GameObject DoorButton;

    [SerializeField] Animator dooranim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var button1 = Button1.GetComponent<CollaborationButton>();
        var button2 = Button2.GetComponent<CollaborationButton>();

        var doorButton = DoorButton.GetComponent<DoorButton>();

        // 두 버튼이 모두 눌렸거나 doorButton이 눌려졌을 때 문을 열음
        if ((button1.buttonOn && button2.buttonOn) || doorButton.buttonOn)
        {
            dooranim.SetBool("isOpen", true);
        }
        else
        {
            dooranim.SetBool("isOpen", false);
        }
    }
}
