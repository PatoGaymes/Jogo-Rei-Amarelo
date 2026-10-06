#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Alteracao de IA - Revisar

O que faz: gera as versoes corrigidas dos documentos do Drive com as decisoes de combate de
           06/10/2026 (mover por hexagonos, alcance em casas, iniciativa d20, Vanguarda nova,
           Foco como recurso) e com as grafias oficiais, a partir da versao ATUAL do Drive.
           Grava em docs/para-repositorio/2026-10-06/, para um dev revisar e subir na pasta
           "Pato Games/Revisao" do Drive.

Por que: desde 06/10/2026 a documentacao do projeto (aqui, fora do Drive) e a mais atualizada;
         o que estiver diferente no Drive deve ir corrigido para a pasta de revisao. Os
         documentos sao lidos pela equipe, entao nada e gerado do zero: o script abre o
         original e troca so o texto, mantendo estilos, titulos, numeracao e sumario.

         A base de cada documento e a versao do Drive desse dia:
           - Arvores de Habilidades: igual a docs/para-repositorio/ (subida em 18/09);
           - GDD: o Drive ganhou o atributo Iniciativa em 06/10 - baixar o .docx atual e
             passar o caminho em --gdd.

Como usar:
    python docs/ferramentas/revisao-2026-10-06.py --gdd "caminho/do/GDD do Drive.docx"

