using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EstadoJogo { Menu, Jogando, FaseConcluida, Vitoria, Derrota }

/// <summary>
/// Guarda pontos, vidas e o andamento das fases. Persiste entre as cenas.
/// É criado automaticamente (GameManager.Garantir), não precisa estar na cena.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    // Resultado lido pela CenaFinal
    public static bool UltimaVitoria = false;
    public static int PontosFinais = 0;

    [Header("Estado")]
    public int pontos = 0;
    public int vidasIniciais = 3;
    public int vidas = 3;
    public int faseAtual = 1;
    public int totalFases = 2;
    public int moedasColetadas = 0;
    public int moedasTotal = 0;
    public EstadoJogo Estado = EstadoJogo.Menu;
    public float limiteQueda = -6f;
    public string mensagem = "";

    public const int PontosMoeda = 10;
    public const int PontosInimigo = 100;

    public static GameManager Garantir()
    {
        if (Instancia == null)
        {
            GameManager achado = FindFirstObjectByType<GameManager>();
            if (achado != null)
            {
                Instancia = achado;
            }
            else
            {
                GameObject go = new GameObject("GameManager");
                Instancia = go.AddComponent<GameManager>();
            }
        }
        return Instancia;
    }

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NovoJogo()
    {
        pontos = 0;
        vidas = vidasIniciais;
        faseAtual = 1;
        moedasColetadas = 0;
        moedasTotal = 0;
        mensagem = "";
        Estado = EstadoJogo.Jogando;
        SceneManager.LoadScene(Jogo.CenaFase1);
    }

    public void IrParaMenu()
    {
        Estado = EstadoJogo.Menu;
        mensagem = "";
        SceneManager.LoadScene(Jogo.CenaMenu);
    }

    /// <summary>Chamado pelo construtor da fase quando ela está pronta.</summary>
    public void IniciarFase(int numero, int totalMoedas)
    {
        faseAtual = numero;
        moedasTotal = totalMoedas;
        moedasColetadas = 0;
        mensagem = "";
        Estado = EstadoJogo.Jogando;
    }

    public void ColetarMoeda(int valor)
    {
        if (Estado != EstadoJogo.Jogando) return;
        pontos += valor;
        moedasColetadas++;
        if (moedasTotal > 0 && moedasColetadas >= moedasTotal) ConcluirFase();
    }

    public void AdicionarPontos(int valor)
    {
        if (Estado != EstadoJogo.Jogando) return;
        pontos += valor;
    }

    /// <summary>Tira uma vida e devolve quantas restam.</summary>
    public int PerderVida()
    {
        vidas = Mathf.Max(0, vidas - 1);
        return vidas;
    }

    public void Derrota()
    {
        if (Estado == EstadoJogo.Derrota || Estado == EstadoJogo.Vitoria) return;
        Estado = EstadoJogo.Derrota;
        UltimaVitoria = false;
        PontosFinais = pontos;
        mensagem = "";
        StartCoroutine(CarregarDepois(Jogo.CenaFinal, 1.4f));
    }

    void ConcluirFase()
    {
        if (faseAtual < totalFases)
        {
            Estado = EstadoJogo.FaseConcluida;
            mensagem = "FASE " + faseAtual + " CONCLUÍDA!";
            string proxima = faseAtual == 1 ? Jogo.CenaFase2 : Jogo.CenaFase1;
            StartCoroutine(CarregarDepois(proxima, 1.8f));
        }
        else
        {
            Estado = EstadoJogo.Vitoria;
            UltimaVitoria = true;
            PontosFinais = pontos;
            mensagem = "VOCÊ COLETOU TUDO!";
            StartCoroutine(CarregarDepois(Jogo.CenaFinal, 1.8f));
        }
    }

    IEnumerator CarregarDepois(string cena, float segundos)
    {
        yield return new WaitForSeconds(segundos);
        SceneManager.LoadScene(cena);
    }
}
