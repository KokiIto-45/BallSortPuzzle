using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public void onStartButton()
    {
        Debug.Log("Mainに遷移");
        SceneManager.LoadScene("PuzzleScene");
    }
}
