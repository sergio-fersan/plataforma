using UnityEngine;

/// <summary>Câmera que segue o jogador com suavidade.</summary>
public class CameraSegue : MonoBehaviour
{
    public Transform jogador;
    public float suavidade = 5f;
    public Vector3 deslocamento = new Vector3(0, 1f, -10f);
    public float limiteEsquerdo = -999f;

    void LateUpdate()
    {
        if (jogador == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) jogador = p.transform;
            else return;
        }

        Vector3 alvo = jogador.position + deslocamento;
        if (alvo.x < limiteEsquerdo) alvo.x = limiteEsquerdo;
        transform.position = Vector3.Lerp(transform.position, alvo, suavidade * Time.deltaTime);
    }
}
