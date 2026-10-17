using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Jogador do jogo de plataforma. Cria sozinho collider, sprites e animação.
/// Controles: setas ou A/D andam; espaço, seta para cima ou W pulam.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 6.5f;
    public float forcaPulo = 14f;
    public float escalaGravidade = 3.2f;
    public float tempoCoyote = 0.1f;
    public float tempoBufferPulo = 0.12f;

    [Header("Dano")]
    public float tempoInvencivel = 1.5f;

    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private SpriteRenderer sr;
    private AnimadorSprite anim;

    private Sprite[] spParado, spCorrendo, spPulando, spDano;

    private float coyote;
    private float bufferPulo;
    private float invencivel;
    private float semControle;
    private float tempoNoChao;
    private bool cortouPulo = true;
    private bool morto;
    private Vector2 ultimoSeguro;

    private static readonly Collider2D[] resultados = new Collider2D[8];

    public bool NoChao { get; private set; }
    public float VelocidadeY { get { return rb != null ? rb.linearVelocity.y : 0f; } }
    public bool Vivo { get { return !morto; } }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = escalaGravidade;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        col = GetComponent<CapsuleCollider2D>();
        if (col == null) col = gameObject.AddComponent<CapsuleCollider2D>();
        col.direction = CapsuleDirection2D.Vertical;
        col.size = new Vector2(0.5f, 0.9f);
        col.offset = new Vector2(0f, 0.45f);
        PhysicsMaterial2D semAtrito = new PhysicsMaterial2D("SemAtrito");
        semAtrito.friction = 0f;
        semAtrito.bounciness = 0f;
        col.sharedMaterial = semAtrito;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 10;

        anim = GetComponent<AnimadorSprite>();
        if (anim == null) anim = gameObject.AddComponent<AnimadorSprite>();

        Recursos rec = Jogo.Rec;
        if (rec != null)
        {
            spParado = Jogo.Fatiar(rec.jogadorParado, 4);
            spCorrendo = Jogo.Fatiar(rec.jogadorCorrendo, 6);
            spPulando = Jogo.Fatiar(rec.jogadorPulando, 8);
            spDano = Jogo.Fatiar(rec.jogadorDano, 4);
            anim.Tocar(spParado, 8f, true);
        }

        ultimoSeguro = transform.position;
    }

    void Update()
    {
        GameManager gm = GameManager.Instancia;
        bool jogando = gm == null || gm.Estado == EstadoJogo.Jogando || gm.Estado == EstadoJogo.FaseConcluida;
        bool podeControlar = !morto && (gm == null || gm.Estado == EstadoJogo.Jogando);

        // ----- Entrada -----
        float mov = 0f;
        bool pulou = false;
        bool segurandoPulo = false;
        Keyboard kb = Keyboard.current;
        if (podeControlar && kb != null)
        {
            if (kb.leftArrowKey.isPressed || kb.aKey.isPressed) mov -= 1f;
            if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) mov += 1f;
            pulou = kb.spaceKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame;
            segurandoPulo = kb.spaceKey.isPressed || kb.upArrowKey.isPressed || kb.wKey.isPressed;
        }

        // ----- Chão -----
        NoChao = VerificarChao();
        if (NoChao)
        {
            coyote = tempoCoyote;
            tempoNoChao += Time.deltaTime;
            if (tempoNoChao > 0.3f && !morto) ultimoSeguro = transform.position;
        }
        else
        {
            coyote -= Time.deltaTime;
            tempoNoChao = 0f;
        }

        if (pulou) bufferPulo = tempoBufferPulo; else bufferPulo -= Time.deltaTime;
        if (semControle > 0f) semControle -= Time.deltaTime;
        if (invencivel > 0f) invencivel -= Time.deltaTime;

        Vector2 v = rb.linearVelocity;

        // ----- Movimento horizontal -----
        if (!morto && semControle <= 0f)
        {
            v.x = mov * velocidade;
        }

        // ----- Pulo (com tolerância e altura variável) -----
        if (podeControlar && bufferPulo > 0f && coyote > 0f)
        {
            v.y = forcaPulo;
            bufferPulo = 0f;
            coyote = 0f;
            cortouPulo = false;
            anim.Tocar(spPulando, 10f, false);
        }
        if (!cortouPulo && !segurandoPulo && v.y > 0f)
        {
            v.y *= 0.5f;
            cortouPulo = true;
        }
        if (v.y <= 0f) cortouPulo = true;

        rb.linearVelocity = v;

        // ----- Virar sprite -----
        if (mov > 0.01f) sr.flipX = false;
        else if (mov < -0.01f) sr.flipX = true;

        // ----- Animação -----
        if (!morto)
        {
            if (invencivel > tempoInvencivel - 0.35f && semControle > 0f)
                anim.Tocar(spDano, 12f, false);
            else if (!NoChao)
                anim.Tocar(spPulando, 10f, false);
            else if (Mathf.Abs(mov) > 0.01f)
                anim.Tocar(spCorrendo, 12f, true);
            else
                anim.Tocar(spParado, 8f, true);
        }

        // ----- Piscar enquanto invencível -----
        Color c = sr.color;
        c.a = (invencivel > 0f && !morto && Mathf.FloorToInt(Time.time * 20f) % 2 == 0) ? 0.35f : 1f;
        sr.color = c;

        // ----- Caiu no buraco -----
        float limite = gm != null ? gm.limiteQueda : -6f;
        if (jogando && !morto && transform.position.y < limite) CairNoVazio();
    }

    bool VerificarChao()
    {
        Vector2 centro = (Vector2)transform.position + new Vector2(0f, -0.03f);
        Vector2 tamanho = new Vector2(0.4f, 0.12f);
        ContactFilter2D filtro = new ContactFilter2D();
        filtro.useTriggers = false;
        int n = Physics2D.OverlapBox(centro, tamanho, 0f, filtro, resultados);
        for (int i = 0; i < n; i++)
        {
            Collider2D c = resultados[i];
            if (c == null || c == col) continue;
            if (c.attachedRigidbody == rb) continue;
            return true;
        }
        return false;
    }

    /// <summary>Chamado pelo inimigo quando encosta no jogador.</summary>
    public void LevarDano(Vector2 origemDoDano)
    {
        if (morto || invencivel > 0f) return;
        GameManager gm = GameManager.Garantir();
        if (gm.Estado != EstadoJogo.Jogando) return;

        int restantes = gm.PerderVida();
        if (restantes <= 0)
        {
            Morrer();
            return;
        }

        invencivel = tempoInvencivel;
        semControle = 0.3f;
        float dirX = transform.position.x >= origemDoDano.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(dirX * 5f, 8f);
    }

    /// <summary>Quique ao pisar em um inimigo.</summary>
    public void Quicar(float forca)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, forca);
        cortouPulo = true;
        coyote = 0f;
    }

    void CairNoVazio()
    {
        GameManager gm = GameManager.Garantir();
        int restantes = gm.PerderVida();
        if (restantes <= 0)
        {
            Morrer();
            return;
        }
        transform.position = new Vector3(ultimoSeguro.x, ultimoSeguro.y + 0.1f, 0f);
        rb.position = transform.position;
        rb.linearVelocity = Vector2.zero;
        invencivel = tempoInvencivel + 0.5f;
        semControle = 0f;
    }

    void Morrer()
    {
        morto = true;
        anim.Tocar(spDano, 8f, false);
        col.enabled = false;
        rb.linearVelocity = new Vector2(0f, 9f);
        GameManager.Garantir().Derrota();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(0f, -0.03f, 0f), new Vector3(0.4f, 0.12f, 0f));
    }
}
