using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controle do jogador - plataforma 2D.
/// Funciona com New Input System (Keyboard.current), sem precisar configurar Input Actions.
/// Arraste para o GameObject do Jogador que tenha Rigidbody2D + BoxCollider2D/CapsuleCollider2D.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 7f;
    public float forcaPulo = 13f;

    [Header("Chão")]
    public Transform peDoJogador; // objeto vazio nos pés
    public float raioChao = 0.2f;
    public LayerMask camadaChao; // marque a Tilemap/Grid como camada "Ground"

    [Header("Vidas / Dano")]
    public int vidasMax = 3;
    public float tempoInvencivel = 1f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private float tempoInvencivelAtual = 0f;
    private bool olhandoDireita = true;

    public int Vidas { get; private set; }
    public bool EstaNoChao { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        Vidas = vidasMax;

        // Trava rotação para o jogador não tombar
        rb.freezeRotation = true;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // --- Movimento esquerda/direita ---
        float mov = 0f;
        if (kb.leftArrowKey.isPressed || kb.aKey.isPressed) mov -= 1f;
        if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) mov += 1f;

        rb.linearVelocity = new Vector2(mov * velocidade, rb.linearVelocity.y);

        // Vira o sprite
        if (mov > 0 && !olhandoDireita) Virar();
        else if (mov < 0 && olhandoDireita) Virar();

        // --- Chão ---
        EstaNoChao = false;
        if (peDoJogador != null)
            EstaNoChao = Physics2D.OverlapCircle(peDoJogador.position, raioChao, camadaChao);

        // --- Pulo (espaço / seta cima / W) ---
        bool pulou = kb.spaceKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame;
        if (pulou && EstaNoChao)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
        }

        if (tempoInvencivelAtual > 0) tempoInvencivelAtual -= Time.deltaTime;

        // --- Animador (se tiver) ---
        if (anim != null)
        {
            anim.SetFloat("velocidade", Mathf.Abs(mov));
            anim.SetBool("noChao", EstaNoChao);
        }
    }

    void Virar()
    {
        olhandoDireita = !olhandoDireita;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    /// <summary>Chamado pelo inimigo quando encosta no jogador.</summary>
    public void LevarDano(Vector2 origemDoDano)
    {
        if (tempoInvencivelAtual > 0) return;

        Vidas--;
        tempoInvencivelAtual = tempoInvencivel;

        // Empurrãozinho para trás/cima
        Vector2 dir = ((Vector2)transform.position - origemDoDano).normalized;
        rb.linearVelocity = new Vector2(dir.x * 6f, 7f);

        // Pisca para mostrar invencibilidade
        if (sprite != null) StartCoroutine(Piscar());

        GameManager.Instance?.AtualizarHUD();

        if (Vidas <= 0)
        {
            GameManager.Instance?.Derrota();
        }
    }

    /// <summary>Chamado quando pisa no inimigo.</summary>
    public void Quicar(float forca = 10f)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, forca);
    }

    System.Collections.IEnumerator Piscar()
    {
        for (int i = 0; i < 5; i++)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (peDoJogador != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(peDoJogador.position, raioChao);
        }
    }
}
