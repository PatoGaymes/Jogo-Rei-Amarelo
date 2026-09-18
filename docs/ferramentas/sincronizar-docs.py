#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Alteracao de IA - Revisar
O que faz: compara os documentos "DoRepositorio" com as nossas copias locais e
           mostra o que esta diferente entre eles.
Por que: a equipe mantem a documentacao em dois lugares (aqui e no Google Drive).
         Sem uma conferencia automatica, uma decisao tomada num lugar se perde no
         outro. Este script roda na maquina e devolve so o resumo das diferencas,
         em vez de o Claude ler os dois documentos inteiros toda vez - o que
         gastaria a cota de uso a toa.

Como usar:
    python docs/ferramentas/sincronizar-docs.py            # so compara e mostra
    python docs/ferramentas/sincronizar-docs.py --converter # gera o .md do docx
"""
import difflib, io, os, re, sys, unicodedata, zipfile

try: sys.stdout.reconfigure(encoding='utf-8')
except Exception: pass

BASE = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'externo', 'Infos')

# (nome amigavel, docx do repositorio, nosso .md local)
PARES = [
    ('GDD',     'GDD Rei de AmareloDoRepositorio.docx',      'GDD Rei de Amarelo.md'),
    ('ARVORES', 'Arvores de HabilidadesDoRepositorio.docx',  'Arvores de Habilidades.md'),
]

def docx_para_texto(caminho):
    """Le um .docx (que por dentro e um zip) e devolve o texto limpo."""
    with zipfile.ZipFile(caminho) as z:
        xml = z.read('word/document.xml').decode('utf-8')
    xml = re.sub(r'</w:p>', '\n', xml)
    xml = re.sub(r'<w:tab[^>]*/>', '\t', xml)
    txt = re.sub(r'<[^>]+>', '', xml)
    for de, para in [('&amp;','&'),('&lt;','<'),('&gt;','>'),('&quot;','"'),('&apos;',"'")]:
        txt = txt.replace(de, para)
    return [l.strip() for l in txt.split('\n') if l.strip()]

def md_para_linhas(caminho):
    txt = io.open(caminho, encoding='utf-8').read()
    txt = re.sub(r'^#+ ', '', txt, flags=re.M)
    txt = txt.replace(r'\-', '-').replace(r'\>', '>').replace(r'\<', '<')
    txt = re.sub(r'\*\*?', '', txt)
    return [l.strip() for l in txt.split('\n') if l.strip()]

def sem_acento(t):
    """Troca letras acentuadas pela versao sem acento (Á vira a)."""
    t = unicodedata.normalize('NFD', t)
    return ''.join(c for c in t if unicodedata.category(c) != 'Mn')

def chave(t):
    return re.sub(r'[^a-z0-9]', '', sem_acento(t).lower())

def achar(nome_parcial):
    """Acha o arquivo mesmo que o nome tenha acento diferente."""
    alvo = chave(nome_parcial)
    for f in os.listdir(BASE):
        if chave(f) == alvo:
            return os.path.join(BASE, f)
    return None

def main():
    converter = '--converter' in sys.argv
    total_div = 0
    for nome, docx_nome, md_nome in PARES:
        pdocx, pmd = achar(docx_nome), achar(md_nome)
        if not pdocx or not pmd:
            print('[%s] arquivo nao encontrado - pulando' % nome); continue

        repo = docx_para_texto(pdocx)
        local = md_para_linhas(pmd)

        def limpar(linhas):
            """Tira o que e so formatacao, para sobrar apenas o conteudo."""
            saida = []
            for l in linhas:
                # nossas proprias anotacoes (blocos de citacao) nao contam como divergencia
                if l.lstrip().startswith('>'):
                    continue
                l = re.sub(r'\[([^\]]+)\]\([^)]*\)', r'', l)   # [texto](link) -> texto
                l = re.sub(r'\{#[^}]*\}', '', l)                  # ancoras {#id}
                l = re.sub(r'HYPERLINK[^"]*"[^"]*"', '', l)      # campos de hyperlink do Word
                l = re.sub(r'^[0-9]+\.\s*', '', l)               # numeracao de sumario
                # marcador de lista: "-X", "- X" e "* X" viram todos a mesma coisa,
                # senao um espaco a mais acusa diferenca que nao existe
                l = re.sub(r'^[-*]\s*', '', l)
                l = re.sub(r'\s+', ' ', l).strip()
                if l: saida.append(l)
            return saida

        repo, local = limpar(repo), limpar(local)
        sm = difflib.SequenceMatcher(None, repo, local)
        semelhanca = sm.ratio() * 100

        so_repo, so_local = [], []
        for tag, i1, i2, j1, j2 in sm.get_opcodes():
            if tag in ('delete', 'replace'): so_repo += repo[i1:i2]
            if tag in ('insert', 'replace'): so_local += local[j1:j2]

        # linhas muito curtas costumam ser diferenca de formatacao, nao de conteudo
        so_repo = [l for l in so_repo if len(l) > 25]
        so_local = [l for l in so_local if len(l) > 25]
        total_div += len(so_repo) + len(so_local)

        print('=' * 70)
        print('%s  -  %.1f%% igual  |  repositorio: %d linhas, local: %d linhas'
              % (nome, semelhanca, len(repo), len(local)))
        print('=' * 70)
        if not so_repo and not so_local:
            print('  Nenhuma diferenca de conteudo.')
        else:
            if so_repo:
                print('  SO NO REPOSITORIO (%d):' % len(so_repo))
                for l in so_repo[:12]: print('    +', l[:130])
                if len(so_repo) > 12: print('    ... e mais %d' % (len(so_repo)-12))
            if so_local:
                print('  SO NA NOSSA COPIA (%d):' % len(so_local))
                for l in so_local[:12]: print('    -', l[:130])
                if len(so_local) > 12: print('    ... e mais %d' % (len(so_local)-12))
        print()

        if converter:
            destino = pmd.replace('.md', 'DoRepositorio.md')
            io.open(destino, 'w', encoding='utf-8').write('\n\n'.join(repo))
            print('  gerado: %s' % os.path.basename(destino))

    print('-' * 70)
    if total_div == 0:
        print('RESULTADO: documentacao sincronizada.')
    else:
        print('RESULTADO: %d linhas divergentes. Decidir qual lado vale antes de aplicar.' % total_div)
    return 0

if __name__ == '__main__':
    sys.exit(main())
