# Plataforma — Monstrinho Coletor

Jogo de plataforma 2D (Unity 6, URP 2D, novo Input System).

## Como abrir
1. Abra a pasta no Unity Hub (versão 6000.6.0f1, a mesma de `ProjectSettings/ProjectVersion.txt`).
2. Abra `Assets/Scenes/MenuInicial.unity` e aperte Play (ou rode o build: as 4 cenas já estão no Build Settings).

## Cenas (Build Settings)
| # | Cena | O que faz |
|---|------|-----------|
| 0 | MenuInicial | Tela inicial com as instruções de como jogar |
| 1 | Fase1 | Fase 1 (16 moedas, 4 inimigos) |
| 2 | Fase2 | Fase 2 (22 moedas, 9 inimigos, buracos) |
| 3 | CenaFinal | VITÓRIA ou DERROTA, pontuação e botão "Jogar novamente" |

Cada cena só tem Câmera, Global Light 2D e um `ControladorDeCena`, que monta tudo por código
(`ConstrutorFase`, `MenuInicial`, `TelaFinal`). Dá para abrir qualquer fase direto e apertar Play.

## Controles
- Setas ou A/D: andar
- Espaço, seta para cima ou W: pular (segure para pular mais alto)
- Pisar na cabeça do inimigo derrota ele (+100). Moeda +10.

## Regras
- 3 vidas. Inimigo encostando pelo lado ou cair no buraco tira 1 vida. Zero vidas = derrota.
- Coletar todas as moedas da fase avança; coletar todas da Fase 2 dá a vitória.

## Editar as fases
Os mapas estão em `Assets/Scripts/NivelDados.cs` (um caractere = uma célula de 1x1):
`#` chão, `P` início, `o` moeda, `e`/`E` inimigos, `T s f l r w c` decoração.

## Arquivos principais
- `Assets/Resources/Recursos.asset`: referências das imagens usadas (tileset, sprites, fundos).
- `Assets/Scripts/`: `GameManager`, `PlayerController2D`, `InimigoPatrulha`, `Coletavel`, `CameraSegue`,
  `Parallax`, `Hud`, `UIHelper`, `AnimadorSprite`, `ConstrutorFase`, `NivelDados`, ...
- `Assets/Scenes/_antigo/SampleScene.unity`: a cena antiga, guardada só como referência.
