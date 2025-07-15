using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Kembali : MonoBehaviour
{
    public void GoBack()
    {
        string role = PlayerPrefs.GetString("UserRole", "Murid");

        if (role == "Murid")
        {
            SceneManager.LoadScene("MainMenuMurid");
        }
        else
        {
            SceneManager.LoadScene("MainMenuGuru");
        }
    }
    public void Materi()
    {
        SceneManager.LoadScene("MateriNew");
    }
}
