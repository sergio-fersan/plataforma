using UnityEngine;

/// <summary>Constantes e funções de apoio usadas por todo o jogo.</summary>
public static class Jogo
{
    public const string CenaMenu = "MenuInicial";
    public const string CenaFase1 = "Fase1";
    public const string CenaFase2 = "Fase2";
    public const string CenaFinal = "CenaFinal";
    public const float PPU = 32f;

    private static Recursos _recursos;

    public static Recursos Rec
    {
        get
        {
            if (_recursos == null) _recursos = Resources.Load<Recursos>("Recursos");
            if (_recursos == null) Debug.LogError("Recursos.asset não encontrado em Assets/Resources.");
            return _recursos;
        }
    }

    /// <summary>Corta uma folha de sprites horizontal em quadros de largura x altura.</summary>
    public static Sprite[] Fatiar(Texture2D tex, int quantidade, int primeiro = 0, int largura = 32, int altura = 32, float ppu = PPU, float pivotY = 0f)
    {
        if (tex == null) return new Sprite[0];
        tex.filterMode = FilterMode.Point;
        int colunas = tex.width / largura;
        if (quantidade <= 0 || quantidade > colunas - primeiro) quantidade = colunas - primeiro;
        Sprite[] lista = new Sprite[quantidade];
        for (int i = 0; i < quantidade; i++)
        {
            Rect r = new Rect((primeiro + i) * largura, 0, largura, altura);
            lista[i] = Sprite.Create(tex, r, new Vector2(0.5f, pivotY), ppu, 0, SpriteMeshType.FullRect);
        }
        return lista;
    }

    /// <summary>Recorta um retângulo (origem embaixo-esquerda) de qualquer textura.</summary>
    public static Sprite Recortar(Texture2D tex, float x, float y, float w, float h, float pivotX, float pivotY, float ppu = PPU)
    {
        if (tex == null) return null;
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(x, y, w, h), new Vector2(pivotX, pivotY), ppu, 0, SpriteMeshType.FullRect);
    }

    /// <summary>Sprite da imagem inteira (para o fundo).</summary>
    public static Sprite Inteira(Texture2D tex, float ppu)
    {
        if (tex == null) return null;
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu, 0, SpriteMeshType.FullRect);
    }

    private static Sprite _coracao;

    /// <summary>Coração pixelado desenhado por código (ícone de vidas).</summary>
    public static Sprite Coracao()
    {
        if (_coracao != null) return _coracao;
        string[] desenho =
        {
            ".XX.XX.",
            "XXXXXXX",
            "XXXXXXX",
            ".XXXXX.",
            "..XXX..",
            "...X..."
        };
        int w = 7, h = 6;
        Texture2D t = new Texture2D(w, h, TextureFormat.RGBA32, false);
        t.filterMode = FilterMode.Point;
        t.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                bool cheio = desenho[h - 1 - y][x] == 'X';
                t.SetPixel(x, y, cheio ? new Color(0.95f, 0.15f, 0.2f, 1f) : new Color(0, 0, 0, 0));
            }
        }
        t.Apply();
        _coracao = Sprite.Create(t, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 1f);
        return _coracao;
    }
}
