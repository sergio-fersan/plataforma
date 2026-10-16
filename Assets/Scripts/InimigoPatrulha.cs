using UnityEngine;

/// <summary>
/// Inimigo que anda de um lado pro outro.
/// Se o jogador cair em cima dele, o inimigo morre e o jogador quica.
/// Se encostar pelo lado/baixo, o jogador leva dano.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class InimigoPatrulha : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform pontoA;
    public Transform pontoB;
    public float velocidade = 2f;

    [Header("Animação (opcional)")]
    public Animator anim;

    private Vector3 alvo;
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (anim == null) anim = GetComponent<Animator>();
        alvo = pontoB != null ? pontoB.position : transform.position + Vector3.right * 3f;
        if (pontoA == null)
        {
            GameObject go = new GameObject(name + "_A");
            go.transform.position = transform.position + Vector3.left * 3f;
            pontoA = go.transform;
        }
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, alvo, velocidade * Time.deltaTime);

        if (Vector3.Distance(transform.position, alvo) < 0.1f)
        {
            alvo = (alvo == pontoA.position) ? pontoB.position : pontoA.position;
            // Vira o sprite
            Vector3 s = transform.localScale;
            s.x *= -1;
            transform.localScale = s;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        PlayerController2D player = col.gameObject.GetComponent<PlayerController2D>();
        if (player == null) return;

        // Verifica se o jogador está caindo em cima (pulo no inimigo)
        // Ponto de contato acima do centro do inimigo = pisou
        ContactPoint2D contato = col.contacts[0];
        bool pisouEmCima = contato.point.y > transform.position.y + 0.3f
                           && player.GetComponent<Rigidbody2D>().linearVelocity.y < 0;

        if (pisouEmCima)
        {
            player.Quicar(11f);
            Morrer();
        }
        else
        {
            player.LevarDano(transform.position);
        }
    }

    void Morrer()
    {
        // Efeito simples: aumenta pontos e destrói
        GameManager.Instance?.AdicionarPontos(100);
        Destroy(gameObject);
    }
}
