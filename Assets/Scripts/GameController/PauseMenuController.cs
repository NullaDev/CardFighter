using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject pauseMenuPanel;
    public Button pauseButton;

    public bool isPaused = false;

    void Start()
    {
        pauseButton.onClick.AddListener(TogglePauseMenu);
        pauseMenuPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        Debug.Log("Press pause");
        isPaused = !isPaused;

        if(isPaused)
        {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;  // 回合制游戏进程怎么算的不知道,先这样吧,主程什么时候来点技术分享
            Debug.Log("do pause");
        }
        else
        {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log("do resume");
        }
    }

    public void ResumeGame()
    {
        Debug.Log("resume game");
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Debug.Log("restart, nothing happened now");
        isPaused = false;
        pauseMenuPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        # else
        Application.Quit();
        #endif
    }

    public void OpenSettings()
    {
        Debug.Log("open settings");
    }

}
