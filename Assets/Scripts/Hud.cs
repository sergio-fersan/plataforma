using UnityEngine;
using UnityEngine.UI;

/// <summary>Interface durante a fase: vidas, pontos, moedas, número da fase e mensagens.</summary>
public class Hud : MonoBehaviour
{
    private Text txtPontos;
    private Text txtMoedas;
    private Text txtFase;
    private Text txtMensagem;
    private Image[] coracoes;

    public static Hud Criar()
    {
        Canvas canvas = UIHelper.CriarCanvas("HUD", 10);
        Hud hud = canvas.gameObject.AddComponent<Hud>();
        hud.Montar(canvas.transform);
        return hud;
    }

    void Montar(Transform raiz)
    {
        Vector2 topoEsq = new Vector2(0f, 1f);
        Vector2 topoDir = new Vector2(1f, 1f);
        Vector2 topoMeio = new Vector2(0.5f, 1f);

        // Vidas (corações)
        int max = GameManager.Garantir().vidasIniciais;
        coracoes = new Image[max];
        for (int i = 0; i < max; i++)
        {
            coracoes[i] = UIHelper.CriarImagem(raiz, "Coracao" + i, Jogo.Coracao(), topoEsq,
                                               new Vector2(30f + i * 62f, -30f), new Vector2(54f, 48f));
        }

        txtPontos = UIHelper.CriarTexto(raiz, "TxtPontos", "PONTOS: 0", 42, TextAnchor.UpperLeft,
                                        topoEsq, new Vector2(30f, -90f), new Vector2(600f, 60f), Color.white);

        txtFase = UIHelper.CriarTexto(raiz, "TxtFase", "FASE 1", 46, TextAnchor.UpperCenter,
                                      topoMeio, new Vector2(0f, -25f), new Vector2(500f, 70f), Color.white);
        txtFase.rectTransform.pivot = topoMeio;

        Sprite[] moeda = Jogo.Fatiar(Jogo.Rec != null ? Jogo.Rec.moeda : null, 1);
        if (moeda.Length > 0)
        {
            UIHelper.CriarImagem(raiz, "IconeMoeda", moeda[0], topoDir, new Vector2(-250f, -30f), new Vector2(60f, 60f));
        }
        txtMoedas = UIHelper.CriarTexto(raiz, "TxtMoedas", "0/0", 46, TextAnchor.UpperRight,
                                        topoDir, new Vector2(-30f, -35f), new Vector2(200f, 60f), Color.white);

        txtMensagem = UIHelper.CriarTexto(raiz, "TxtMensagem", "", 90, TextAnchor.MiddleCenter,
                                          new Vector2(0.5f, 0.5f), new Vector2(0f, 120f), new Vector2(1500f, 200f),
                                          new Color(1f, 0.92f, 0.3f, 1f));
        txtMensagem.rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    void Update()
    {
        GameManager gm = GameManager.Instancia;
        if (gm == null) return;

        txtPontos.text = "PONTOS: " + gm.pontos;
        txtMoedas.text = gm.moedasColetadas + "/" + gm.moedasTotal;
        txtFase.text = "FASE " + gm.faseAtual;
        txtMensagem.text = gm.mensagem;

        for (int i = 0; i < coracoes.Length; i++)
        {
            coracoes[i].enabled = i < gm.vidas;
        }
    }
}