O que cada troca faz esta listado em docs/MUDANCAS-DOCUMENTACAO.md (06/10/2026).
"""
import argparse
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from docx_editor import Documento, verificar          # noqa: E402

try:
    sys.stdout.reconfigure(encoding='utf-8')
except Exception:
    pass

RAIZ = os.path.abspath(os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', '..'))
SAIDA = os.path.join(RAIZ, 'docs', 'para-repositorio', '2026-10-06')

# "energia" como recurso das habilidades vira "Foco" (decisao do PO em 06/10/2026). Os NOMES
# das habilidades ("Energia comprimida", "Manipulacao de energia") e o atributo Energia ficam.
ENERGIA_PARA_FOCO = [
    ('Energia comprimida: causa dano e recupera energia', 'Energia comprimida: causa dano e recupera Foco'),
    ('Renovação: recupera uma certa quantidade de energia', 'Renovação: recupera uma certa quantidade de Foco'),
    ('quando um aliado consome mais do que uma certa quantidade de energia, você recupera metade do gasto da energia',
     'quando um aliado consome mais do que uma certa quantidade de Foco, você recupera metade do Foco gasto'),
    ('Sangue grosso: ao perder vida, recupera energia e sanidade', 'Sangue grosso: ao perder vida, recupera Foco e sanidade'),
    ('não gasta energia nas habilidades', 'não gasta Foco nas habilidades'),
    ('Foco total: recupera energia, entra e sai na postura oculta', 'Foco total: recupera Foco, entra e sai na postura oculta'),
    ('Seu ataque básico agora gasta energia', 'Seu ataque básico agora gasta Foco'),
    ('Feitiço inato: gastar energia recupera', 'Feitiço inato: gastar Foco recupera'),
    ('Verde: Recupera energia no acerto', 'Verde: Recupera Foco no acerto'),
    ('recupera vida e energia para xamã', 'recupera vida e Foco para a xamã'),
    ('todos os aliados a sua frente recuperam energia', 'todos os aliados a sua frente recuperam Foco'),
    ('Puritano: Seus pontos de energia aumentam', 'Puritano: Seus pontos de Foco aumentam'),
    ('aumenta os pontos de energia e sanidade máximos', 'aumenta os pontos de Foco e sanidade máximos'),
    ('não gastam mais energia para serem lançados', 'não gastam mais Foco para serem lançados'),
]


def arvores():
    print('== Árvores de Habilidades ==')
    orig = os.path.join(RAIZ, 'docs', 'para-repositorio', 'Árvores de Habilidades.docx')
    novo = os.path.join(SAIDA, 'Árvores de Habilidades.docx')
    d = Documento(orig)

    # as acoes do turno e o que passou a valer no combate em hexagonos
    d.substituir_texto('-Os personagens tem 4 ações: atacar/usar habilidades, usar itens, defender, fugir',
                       '-Os personagens têm 5 ações: atacar, usar habilidades, usar itens, defender e fugir')
    P = d.paragrafos()
    i_uma_acao = next(i for i, (_, _, t) in enumerate(P) if t.startswith('-cada personagem só pode fazer uma ação por turno'))
    molde = d.molde(i_uma_acao)
    novos = ''.join(d.paragrafo_novo(t, molde) for t in [
        '-Todos, personagens e inimigos, também podem se mover: até 3 casas (hexágonos) por turno. Andar não gasta '
        'a ação — dá para andar e atacar no mesmo turno, a não ser que alguma condição impeça',
        '-O alcance dos ataques e das habilidades também é contado em casas',
        '-Iniciativa: no começo do combate, cada um rola um dado de 20 lados e soma o atributo Iniciativa; a ordem vai do '
        'maior para o menor, misturando aliados e inimigos. Quem tira 20 no dado joga 2 turnos seguidos sempre que chega a vez dele',
        '-O recurso gasto pelas habilidades é o Foco',
    ])
    d.substituir_faixa(i_uma_acao, i_uma_acao, molde + novos, '(mover, alcance, iniciativa e Foco)')

    # Vanguarda: e a condicao de quem protege
    d.substituir_texto('-vanguarda: a unidade com essa condição é protegida de qualquer ataque lançado a ela',
                       '-vanguarda: condição de quem protege. O aliado protegido fica "Protegido": todo o dano que ele '
                       'receberia vai para a Vanguarda; em dano de área, metade fica com o protegido e metade vai para '
                       'a Vanguarda. Dá para proteger vários aliados, um por uso')

    for velho, novo_texto in ENERGIA_PARA_FOCO:
        d.substituir_texto(velho, novo_texto)

    # grafias oficiais
    d.substituir_texto('conforme a sanidade da cartomante', 'conforme a sanidade da xamã')
    d.substituir_texto('-Escurdeiro-', '-Escudeiro-')

    d.salvar(novo)
    print(d.relatorio())
    verificar(orig, novo,
              deve_sumir=['tem 4 ações', 'é protegida de qualquer ataque', 'recupera energia', 'gasta energia',
                          'cartomante', 'Escurdeiro'],
              deve_existir=['5 ações', 'até 3 casas', 'dado de 20 lados', 'condição de quem protege',
                            'Energia comprimida', 'recupera Foco', 'Morte lenta',
                            'Cavaleira', 'Caçadora', 'Peregrino', 'Desgarrado', 'Hemomante', 'Aberração',
                            'Xamã', 'Escudeiro', 'Brutamonte', 'Bruxo', 'Espinhos'])
    print()


def gdd(base):
    print('== GDD ==')
    novo = os.path.join(SAIDA, 'GDD Rei de Amarelo.docx')
    d = Documento(base)

    d.substituir_texto('A movimentação é de apenas para trás e frente, com interações específicas e também escondidas no cenário.',
                       'A movimentação é livre pelo mapa 3D isométrico, com a câmera girando em 8 ângulos em volta do '
                       'personagem (referência: Don\'t Starve Together), com interações específicas e também escondidas no cenário.')

    # grafias oficiais (docs/JOGO.md): Tao, Jedara, Xamã — no sumário e no corpo
    for velho, novo_texto in [('Thao A’Bajal (Peregrino)', 'Tao A’Bajal (Peregrino)'),
                              ('Thao desde cedo', 'Tao desde cedo'),
                              ('Jedah, Filho de Tauron', 'Jedara, Filho de Tauron'),
                              ('Amana K\'Ushim(Cartomante)', 'Amana K\'Ushim (Xamã)')]:
        quantos = max(d.xml.count(v) for v in d._variantes(velho))
        d.substituir_texto(velho, novo_texto, esperado=max(1, quantos))

    d.salvar(novo)
    print(d.relatorio())
    verificar(base, novo,
              deve_sumir=['apenas para trás e frente', 'Thao', 'Jedah', 'Cartomante'],
              deve_existir=['3D isométrico', 'Tao A', 'Jedara', 'Iniciativa:',
                            # lore que precisa continuar la
                            'Antiga Válquia', 'Yggdrasil', 'Dalsebria', 'Abanur',
                            'Khalid', 'Emi Matsunaga', 'Uzhan', 'Lancelot'])
    print()


if __name__ == '__main__':
    argumentos = argparse.ArgumentParser(description=__doc__.split('\n')[3])
    argumentos.add_argument('--gdd', required=True, help='o .docx do GDD baixado do Drive (versão atual)')
    a = argumentos.parse_args()
    os.chdir(RAIZ)
    os.makedirs(SAIDA, exist_ok=True)
    arvores()
    gdd(a.gdd)
    print('Prontos em docs/para-repositorio/2026-10-06/ — formatação original preservada.')
