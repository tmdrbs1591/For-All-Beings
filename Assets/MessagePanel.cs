using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(False());
    }

    IEnumerator False()
    {
        yield return new WaitForSeconds(4);
        gameObject.SetActive(false);
    }
}
