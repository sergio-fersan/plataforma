using UnityEngine;

/// <summary>
/// Referências para as imagens do jogo. O asset fica em Assets/Resources/Recursos.asset
/// e é carregado por Resources.Load, então as cenas não precisam de referências manuais.
/// </summary>
[CreateAssetMenu(fileName = "Recursos", menuName = "Plataforma/Recursos")]
public class Recursos : ScriptableObject
{
    [Header("Cenário")]
    public Texture2D tileset;
    public Texture2D ceu;
    public Texture2D montanhas;
    public Texture2D colinas;

    [Header("Itens")]
    public Texture2D moeda;

    [Header("Jogador")]
    public Texture2D jogadorParado;
    public Texture2D jogadorCorrendo;
    public Texture2D jogadorPulando;
    public Texture2D jogadorDano;

    [Header("Inimigos")]
    public Texture2D inimigoA;
    public Texture2D inimigoB;
}
