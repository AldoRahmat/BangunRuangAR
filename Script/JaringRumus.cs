using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JaringRumus : MonoBehaviour
{
    public void Geser()
    {
        SceneManager.LoadScene("Jaring");
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
