using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jaring : MonoBehaviour
{
    public void Geser()
    {
        SceneManager.LoadScene("JaringRumus");
    }
    public void Keluar()
    {
        SceneManager.LoadScene("MenuAwal");
    }
    public void Dimensi()
    {
        SceneManager.LoadScene("Dimensi");
    }
    public void Rusuk()
    {
        SceneManager.LoadScene("Rusuk");
    }
}
