using UnityEngine;

/// <summary>
/// Fundo com parallax infinito. Cada camada é uma faixa de sprite repetido (Tiled).
/// Fator perto de 0 = longe (quase parado); perto de 1 = acompanha a câmera.
/// </summary>
[DefaultExecutionOrder(100)]
public class Parallax : MonoBehaviour
{
    [System.Serializable]
    public class Camada
    {
        public Transform fundo;
        public float fatorX = 0.3f;
        public float fatorY = 0.3f;
        public float largura = 16f;   // largura de uma repetição, em unidades do mundo
        public float yBase = 0f;      // y do centro da faixa quando a câmera está na posição inicial
        public bool seguirCameraEmY = false;
    }

    public Camada[] camadas;
    public Transform cameraAlvo;
    public float derivaAutomatica = 0f;   // unidades/segundo (usado no menu)

    private float refY;
    private float deriva;
    private bool iniciado;

    void Start()
    {
        Iniciar();
    }

    void Iniciar()
    {
        if (iniciado) return;
        if (cameraAlvo == null && Camera.main != null) cameraAlvo = Camera.main.transform;
        if (cameraAlvo != null) refY = cameraAlvo.position.y;
        iniciado = true;
    }

    void LateUpdate()
    {
        Iniciar();
        if (cameraAlvo == null || camadas == null) return;

        deriva += derivaAutomatica * Time.deltaTime;
        float camX = cameraAlvo.position.x;
        float camY = cameraAlvo.position.y;
        float virtualX = camX + deriva;

        for (int i = 0; i < camadas.Length; i++)
        {
            Camada c = camadas[i];
            if (c.fundo == null) continue;

            float L = Mathf.Max(0.01f, c.largura);
            float deslocado = c.fatorX * virtualX;
            float x = deslocado + L * Mathf.Round((camX - deslocado) / L);
            float y = c.seguirCameraEmY ? camY : c.yBase + (camY - refY) * c.fatorY;
            c.fundo.position = new Vector3(x, y, c.fundo.position.z);
        }
    }
}
