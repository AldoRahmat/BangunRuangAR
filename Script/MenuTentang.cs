using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTentang : MonoBehaviour
{
    public GameObject utama;     // GameObject panel utama (image/utama)
    public GameObject kampus;    // GameObject panel kampus
    public GameObject sd;        // GameObject panel SD

    public void GoToKampus()
    {
        utama.SetActive(false);
        kampus.SetActive(true);
        sd.SetActive(false);
    }

    public void GoToSD()
    {
        utama.SetActive(false);
        kampus.SetActive(false);
        sd.SetActive(true);
    }

    public void GoBack()
    {
        utama.SetActive(true);
        kampus.SetActive(false);
        sd.SetActive(false);
    }
}