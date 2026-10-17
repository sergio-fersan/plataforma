using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>Funções para criar interface (Canvas, textos, botões) por código.</summary>
public static class UIHelper
{
    private static Font _fonte;

    public static Font Fonte
    {
        get
        {
            if (_fonte == null) _fonte = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_fonte == null) _fonte = Font.CreateDynamicFontFromOSFont("Arial", 32);
            return _fonte;
        }
    }

    public static Canvas CriarCanvas(string nome, int ordem)
    {
        GameObject go = new GameObject(nome);
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = ordem;
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    static RectTransform Posicionar(GameObject go, Transform pai, Vector2 ancora, Vector2 posicao, Vector2 tamanho)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.SetParent(pai, false);
        rt.anchorMin = ancora;
        rt.anchorMax = ancora;
        rt.pivot = ancora;
        rt.anchoredPosition = posicao;
        rt.sizeDelta = tamanho;
        return rt;
    }

    public static Image CriarPainel(Transform pai, string nome, Vector2 ancora, Vector2 posicao, Vector2 tamanho, Color cor)
    {
        GameObject go = new GameObject(nome);
        Image img = go.AddComponent<Image>();
        img.color = cor;
        img.raycastTarget = false;
        Posicionar(go, pai, ancora, posicao, tamanho);
        return img;
    }

    public static Image CriarImagem(Transform pai, string nome, Sprite sprite, Vector2 ancora, Vector2 posicao, Vector2 tamanho)
    {
        GameObject go = new GameObject(nome);
        Image img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        img.raycastTarget = false;
        Posicionar(go, pai, ancora, posicao, tamanho);
        return img;
    }

    public static Text CriarTexto(Transform pai, string nome, string conteudo, int tamanhoFonte, TextAnchor alinhamento,
                                  Vector2 ancora, Vector2 posicao, Vector2 tamanho, Color cor, bool contorno = true)
    {
        GameObject go = new GameObject(nome);
        Text t = go.AddComponent<Text>();
        t.font = Fonte;
        t.text = conteudo;
        t.fontSize = tamanhoFonte;
        t.alignment = alinhamento;
        t.color = cor;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.supportRichText = true;
        t.raycastTarget = false;
        Posicionar(go, pai, ancora, posicao, tamanho);
        if (contorno)
        {
            Outline o = go.AddComponent<Outline>();
            o.effectColor = new Color(0f, 0f, 0f, 0.9f);
            o.effectDistance = new Vector2(2f, -2f);
        }
        return t;
    }

    public static BotaoUI CriarBotao(Transform pai, string nome, string rotulo, Vector2 ancora, Vector2 posicao, Vector2 tamanho,
                                     System.Action acao, Key atalho1, Key atalho2, Color cor)
    {
        GameObject go = new GameObject(nome);
        Image fundo = go.AddComponent<Image>();
        fundo.color = cor;
        Posicionar(go, pai, ancora, posicao, tamanho);

        Text t = CriarTexto(go.transform, "Rotulo", rotulo, 40, TextAnchor.MiddleCenter,
                            new Vector2(0.5f, 0.5f), Vector2.zero, tamanho, Color.white, true);

        BotaoUI b = go.AddComponent<BotaoUI>();
        b.fundo = fundo;
        b.corNormal = cor;
        b.corSobre = Color.Lerp(cor, Color.white, 0.3f);
        b.aoClicar = acao;
        b.atalho1 = atalho1;
        b.atalho2 = atalho2;
        b.atraso = 0.5f;
        return b;
    }
}

/// <summary>
/// Botão simples que funciona com o novo Input System, sem precisar de EventSystem:
/// clique do mouse ou teclas de atalho.
/// </summary>
public class BotaoUI : MonoBehaviour
{
    public System.Action aoClicar;
    public Image fundo;
    public Color corNormal = Color.gray;
    public Color corSobre = Color.white;
    public Key atalho1 = Key.None;
    public Key atalho2 = Key.None;
    public float atraso = 0.5f;   // ignora entradas logo que a tela abre

    private RectTransform rt;
    private bool usado;
    private float tempo;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        tempo += Time.unscaledDeltaTime;
        if (usado) return;

        Mouse mouse = Mouse.current;
        Keyboard kb = Keyboard.current;

        bool sobre = false;
        if (mouse != null)
        {
            Vector2 pos = mouse.position.ReadValue();
            sobre = RectTransformUtility.RectangleContainsScreenPoint(rt, pos, null);
        }
        if (fundo != null) fundo.color = sobre ? corSobre : corNormal;

        if (tempo < atraso) return;

        bool clique = sobre && mouse != null && mouse.leftButton.wasPressedThisFrame;
        bool tecla = false;
        if (kb != null)
        {
            if (atalho1 != Key.None && kb[atalho1].wasPressedThisFrame) tecla = true;
            if (atalho2 != Key.None && kb[atalho2].wasPressedThisFrame) tecla = true;
        }

        if (clique || tecla)
        {
            usado = true;
            if (aoClicar != null) aoClicar();
        }
    }
}
