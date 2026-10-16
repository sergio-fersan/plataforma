using UnityEngine;

/// <summary>
/// Item colecionável (moeda, maçã, banana...).
/// Precisa de CircleCollider2D com IsTrigger = true.
/// Flutua + gira por código, então funciona mesmo sem Animator.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Coletavel : MonoBehaviour
{
    [Header("Pontos")]
    public int valor = 10;
    public string nomeItem = "Moeda";

    [Header("Animação por código")]
    public bool flutuar = true;
    public float alturaFlutuar = 0.15f;
    public float velocidadeFlutuar = 3f;

    private Vector3 posInicial;

    void Start()
    {
        posInicial = transform.position;
        // Garante que conta para a vitória
        GameManager.Instance?.RegistrarColetavel();
    }

    void Update()
    {
        if (flutuar)
        {
            float y = Mathf.Sin(Time.time * velocidadeFlutuar) * alturaFlutuar;
            transform.position = posInicial + new Vector3(0, y, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (!outro.CompareTag("Player")) return;

        GameManager.Instance?.ColetarItem(valor);
        Destroy(gameObject);
    }
}
