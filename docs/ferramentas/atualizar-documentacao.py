#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Alteracao de IA - Revisar

O que faz: aplica nos documentos oficiais (.docx) as decisoes que a equipe tomou, e grava
           o resultado em docs/para-repositorio/, pronto para o analista colar no Drive.

Por que: o Claude nao tem acesso ao Google Drive (Regra 3), entao tudo que muda aqui
         precisa ser repassado a mao. Este script fecha esse caminho **mantendo a
         formatacao original de cada documento** - os arquivos sao lidos por outras
         pessoas da equipe e precisam continuar com a mesma aparencia.

         Ele nao escreve um documento novo: abre o original e mexe so no texto.

Como usar:
    python docs/ferramentas/atualizar-documentacao.py

A cada nova rodada de decisoes, acrescentar as mudancas aqui e rodar de novo.
O historico do que ja foi aplicado fica em docs/MUDANCAS-DOCUMENTACAO.md.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from docx_editor import Documento, verificar          # noqa: E402

RAIZ = os.path.abspath(os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', '..'))
INFOS = os.path.join(RAIZ, 'docs', 'externo', 'Infos')
SAIDA = os.path.join(RAIZ, 'docs', 'para-repositorio')

AVISO_1 = 'ATENÇÃO — seção desatualizada, substituída em 18/09/2026.'
AVISO_2 = ('Sistema de status, condições e ações de combate: ver o documento '
           '"Árvores de Habilidades", que é a fonte oficial e mais atualizada. '
           'As regras que ficavam aqui estão desatualizadas.')


def arvores():
    print('== Árvores de Habilidades ==')
    orig = os.path.join(INFOS, 'Árvores de HabilidadesDoRepositorio.docx')
    novo = os.path.join(SAIDA, 'Árvores de Habilidades.docx')
    d = Documento(orig)

    # habilidades renomeadas: quem as citava pelo nome antigo ficou para tras
    d.substituir_texto('marcados por "sede de sangue"', 'marcados por "Morte lenta"')
    d.substituir_texto('"Dance": Sede de sangue agora marca', '"Dance": Morte lenta agora marca')
    d.substituir_texto('deixa poças que aplicam ácido e corrosão',
                       'deixa poças que aplicam veneno e corrosão')

    d.salvar(novo)
    print(d.relatorio())
    verificar(orig, novo,
              deve_sumir=['sede de sangue', 'ácido e corrosão'],
              deve_existir=['Morte lenta', 'veneno', 'Cavaleira', 'Caçadora', 'Espinhos'])
    print()


def demo():
    print('== Demo ==')
    orig = os.path.join(INFOS, 'Demo.docx')
    novo = os.path.join(SAIDA, 'Demo.docx')
    d = Documento(orig)

    d.remover_paragrafo_com('Quests do ferreiro')      # saiu do escopo da demo
    d.substituir_texto('Caçador/pronto', 'Caçadora/pronto')

    d.salvar(novo)
    print(d.relatorio())
    verificar(orig, novo,
              deve_sumir=['ferreiro'],
              deve_existir=['Caçadora', 'Brutamonte', 'Xamã', 'Hemomante',
                            'minas subterrâneas', 'não respawna'])
    print()


def gdd():
    print('== GDD ==')
    orig = os.path.join(INFOS, 'GDD Rei de AmareloDoRepositorio.docx')
    novo = os.path.join(SAIDA, 'GDD Rei de Amarelo.docx')
    d = Documento(orig)

    P = d.paragrafos()

    def indice(texto, a_partir_de=50):
        """Acha o paragrafo pelo texto, pulando o sumario do comeco do documento."""
        for i, (_, _, t) in enumerate(P):
            if i >= a_partir_de and t.strip().startswith(texto):
                return i
        raise SystemExit('nao achei a seção "%s" no GDD' % texto)

    i_combate = indice('Combate')
    i_atributos = indice('Atributos')
    i_status = indice('Status, Efeitos')
    i_progressao = indice('Sistema de progress')

    molde = d.molde(i_combate + 2)                     # um paragrafo de corpo comum
    aviso = d.paragrafo_novo(AVISO_1, molde) + d.paragrafo_novo(AVISO_2, molde)

    # os indices mudam a cada edicao, entao as trocas vao de tras para frente
    d.substituir_faixa(i_status + 1, i_progressao - 1, aviso, '(seção Status -> aviso)')

    i_labia = indice('Lábia:')
    i_analise = indice('Análise:')
    d.substituir_faixa(i_labia, i_analise, '', '(Lábia, Intuição e Análise removidas)')

    i_furt = indice('Furtividade:')
    d.substituir_faixa(i_furt, i_furt, d.paragrafo_novo(
        'Furtividade: Capacidade de se esconder. Diminui passivamente o agro inimigo em '
        'combate e, na exploração, reduz o raio em que os inimigos percebem o personagem — '
        'quanto maior a Furtividade, mais perto dá para chegar sem ser notado;', molde),
        '(Furtividade ganhou o raio de agro)')

    i_mente = indice('-Mente')
    d.substituir_faixa(i_mente, i_mente, d.paragrafo_novo(
        '-Mente: responsável por administrar Sanidade e a perícia Vontade.', molde),
        '(atributo Mente sem as perícias removidas)')

    d.substituir_faixa(i_combate + 1, i_atributos - 1, aviso, '(seção Combate -> aviso)')

    i_respawn = indice('Transicionar de uma área')
    d.substituir_faixa(i_respawn, i_respawn, d.paragrafo_novo(
        'Inimigos e loot são fixos e não respawnam. Está previsto criar, depois da demo, '
        'um item ou mecânica que faça um inimigo ou uma área inteira voltar a aparecer — '
        'mas isso não entra no escopo da demo.', molde),
        '(respawn corrigido)')

    d.salvar(novo)
    print(d.relatorio())
    verificar(orig, novo,
              deve_sumir=['Lábia:', 'Intuição:', 'Análise:',
                          'Transicionar de uma área para outra reseta'],
              deve_existir=['seção desatualizada', 'não respawnam', 'raio em que os inimigos',
                            'Sanidade e a perícia Vontade',
                            # lore que precisa continuar la
                            'Antiga Válquia', 'Yggdrasil', 'Dalsebria', 'Abanur',
                            'Khalid', 'Emi Matsunaga', 'Uzhan', 'Lancelot'])
    print()


if __name__ == '__main__':
    os.chdir(RAIZ)
    os.makedirs(SAIDA, exist_ok=True)
    arvores()
    demo()
    gdd()
    print('Prontos em docs/para-repositorio/ — formatação original preservada.')
