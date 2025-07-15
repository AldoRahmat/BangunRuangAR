using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusTabung : MonoBehaviour
{
    public TMP_InputField inputJariJari; // r
    public TMP_InputField inputTinggi;   // t
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungVolumeTabung()
    {
        float r, t;

        bool sukses1 = float.TryParse(inputJariJari.text, out r);
        bool sukses2 = float.TryParse(inputTinggi.text, out t);

        if (sukses1 && sukses2)
        {
            float volume = Mathf.PI * r * r * t;
            hasilText.text = volume.ToString("F2") + $" cm³";

            // Menampilkan rumus
            rumusText.text = $"(π × r²) × t<br>3.14 × {r}² × {t}";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}
