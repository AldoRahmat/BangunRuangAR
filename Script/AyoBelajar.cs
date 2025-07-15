using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AyoBelajar : MonoBehaviour
{
    public void Materi()
    {
        SceneManager.LoadScene ("Materi");
    }

    public void BangunRuang3Dimensi()
    {
        SceneManager.LoadScene ("3D");
    }

    public void Kembali()
    {
        SceneManager.LoadScene ("MainMenuGuru");
        SceneManager.LoadScene ("MainMenuMurid");
        
    }
}
