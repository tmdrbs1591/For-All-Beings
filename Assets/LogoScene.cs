using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SceneCor());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator SceneCor()
    {
        yield return new WaitForSeconds(1f);
        SingleAudioManager.instance.PlaySound(transform.position, 0, UnityEngine.Random.Range(1f, 1f), 0.5f);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Title");
    }
}
