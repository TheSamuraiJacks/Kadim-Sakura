using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUı : MonoBehaviour
{
    public GameObject GameoverPanel;
    public GameObject[] gameObjectsToDisactive;


    private void OnEnable()
    {
        ShowGameOver();
    }
    public void ShowGameOver()
    {
        Time.timeScale = 0f; // Oyunu durdur
        foreach(GameObject go in gameObjectsToDisactive)
        {
            Destroy(go);
        }
        GameoverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void Respawn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
