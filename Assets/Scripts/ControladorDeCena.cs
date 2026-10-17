using UnityEngine;

public enum TipoCena { Menu, Fase, Final }

/// <summary>
/// Único objeto necessário em cada cena: monta o menu, a fase ou a tela final.
/// Cenas: MenuInicial (Menu), Fase1 e Fase2 (Fase), CenaFinal (Final).
/// </summary>
public class ControladorDeCena : MonoBehaviour
{
    public TipoCena tipo = TipoCena.Fase;
    public int numeroFase = 1;

    void Start()
    {
        GameManager.Garantir();
        switch (tipo)
        {
            case TipoCena.Menu: MenuInicial.Construir(); break;
            case TipoCena.Fase: ConstrutorFase.Construir(numeroFase); break;
            case TipoCena.Final: TelaFinal.Construir(); break;
        }
    }
}
