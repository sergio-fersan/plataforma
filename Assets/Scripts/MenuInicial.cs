using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>Tela inicial com as orientações de como jogar.</summary>
public static class MenuInicial
{
    public static void Construir()
    {
        GameManager gm = GameManager.Garantir();
        gm.Estado = EstadoJogo.Menu;

        Camera cam = ConstrutorFase.ConfigurarCamera();
        cam.transform.position = new Vector3(16f, 4.5f, -10f);
        ConstrutorFase.CriarCenario(cam, 1.2f);
        ConstrutorFase.CriarChao(new string[]
        {
            "################################",
            "################################",
            "################################"
        });
        DecorarPersonagens();

        Canvas canvas = UIHelper.CriarCanvas("Menu", 10);
        Transform raiz = canvas.transform;
        Vector2 meio = new Vector2(0.5f, 0.5f);

        UIHelper.CriarTexto(raiz, "Titulo", "MONSTRINHO COLETOR", 120, TextAnchor.MiddleCenter,
                            new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(1800f, 160f), new Color(1f, 0.92f, 0.3f));
        ((RectTransform)raiz.Find("Titulo")).pivot = new Vector2(0.5f, 1f);

        UIHelper.CriarPainel(raiz, "PainelInstrucoes", meio, new Vector2(0f, 10f), new Vector2(1250f, 560f), new Color(0.05f, 0.1f, 0.2f, 0.78f));

        string instrucoes =
            "<size=56><b>COMO JOGAR</b></size>\n\n" +
            "<b>Setas</b> ou <b>A / D</b>  -  andar\n" +
            "<b>Espaço</b>, <b>Seta para cima</b> ou <b>W</b>  -  pular (segure para pular mais alto)\n\n" +
            "Pule na cabeça dos inimigos para derrotá-los (+100)\n" +
            "Cada moeda vale +10 pontos\n" +
            "Colete <b>TODAS as moedas</b> da fase para avançar\n" +
            "São <b>2 fases</b>. Você começa com <b>3 vidas</b>\n\n" +
            "Se os inimigos te machucarem ou você cair no buraco,\nperde uma vida. Sem vidas, <b>você perde</b>!";
        Text t = UIHelper.CriarTexto(raiz, "Instrucoes", instrucoes, 38, TextAnchor.MiddleCenter,
                                     meio, new Vector2(0f, 10f), new Vector2(1180f, 520f), Color.white);
        t.rectTransform.pivot = meio;

        BotaoUI botao = UIHelper.CriarBotao(raiz, "BotaoJogar", "JOGAR  (Enter)", new Vector2(0.5f, 0f), new Vector2(0f, 60f),
                                            new Vector2(560f, 110f), () => GameManager.Garantir().NovoJogo(),
                                            Key.Enter, Key.Space, new Color(0.15f, 0.6f, 0.25f, 1f));
        ((RectTransform)botao.transform).pivot = new Vector2(0.5f, 0f);
    }

    static void DecorarPersonagens()
    {
        Recursos rec = Jogo.Rec;
        if (rec == null) return;

        CriarPersonagem(Jogo.Fatiar(rec.jogadorCorrendo, 6), 10f, 12f, 3f);
        CriarPersonagem(Jogo.Fatiar(rec.inimigoA, 6), 21.5f, 9f, 3f);
        CriarPersonagem(Jogo.Fatiar(rec.inimigoB, 6), 23.5f, 9f, 3f);

        GameObject moeda = new GameObject("MoedaMenu");
        moeda.transform.position = new Vector3(12.5f, 4.2f, 0f);
        SpriteRenderer sr = moeda.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 5;
        moeda.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        moeda.AddComponent<AnimadorSprite>().Tocar(Jogo.Fatiar(rec.moeda, 32), 18f, true);
    }

    static void CriarPersonagem(Sprite[] quadros, float x, float fps, float escala)
    {
        GameObject go = new GameObject("PersonagemMenu");
        go.transform.position = new Vector3(x, 3f, 0f);
        go.transform.localScale = new Vector3(escala, escala, 1f);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 6;
        sr.flipX = x > 16f;
        go.AddComponent<AnimadorSprite>().Tocar(quadros, fps, true);
    }
}
