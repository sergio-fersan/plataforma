using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Vai na CenaFinal. Mostra VITÓRIA ou DERROTA + pontos + botão reiniciar.
/// Crie: TxtResultado, TxtPontosFinais e BtnReiniciar (Button).
/// </summary>
public class TelaFinal : MonoBehaviour
{
    public TMP_Text txtResultado;
    public TMP_Text txtPontosFinais;
    public Button btnReiniciar;

    void Start()
    {
        if (txtResultado != null)
        {
            txtResultado.text = GameManager.UltimaVitoria ? "VITÓRIA!" : "DERROTA";
            txtResultado.color = GameManager.UltimaVitoria ? Color.green : Color.red;
        }

        if (txtPontosFinais != null)
            txtPontosFinais.text = "Pontos: " + GameManager.PontosFinais;

        if (btnReiniciar != null)
            btnReiniciar.onClick.AddListener(() =>
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.ReiniciarJogo();
                else
                    SceneManager.LoadScene("Fase1");
            });
    }
}
