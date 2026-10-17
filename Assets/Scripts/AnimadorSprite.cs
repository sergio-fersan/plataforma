using UnityEngine;

/// <summary>Anima um SpriteRenderer trocando os quadros por código.</summary>
[RequireComponent(typeof(SpriteRenderer))]
public class AnimadorSprite : MonoBehaviour
{
    public Sprite[] quadros;
    public float fps = 10f;
    public bool repetir = true;

    private SpriteRenderer sr;
    private float tempo;
    private int atual = -1;

    public bool Terminou { get; private set; }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>Troca a animação (não reinicia se for a mesma).</summary>
    public void Tocar(Sprite[] novos, float novoFps, bool loop)
    {
        if (novos == null || novos.Length == 0) return;
        if (ReferenceEquals(quadros, novos)) return;
        quadros = novos;
        fps = novoFps;
        repetir = loop;
        tempo = 0f;
        atual = -1;
        Terminou = false;
        Aplicar(0);
    }

    void Update()
    {
        if (quadros == null || quadros.Length == 0) return;
        tempo += Time.deltaTime;
        int i = Mathf.FloorToInt(tempo * fps);
        if (repetir)
        {
            i %= quadros.Length;
        }
        else if (i >= quadros.Length)
        {
            i = quadros.Length - 1;
            Terminou = true;
        }
        Aplicar(i);
    }

    void Aplicar(int i)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (i == atual) return;
        atual = i;
        sr.sprite = quadros[i];
    }
}
