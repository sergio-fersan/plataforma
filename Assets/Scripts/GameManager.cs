using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Controla pontos, vidas, vitória e derrota. Não destruir entre fases.
/// Arraste os Textos do Canvas nos campos hudPontos / hudVidas / hudFase.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Estado")]
    public int pontos = 0;
    public int totalColetaveisFase = 0;
    public int coletadosFase = 0;

    [Header("HUD (arraste aqui)")]
    public TMP_Text hudPontos;
    public TMP_Text hudVidas;
    public TMP_Text hudFase;

    [Header("Cenas (nome exato no Build Settings)")]
    public string cenaFase1 = "Fase1";
    public string cenaFase2 = "Fase2";
    public string cenaFinal = "CenaFinal";

    // Usado pela CenaFinal para mostrar vitória/derrota
    public static bool UltimaVitoria = false;
    public static int PontosFinais = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;
    }

    void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        // Zera contagem da fase nova e reconecta o HUD (o Canvas é novo por cena)
        totalColetaveisFase = 0;
        coletadosFase = 0;

        if (hudPontos == null) hudPontos = GameObject.Find("TxtPontos")?.GetComponent<TMP_Text>();
        if (hudVidas == null) hudVidas = GameObject.Find("TxtVidas")?.GetComponent<TMP_Text>();
        if (hudFase == null) hudFase = GameObject.Find("TxtFase")?.GetComponent<TMP_Text>();

        AtualizarHUD();
    }

    public void RegistrarColetavel()
    {
        totalColetaveisFase++;
    }

    public void ColetarItem(int valor)
    {
        pontos += valor;
        coletadosFase++;
        AtualizarHUD();

        // VITÓRIA DA FASE = coletou tudo que existia nela
        if (totalColetaveisFase > 0 && coletadosFase >= totalColetaveisFase)
        {
            VitoriaFase();
        }
    }

    public void AdicionarPontos(int valor)
    {
        pontos += valor;
        AtualizarHUD();
    }

    void VitoriaFase()
    {
        string atual = SceneManager.GetActiveScene().name;

        if (atual == cenaFase1)
        {
            SceneManager.LoadScene(cenaFase2);
        }
        else
        {
            // Era a Fase2 -> zerou o jogo
            VitoriaJogo();
        }
    }

    public void VitoriaJogo()
    {
        UltimaVitoria = true;
        PontosFinais = pontos;
        SceneManager.LoadScene(cenaFinal);
    }

    public void Derrota()
    {
        // Se ainda tem vida, só atualiza HUD. Se zerou, vai pro final.
        var player = FindFirstObjectByType<PlayerController2D>();
        if (player != null && player.Vidas > 0)
        {
            AtualizarHUD();
            return;
        }

        UltimaVitoria = false;
        PontosFinais = pontos;
        SceneManager.LoadScene(cenaFinal);
    }

    public void AtualizarHUD()
    {
        if (hudPontos == null) hudPontos = GameObject.Find("TxtPontos")?.GetComponent<TMP_Text>();
        if (hudVidas == null) hudVidas = GameObject.Find("TxtVidas")?.GetComponent<TMP_Text>();

        if (hudPontos != null) hudPontos.text = "Pontos: " + pontos;

        var player = FindFirstObjectByType<PlayerController2D>();
        if (hudVidas != null)
        {
            int v = player != null ? player.Vidas : 3;
            hudVidas.text = "Vidas: " + v;
        }

        if (hudFase != null) hudFase.text = SceneManager.GetActiveScene().name;
    }

    // Chamado pelo botão "Jogar Novamente" da CenaFinal
    public void ReiniciarJogo()
    {
        pontos = 0;
        SceneManager.LoadScene(cenaFase1);
    }
}
