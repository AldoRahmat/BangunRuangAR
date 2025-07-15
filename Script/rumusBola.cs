using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rumusBola : MonoBehaviour
{
    public TMP_InputField inputJariJari; // Input jari-jari bola (r)
    public TextMeshProUGUI hasilText;
    public TextMeshProUGUI rumusText;

    public void HitungVolumeBola()
    {
        float jariJari;

        bool sukses = float.TryParse(inputJariJari.text, out jariJari);

        if (sukses)
        {
            float volume = (4f / 3f) * Mathf.PI * Mathf.Pow(jariJari, 3);
            hasilText.text = volume.ToString("F2") + $" cm³";

            // Tampilkan rumus
            rumusText.text = $"4/3 × π × r³<br>4/3 × 3.14 × {jariJari}³";
        }
        else
        {
            hasilText.text = "Input tidak valid!";
            rumusText.text = "";
        }
    }
}
