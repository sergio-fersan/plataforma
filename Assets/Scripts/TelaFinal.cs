using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>Cena final: mostra vitória ou derrota, os pontos e o botão para reiniciar.</summary>
public static class TelaFinal
{
    public static void Construir()
    {
        GameManager gm = GameManager.Garantir();
        bool vitoria = GameManager.UltimaVitoria;
        int pontos = GameManager.PontosFinais;

        Camera cam = ConstrutorFase.ConfigurarCamera();
        cam.transform.position = new Vector3(16f, 4.5f, -10f);
        ConstrutorFase.CriarCenario(cam, 0.8f);
        ConstrutorFase.CriarChao(new string[]
        {
            "################################",
            "################################",
            "################################"
        });

        Recursos rec = Jogo.Rec;
        if (rec != null)
        {
            GameObject go = new GameObject("Personagem");
            go.transform.position = new Vector3(16f, 3f, 0f);
            go.transform.localScale = new Vector3(4f, 4f, 1f);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 10;
            AnimadorSprite a = go.AddComponent<AnimadorSprite>();
            if (vitoria) a.Tocar(Jogo.Fatiar(rec.jogadorPulando, 8), 8f, true);
            else a.Tocar(Jogo.Fatiar(rec.jogadorDano, 4), 5f, true);
        }

        Canvas canvas = UIHelper.CriarCanvas("TelaFinal", 10);
        Transform raiz = canvas.transform;
        Vector2 meio = new Vector2(0.5f, 0.5f);

        UIHelper.CriarPainel(raiz, "Painel", meio, new Vector2(0f, 220f), new Vector2(1200f, 470f), new Color(0.05f, 0.1f, 0.2f, 0.78f));

        Color cor = vitoria ? new Color(0.35f, 1f, 0.45f) : new Color(1f, 0.3f, 0.3f);
        UIHelper.CriarTexto(raiz, "TxtResultado", vitoria ? "VITÓRIA!" : "DERROTA", 170, TextAnchor.MiddleCenter,
                            meio, new Vector2(0f, 330f), new Vector2(1100f, 220f), cor);
        UIHelper.CriarTexto(raiz, "TxtMensagem",
                            vitoria ? "Você coletou todas as moedas das 2 fases!" : "Os inimigos acabaram com as suas vidas.",
                            46, TextAnchor.MiddleCenter, meio, new Vector2(0f, 215f), new Vector2(1100f, 80f), Color.white);
        UIHelper.CriarTexto(raiz, "TxtPontosFinais", "PONTOS: " + pontos, 74, TextAnchor.MiddleCenter,
                            meio, new Vector2(0f, 115f), new Vector2(1100f, 100f), new Color(1f, 0.92f, 0.3f));

        UIHelper.CriarBotao(raiz, "BotaoReiniciar", "JOGAR NOVAMENTE  (Enter)", new Vector2(0.5f, 0f), new Vector2(-330f, 70f),
                            new Vector2(620f, 110f), () => GameManager.Garantir().NovoJogo(),
                            Key.Enter, Key.NumpadEnter, new Color(0.15f, 0.6f, 0.25f, 1f));
        UIHelper.CriarBotao(raiz, "BotaoMenu", "MENU  (Esc)", new Vector2(0.5f, 0f), new Vector2(330f, 70f),
                            new Vector2(420f, 110f), () => GameManager.Garantir().IrParaMenu(),
                            Key.Escape, Key.None, new Color(0.25f, 0.35f, 0.65f, 1f));
    }
}
