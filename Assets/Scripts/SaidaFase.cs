using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Porta/bandeira de saída da fase. Precisa de BoxCollider2D com IsTrigger.
/// Se ainda faltar coletável, mostra aviso e não deixa passar.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SaidaFase : MonoBehaviour
{
    public string proximaCena = "Fase2";

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (!outro.CompareTag("Player")) return;

        var gm = GameManager.Instance;
        if (gm != null && gm.totalColetaveisFase > 0 && gm.coletadosFase < gm.totalColetaveisFase)
        {
            Debug.Log("Ainda faltam " + (gm.totalColetaveisFase - gm.coletadosFase) + " itens!");
            return;
        }

        SceneManager.LoadScene(proximaCena);
    }
}
