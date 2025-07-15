using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusLuasKubus : MonoBehaviour
{
    public TMP_InputField inputSisi; // Input sisi kubus
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungLuasPermukaanKubus()
    {
        float sisi;

        bool sukses = float.TryParse(inputSisi.text, out sisi);

        if (sukses)
        {
            float luasPermukaan = 6 * sisi * sisi;
            hasilText.text = luasPermukaan.ToString("F2")  + $" cm²";

            // Tampilkan rumus
            rumusText.text = $"6 × s × s<br>6 × {sisi} x {sisi}";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}
