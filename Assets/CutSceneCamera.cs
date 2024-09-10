using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BossSound()
    {
        AudioManager.instance.PlaySound(transform.position, 19, Random.Range(1f, 1f), 1f);

    }
}
