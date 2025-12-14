using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManaging : MonoBehaviour
{
    public string nextScene;

    public void UploadScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
