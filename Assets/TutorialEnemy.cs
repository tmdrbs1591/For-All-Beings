using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        TutorialManager.instance.monsterkillCount++;
        TutorialManager.instance.questText.text = "모든 적 처치하기 " + TutorialManager.instance.monsterkillCount + "/ 4";
    }
}
