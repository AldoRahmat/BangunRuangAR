using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusLuasTabung : MonoBehaviour
{
    public TMP_InputField inputJariJari; // r
    public TMP_InputField inputTinggi;   // t
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungLuasPermukaanTabung()
    {
        float r, t;

        bool sukses1 = float.TryParse(inputJariJari.text, out r);
        bool sukses2 = float.TryParse(inputTinggi.text, out t);

        if (sukses1 && sukses2)
        {
            float luas = 2 * Mathf.PI * r * (r + t);
            hasilText.text = luas.ToString("F2") + $" cm²";

            // Tampilkan rumus
            rumusText.text = $"2 × π × r × (r + t)<br>2 × 3.14 × {r} × ({r} + {t})";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}
