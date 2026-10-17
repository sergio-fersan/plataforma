using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Monta uma fase inteira a partir do mapa em NivelDados:
/// chão (Tilemap com colisão), decoração, moedas, inimigos, jogador, câmera, parallax e HUD.
/// </summary>
public static class ConstrutorFase
{
    public const float TamanhoCamera = 6.5f;
    public static readonly Color CorCeu = new Color(136f / 255f, 194f / 255f, 246f / 255f, 1f);

    // ------------------------------------------------------------------ câmera / cenário

    public static Camera ConfigurarCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Camera qualquer = Object.FindFirstObjectByType<Camera>();
            if (qualquer != null) cam = qualquer;
        }
        if (cam == null)
        {
            GameObject go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            cam = go.AddComponent<Camera>();
            go.AddComponent<AudioListener>();
        }
        cam.orthographic = true;
        cam.orthographicSize = TamanhoCamera;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = CorCeu;
        cam.transform.position = new Vector3(0f, 4.5f, -10f);
        return cam;
    }

    static Transform CriarFaixa(Transform pai, string nome, Sprite sprite, int ordem, float larguraFaixa)
    {
        GameObject go = new GameObject(nome);
        go.transform.SetParent(pai, false);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = ordem;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(larguraFaixa, sprite.bounds.size.y);
        return go.transform;
    }

    /// <summary>Céu, montanhas e colinas com parallax. Devolve o componente Parallax.</summary>
    public static Parallax CriarCenario(Camera cam, float derivaAutomatica)
    {
        Recursos rec = Jogo.Rec;
        GameObject raiz = new GameObject("Cenario");
        Parallax p = raiz.AddComponent<Parallax>();
        p.cameraAlvo = cam.transform;
        p.derivaAutomatica = derivaAutomatica;

        List<Parallax.Camada> camadas = new List<Parallax.Camada>();
        const float faixa = 160f;   // múltiplo da largura de cada imagem

        if (rec != null && rec.ceu != null)
        {
            Sprite s = Jogo.Inteira(rec.ceu, 16f);
            Parallax.Camada c = new Parallax.Camada();
            c.fundo = CriarFaixa(raiz.transform, "Ceu", s, -100, faixa);
            c.fatorX = 0.05f;
            c.fatorY = 1f;
            c.seguirCameraEmY = true;
            c.largura = s.bounds.size.x;
            camadas.Add(c);
        }
        if (rec != null && rec.montanhas != null)
        {
            Sprite s = Jogo.Inteira(rec.montanhas, 16f);
            Parallax.Camada c = new Parallax.Camada();
            c.fundo = CriarFaixa(raiz.transform, "Montanhas", s, -90, faixa);
            c.fatorX = 0.2f;
            c.fatorY = 0.6f;
            c.yBase = 6f;
            c.largura = s.bounds.size.x;
            camadas.Add(c);
        }
        if (rec != null && rec.colinas != null)
        {
            Sprite s = Jogo.Inteira(rec.colinas, 16f);
            Parallax.Camada c = new Parallax.Camada();
            c.fundo = CriarFaixa(raiz.transform, "Colinas", s, -80, faixa);
            c.fatorX = 0.5f;
            c.fatorY = 0.8f;
            c.yBase = 0.5f;
            c.largura = s.bounds.size.x;
            camadas.Add(c);
        }
        p.camadas = camadas.ToArray();
        return p;
    }

    // ------------------------------------------------------------------ tilemap

    static Tile CriarTile(Sprite s)
    {
        Tile t = ScriptableObject.CreateInstance<Tile>();
        t.sprite = s;
        t.colliderType = Tile.ColliderType.Grid;
        return t;
    }

    /// <summary>Pinta o chão no Tilemap e liga a colisão (TilemapCollider2D + Composite + Rigidbody2D estático).</summary>
    public static void CriarChao(string[] mapa)
    {
        Recursos rec = Jogo.Rec;
        int altura = mapa.Length;
        int largura = mapa[0].Length;

        Sprite spTopo = Jogo.Recortar(rec.tileset, 96, 32, 32, 32, 0.5f, 0.5f);
        Sprite spMiolo = Jogo.Recortar(rec.tileset, 96, 0, 32, 32, 0.5f, 0.5f);
        Tile tileTopo = CriarTile(spTopo);
        Tile tileMiolo = CriarTile(spMiolo);

        GameObject grid = new GameObject("Grid");
        grid.AddComponent<Grid>();

        GameObject chao = new GameObject("Chao");
        chao.transform.SetParent(grid.transform, false);
        Tilemap tm = chao.AddComponent<Tilemap>();
        TilemapRenderer tr = chao.AddComponent<TilemapRenderer>();
        tr.sortingOrder = 0;

        for (int linha = 0; linha < altura; linha++)
        {
            int y = altura - 1 - linha;
            for (int x = 0; x < largura; x++)
            {
                if (mapa[linha][x] != '#') continue;
                bool temChaoAcima = linha > 0 && mapa[linha - 1][x] == '#';
                tm.SetTile(new Vector3Int(x, y, 0), temChaoAcima ? tileMiolo : tileTopo);
            }
        }
        tm.CompressBounds();

        Rigidbody2D rb = chao.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        CompositeCollider2D comp = chao.AddComponent<CompositeCollider2D>();
        comp.geometryType = CompositeCollider2D.GeometryType.Polygons;
        TilemapCollider2D tc = chao.AddComponent<TilemapCollider2D>();
        tc.compositeOperation = Collider2D.CompositeOperation.Merge;
    }

    /// <summary>Paredes invisíveis nas laterais da fase.</summary>
    static void CriarParedes(int largura)
    {
        GameObject go = new GameObject("Limites");
        BoxCollider2D esq = go.AddComponent<BoxCollider2D>();
        esq.size = new Vector2(1f, 200f);
        esq.offset = new Vector2(-0.5f, 0f);
        GameObject go2 = new GameObject("LimiteDireito");
        go2.transform.SetParent(go.transform, false);
        BoxCollider2D dir = go2.AddComponent<BoxCollider2D>();
        dir.size = new Vector2(1f, 200f);
        dir.offset = new Vector2(largura + 0.5f, 0f);
    }

    // ------------------------------------------------------------------ objetos

    static GameObject CriarMoeda(Sprite[] quadros, float x, float y)
    {
        GameObject go = new GameObject("Moeda");
        go.transform.position = new Vector3(x, y, 0f);
        CircleCollider2D cc = go.AddComponent<CircleCollider2D>();
        cc.isTrigger = true;
        cc.radius = 0.35f;
        Coletavel col = go.AddComponent<Coletavel>();

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(go.transform, false);
        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 5;
        AnimadorSprite a = visual.AddComponent<AnimadorSprite>();
        a.Tocar(quadros, 18f, true);
        col.visual = visual.transform;
        return go;
    }

    static void CriarInimigo(Sprite[] quadros, float x, float y, float xMin, float xMax)
    {
        GameObject go = new GameObject("Inimigo");
        go.transform.position = new Vector3(x, y, 0f);

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(0.55f, 0.7f);
        bc.offset = new Vector2(0f, 0.35f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 6;
        AnimadorSprite a = go.AddComponent<AnimadorSprite>();
        a.Tocar(quadros, 9f, true);

        InimigoPatrulha ip = go.AddComponent<InimigoPatrulha>();
        ip.xMin = xMin;
        ip.xMax = xMax;
    }

    static void CriarDecoracao(Sprite s, float x, float y)
    {
        if (s == null) return;
        GameObject go = new GameObject("Decoracao");
        go.transform.position = new Vector3(x, y, 0f);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = -5;
    }

    static bool Solido(string[] mapa, int x, int linha)
    {
        if (x < 0 || x >= mapa[0].Length) return true;
        if (linha < 0) return false;
        if (linha >= mapa.Length) return true;
        return mapa[linha][x] == '#';
    }

    // ------------------------------------------------------------------ fase completa

    public static void Construir(int numeroFase)
    {
        Recursos rec = Jogo.Rec;
        GameManager gm = GameManager.Garantir();
        string[] mapa = NivelDados.Obter(numeroFase);
        int altura = mapa.Length;
        int largura = mapa[0].Length;

        Camera cam = ConfigurarCamera();
        CriarCenario(cam, 0f);
        CriarChao(mapa);
        CriarParedes(largura);

        // Sprites
        Sprite[] quadrosMoeda = Jogo.Fatiar(rec.moeda, 32);
        Sprite[] andarA = Jogo.Fatiar(rec.inimigoA, 6);
        Sprite[] andarB = Jogo.Fatiar(rec.inimigoB, 6);
        Sprite arvore = Jogo.Recortar(rec.tileset, 208, 64, 112, 112, 0.45f, 0f);
        Sprite placaD = Jogo.Recortar(rec.tileset, 96, 96, 32, 32, 0.5f, 0f);
        Sprite placaL = Jogo.Recortar(rec.tileset, 128, 96, 32, 32, 0.5f, 0f);
        Sprite cerca = Jogo.Recortar(rec.tileset, 32, 160, 96, 32, 0.5f, 0f);
        Sprite poste = Jogo.Recortar(rec.tileset, 192, 0, 32, 64, 0.5f, 0f);
        Sprite pedra = Jogo.Recortar(rec.tileset, 32, 128, 32, 32, 0.5f, 0f);
        Sprite flores = Jogo.Recortar(rec.tileset, 32, 96, 32, 32, 0.5f, 0f);
        Sprite bau = Jogo.Recortar(rec.tileset, 32, 64, 32, 32, 0.5f, 0f);

        Vector2 inicio = new Vector2(2.5f, 3f);
        int totalMoedas = 0;

        for (int linha = 0; linha < altura; linha++)
        {
            int y = altura - 1 - linha;
            for (int x = 0; x < largura; x++)
            {
                char c = mapa[linha][x];
                float cx = x + 0.5f;
                switch (c)
                {
                    case 'P':
                        inicio = new Vector2(cx, y + 0.05f);
                        break;
                    case 'o':
                        CriarMoeda(quadrosMoeda, cx, y + 0.5f);
                        totalMoedas++;
                        break;
                    case 'e':
                    case 'E':
                        {
                            // Patrulha: até 4 células para cada lado, sem passar da borda da plataforma
                            int esq = x;
                            while (esq - 1 >= 0 && Solido(mapa, esq - 1, linha + 1) && !Solido(mapa, esq - 1, linha)) esq--;
                            int dir = x;
                            while (dir + 1 < largura && Solido(mapa, dir + 1, linha + 1) && !Solido(mapa, dir + 1, linha)) dir++;
                            esq = Mathf.Max(esq, x - 4);
                            dir = Mathf.Min(dir, x + 4);
                            CriarInimigo(c == 'e' ? andarA : andarB, cx, y + 0.02f, esq + 0.5f, dir + 0.5f);
                            break;
                        }
                    case 'T': CriarDecoracao(arvore, cx, y); break;
                    case 's': CriarDecoracao(placaD, cx, y); break;
                    case 'f': CriarDecoracao(cerca, cx, y); break;
                    case 'l': CriarDecoracao(poste, cx, y); break;
                    case 'r': CriarDecoracao(pedra, cx, y); break;
                    case 'w': CriarDecoracao(flores, cx, y); break;
                    case 'c': CriarDecoracao(bau, cx, y); break;
                }
            }
        }

        // Jogador
        GameObject jogador = new GameObject("Jogador");
        jogador.tag = "Player";
        jogador.transform.position = new Vector3(inicio.x, inicio.y, 0f);
        jogador.AddComponent<PlayerController2D>();

        // Câmera
        CameraSegue seguir = cam.GetComponent<CameraSegue>();
        if (seguir == null) seguir = cam.gameObject.AddComponent<CameraSegue>();
        seguir.jogador = jogador.transform;
        seguir.minX = 0f;
        seguir.maxX = largura;
        seguir.minY = -2f;
        seguir.Posicionar();

        Hud.Criar();

        gm.limiteQueda = -6f;
        gm.IniciarFase(numeroFase, totalMoedas);
    }
}
