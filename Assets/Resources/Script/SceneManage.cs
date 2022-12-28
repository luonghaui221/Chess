using System.Collections;
using System.Collections.Generic;
using Resources.Script;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneManage : MonoBehaviour
{
    // Start is called before the first frame update
    public void PLayGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame(){
        Application.Quit();
    }
    
}
