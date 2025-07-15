using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dimensi : MonoBehaviour
{
    public void Geser()
    {
        SceneManager.LoadScene("Drumus");
    }
    public void Keluar()
    {
        SceneManager.LoadScene("MenuAwal");
    }
    public void Jaring()
    {
        SceneManager.LoadScene("Jaring");
    }
    public void Rusuk()
    {
        SceneManager.LoadScene("Rusuk");
    }
}
