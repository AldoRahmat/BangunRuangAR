using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusBalok : MonoBehaviour
{
    public TMP_InputField input1; // Panjang
    public TMP_InputField input2; // Lebar
    public TMP_InputField input3; // Tinggi
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungVolumeBalok()
    {
        float panjang, lebar, tinggi;

        bool sukses1 = float.TryParse(input1.text, out panjang);
        bool sukses2 = float.TryParse(input2.text, out lebar);
        bool sukses3 = float.TryParse(input3.text, out tinggi);

        if (sukses1 && sukses2 && sukses3)
        {
            float volume = panjang * lebar * tinggi;
            hasilText.text = volume.ToString() + $" cm³";

            // Menampilkan rumus
            rumusText.text = $"p × l × t<br>{panjang} × {lebar} × {tinggi}";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}
