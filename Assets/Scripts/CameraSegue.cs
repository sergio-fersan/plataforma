using UnityEngine;

/// <summary>Câmera que segue o jogador com suavidade e respeita os limites da fase.</summary>
[RequireComponent(typeof(Camera))]
public class CameraSegue : MonoBehaviour
{
    public Transform jogador;
    public float suavidade = 0.15f;
    public Vector2 deslocamento = new Vector2(0f, 1.5f);

    public bool limitar = true;
    public float minX = 0f;
    public float maxX = 100f;
    public float minY = -2f;

    private Camera cam;
    private Vector3 velocidade;

    Vector3 CalcularAlvo()
    {
        if (cam == null) cam = GetComponent<Camera>();
        float meioAlt = cam.orthographicSize;
        float meioLarg = meioAlt * cam.aspect;

        Vector3 alvo = new Vector3(jogador.position.x + deslocamento.x, jogador.position.y + deslocamento.y, -10f);
        if (limitar)
        {
            float xmin = minX + meioLarg;
            float xmax = maxX - meioLarg;
            alvo.x = xmax < xmin ? (minX + maxX) * 0.5f : Mathf.Clamp(alvo.x, xmin, xmax);
            alvo.y = Mathf.Max(alvo.y, minY + meioAlt);
        }
        return alvo;
    }

    /// <summary>Coloca a câmera direto na posição do jogador (sem suavizar).</summary>
    public void Posicionar()
    {
        if (jogador == null) return;
        transform.position = CalcularAlvo();
        velocidade = Vector3.zero;
    }

    void LateUpdate()
    {
        if (jogador == null) return;
        transform.position = Vector3.SmoothDamp(transform.position, CalcularAlvo(), ref velocidade, suavidade);
    }
}
