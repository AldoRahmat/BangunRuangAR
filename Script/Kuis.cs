using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Kuis : MonoBehaviour
{
    public void Kembali()
    {
        SceneManager.LoadScene ("Soal");
    }
}
