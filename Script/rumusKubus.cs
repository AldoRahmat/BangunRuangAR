using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusKubus : MonoBehaviour
{
    public TMP_InputField input1; // Anggap ini sebagai "sisi"
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungVolumeKubus()
    {
        float sisi;

        bool sukses = float.TryParse(input1.text, out sisi);

        if (sukses)
        {
            float volume = sisi * sisi * sisi;
            hasilText.text = volume.ToString() + $" cm³";

            // Tampilkan rumus volume kubus
            rumusText.text = $"s × s × s<br>{sisi} × {sisi} × {sisi}";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}
