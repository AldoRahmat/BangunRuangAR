using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusKerucut : MonoBehaviour
{
    public TMP_InputField inputJariJari; // r
    public TMP_InputField inputTinggi;   // t
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungVolumeKerucut()
    {
        float r, t;

        bool sukses1 = float.TryParse(inputJariJari.text, out r);
        bool sukses2 = float.TryParse(inputTinggi.text, out t);

        if (sukses1 && sukses2)
        {
            float volume = (1f / 3f) * Mathf.PI * r * r * t;
            hasilText.text = volume.ToString("F2") + $" cm³";

            // Menampilkan rumus
            rumusText.text = $"1/3 × π × r² × t<br>1/3 × 3.14 × {r}² × {t} ";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}
