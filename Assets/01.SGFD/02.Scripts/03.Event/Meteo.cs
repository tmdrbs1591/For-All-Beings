using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteo : MonoBehaviour
{
    [SerializeField] GameObject meteoColider;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyObj());
        StartCoroutine(ColiderSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    
    }

 
   
    IEnumerator DestroyObj()
    {
        yield return new WaitForSeconds(7);
        Destroy(gameObject);
    }
    
       
    IEnumerator ColiderSpawn()
    {
        yield return new WaitForSeconds(1.2f);
        meteoColider.SetActive(true);
    }
}
