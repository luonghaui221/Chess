using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{
    public GameObject exitPanel;
    public void togglePanel()
    {
        if (exitPanel.activeInHierarchy)
        {
            Play();
        }
        else
        {
            Pause();
        }
        exitPanel.SetActive(!exitPanel.activeInHierarchy);
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }
    
    public void Play()
    {
        Time.timeScale = 1;
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
