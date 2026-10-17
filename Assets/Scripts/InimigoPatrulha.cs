using UnityEngine;

/// <summary>
/// Inimigo que anda de um lado para o outro entre xMin e xMax.
/// Pular na cabeça derrota o inimigo; encostar pelo lado machuca o jogador.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class InimigoPatrulha : MonoBehaviour
{
    public float velocidade = 1.8f;
    public float xMin;
    public float xMax;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int direcao = 1;
    private bool morto;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (morto) return;
        Vector2 p = rb.position;
        p.x += direcao * velocidade * Time.fixedDeltaTime;
        if (p.x >= xMax) { p.x = xMax; direcao = -1; }
        else if (p.x <= xMin) { p.x = xMin; direcao = 1; }
        rb.MovePosition(p);
        if (sr != null) sr.flipX = direcao < 0;
    }

    void OnTriggerEnter2D(Collider2D outro) { Tratar(outro); }
    void OnTriggerStay2D(Collider2D outro) { Tratar(outro); }

    void Tratar(Collider2D outro)
    {
        if (morto) return;
        PlayerController2D jogador = outro.GetComponent<PlayerController2D>();
        if (jogador == null || !jogador.Vivo) return;

        GameManager gm = GameManager.Garantir();
        if (gm.Estado != EstadoJogo.Jogando) return;

        bool pisou = jogador.VelocidadeY <= 0.1f && jogador.transform.position.y > transform.position.y + 0.3f;
        if (pisou)
        {
            jogador.Quicar(10f);
            gm.AdicionarPontos(GameManager.PontosInimigo);
            Derrotar();
        }
        else
        {
            jogador.LevarDano(transform.position);
        }
    }

    void Derrotar()
    {
        morto = true;
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.enabled = false;
        AnimadorSprite a = GetComponent<AnimadorSprite>();
        if (a != null) a.enabled = false;
        transform.localScale = new Vector3(1f, 0.3f, 1f);
        Destroy(gameObject, 0.3f);
    }
}
