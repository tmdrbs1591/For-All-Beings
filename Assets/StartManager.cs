using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    [SerializeField] GameObject Fadein;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            StartCoroutine(MenuScene());
        }
    }
    IEnumerator MenuScene()
    {
        Fadein.SetActive(true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Menu");
    }
}
