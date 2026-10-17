using UnityEngine;

/// <summary>
/// Item colecionável (moeda, maçã, banana...). Precisa de um Collider2D com IsTrigger.
/// O visual (filho) flutua; o collider fica parado.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Coletavel : MonoBehaviour
{
    public int valor = GameManager.PontosMoeda;
    public Transform visual;
    public float alturaFlutuar = 0.12f;
    public float velocidadeFlutuar = 3f;

    private bool coletado;
    private float fase;

    void Start()
    {
        fase = Random.value * 6.28f;
    }

    void Update()
    {
        if (visual == null) return;
        visual.localPosition = new Vector3(0f, Mathf.Sin(Time.time * velocidadeFlutuar + fase) * alturaFlutuar, 0f);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (coletado) return;
        if (outro.GetComponent<PlayerController2D>() == null) return;

        coletado = true;
        GameManager gm = GameManager.Garantir();
        gm.ColetarMoeda(valor);
        Destroy(gameObject);
    }
}
