using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControler : MonoBehaviour
{
    private AudioSource bombAudioSource; // 按钮音效

    void Start()
    {
        bombAudioSource = GetComponent<AudioSource>();
    }
    public void jumpToMainScene(string sceneName)
    {   
        bombAudioSource.Play();
        SceneManager.LoadScene(1);//1和下面的0表示场景的编号
    }
    
}
