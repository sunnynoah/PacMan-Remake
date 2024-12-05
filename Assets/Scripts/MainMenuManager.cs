using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartClicked()
    {
        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.SetInt("lives", 3);
        PlayerPrefs.SetInt("score", 0);
        SceneManager.LoadScene("Game");
    }

    public void QuitClicked()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }
}
