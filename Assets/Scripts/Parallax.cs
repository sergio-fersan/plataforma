using UnityEngine;

/// <summary>
/// Fundo com parallax: arraste as camadas do fundo aqui.
/// Quanto menor o fator, mais longe (move menos).
/// Ex: céu 0.1, montanhas 0.3, árvores 0.6
/// </summary>
public class Parallax : MonoBehaviour
{
    [System.Serializable]
    public class Camada
    {
        public Transform fundo;
        public float fator = 0.3f;
    }

    public Camada[] camadas;
    public Transform cameraAlvo;

    private Vector3 ultimaPosCamera;

    void Start()
    {
        if (cameraAlvo == null && Camera.main != null)
            cameraAlvo = Camera.main.transform;
        ultimaPosCamera = cameraAlvo.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cameraAlvo.position - ultimaPosCamera;

        foreach (var c in camadas)
        {
            if (c.fundo == null) continue;
            c.fundo.position += new Vector3(delta.x * c.fator, delta.y * c.fator, 0);
        }

        ultimaPosCamera = cameraAlvo.position;
    }
}
