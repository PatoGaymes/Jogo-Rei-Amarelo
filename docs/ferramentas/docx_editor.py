#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Alteracao de IA - Revisar

O que faz: ferramentas para alterar um documento .docx **sem perder a formatacao dele**.

Por que: os documentos do projeto sao lidos por outras pessoas da equipe, entao precisam
         manter a aparencia original - titulos, fontes, sumario, espacamento. Gerar um
         .docx do zero a partir de texto simples perde tudo isso e entrega um documento
         com cara diferente do resto da documentacao.

         Por dentro, um .docx e um arquivo zip com varios XML. A abordagem daqui e:
         abrir o original, mexer **so no texto** dentro de word/document.xml, e gravar
         de volta copiando todo o resto (estilos, tema, numeracao, cabecalho) intacto.

Como usar (exemplo):

    from docx_editor import Documento

    doc = Documento('original.docx')
    doc.substituir_texto('sede de sangue', 'Morte lenta')
    doc.remover_paragrafo_com('Quests do ferreiro')
    doc.salvar('novo.docx')
    print(doc.relatorio())

Sempre conferir o resultado depois (ver `verificar` no fim do arquivo).
"""
import io
import os
import re
import shutil
import subprocess
import sys
import zipfile

try:
    sys.stdout.reconfigure(encoding='utf-8')
except Exception:
    pass

ALVO_XML = 'word/document.xml'


def _escapar(texto):
    """Prepara o texto para entrar no XML (& < > tem significado especial la)."""
    return texto.replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;')


def _desescapar(texto):
    for de, para in [('&amp;', '&'), ('&lt;', '<'), ('&gt;', '>'),
                     ('&quot;', '"'), ('&apos;', "'")]:
        texto = texto.replace(de, para)
    return texto


class Documento:
    """Um .docx aberto para edicao, preservando a formatacao original."""

    def __init__(self, caminho):
        self.origem = caminho
        with zipfile.ZipFile(caminho) as z:
            self.xml = z.read(ALVO_XML).decode('utf-8')
        self._log = []
        self._paragrafos_inicio = self.contar_paragrafos()

    # ---------- leitura ----------

    def paragrafos(self):
        """Lista os paragrafos como (inicio, fim, texto) dentro do XML."""
        saida = []
        for m in re.finditer(r'<w:p [^>]*>.*?</w:p>', self.xml, re.S):
            texto = ''.join(re.findall(r'<w:t[^>]*>(.*?)</w:t>', m.group(0), re.S))
            saida.append((m.start(), m.end(), _desescapar(texto).strip()))
        return saida

    def contar_paragrafos(self):
        return len(re.findall(r'<w:p [^>]*>', self.xml))

    def texto(self):
        """Todo o texto do documento, sem marcacao."""
        return '\n'.join(t for _, _, t in self.paragrafos() if t)

    def achar(self, trecho):
        """Indices dos paragrafos que contem o trecho."""
        return [i for i, (_, _, t) in enumerate(self.paragrafos()) if trecho in t]

    # ---------- escrita ----------

    def _variantes(self, texto):
        """Gera as formas que o texto pode ter dentro do XML.

        O Word grava aspas e apostrofos de varios jeitos: aspas retas viram &quot;,
        e editores costumam trocar por aspas curvas. Sem prever isso, uma busca
        aparentemente certa nao encontra nada."""
        base = _escapar(texto)
        formas = {base}
        for aspas in ['&quot;', '"', '“', '”']:
            formas.add(re.sub(r'["“”]|&quot;', aspas, base))
        saida = set()
        for f in formas:
            saida.add(f)
            saida.add(f.replace("'", '’'))
            saida.add(f.replace('’', "'"))
        return saida

    def substituir_texto(self, velho, novo, esperado=1):
        """Troca um trecho de texto. Avisa se aparecer numa quantidade diferente da
        esperada - assim uma troca que pegaria o lugar errado nao passa despercebida."""
        for alvo in sorted(self._variantes(velho), key=len, reverse=True):
            achados = self.xml.count(alvo)
            if achados == esperado:
                # a troca precisa usar as mesmas aspas que o alvo encontrado
                troca = _escapar(novo)
                if '&quot;' in alvo:
                    troca = re.sub(r'["“”]', '&quot;', troca)
                elif '“' in alvo or '”' in alvo:
                    troca = troca.replace('"', '”')
                if '’' in alvo:
                    troca = troca.replace("'", '’')
                self.xml = self.xml.replace(alvo, troca)
                self._log.append('trocado   "%s" -> "%s"' % (velho[:40], novo[:40]))
                return True
        encontrados = max((self.xml.count(a) for a in self._variantes(velho)), default=0)
        self._log.append('IGNORADO  "%s": %d ocorrencia(s), esperava %d'
                         % (velho[:45], encontrados, esperado))
        return False

    def remover_paragrafo_com(self, trecho):
        """Apaga o paragrafo inteiro que contem o trecho."""
        alvo = _escapar(trecho)
        i = self.xml.find(alvo)
        if i < 0:
            self._log.append('IGNORADO  paragrafo com "%s" nao encontrado' % trecho[:45])
            return False
        ini = self.xml.rfind('<w:p ', 0, i)
        fim = self.xml.find('</w:p>', i)
        if ini < 0 or fim < 0:
            self._log.append('IGNORADO  nao achei os limites do paragrafo "%s"' % trecho[:45])
            return False
        self.xml = self.xml[:ini] + self.xml[fim + len('</w:p>'):]
        self._log.append('removido  paragrafo "%s"' % trecho[:45])
        return True

    def molde(self, indice):
        """Pega um paragrafo existente para servir de modelo de formatacao."""
        ini, fim, _ = self.paragrafos()[indice]
        return self.xml[ini:fim]

    def paragrafo_novo(self, texto, molde_xml):
        """Cria um paragrafo com a mesma formatacao do molde, trocando o texto."""
        novo = re.sub(r'(<w:t[^>]*>).*?(</w:t>)',
                      lambda m: m.group(1) + _escapar(texto) + m.group(2),
                      molde_xml, count=1, flags=re.S)
        runs = re.findall(r'<w:r[ >].*?</w:r>', novo, re.S)
        for r in runs[1:]:          # se o molde tinha varios pedacos, mantem so o primeiro
            novo = novo.replace(r, '', 1)
        return novo

    def substituir_faixa(self, do_paragrafo, ate_paragrafo, xml_novo, rotulo=''):
        """Troca um intervalo de paragrafos por outro conteudo.
        ATENCAO: os indices mudam a cada edicao. Fazer as trocas de tras para frente."""
        P = self.paragrafos()
        ini = P[do_paragrafo][0]
        fim = P[ate_paragrafo][1]
        self.xml = self.xml[:ini] + xml_novo + self.xml[fim:]
        self._log.append('faixa     paragrafos %d-%d %s' % (do_paragrafo, ate_paragrafo, rotulo))

    # ---------- gravacao ----------

    def salvar(self, destino):
        """Grava o .docx copiando o original e trocando so o texto.
        Estilos, tema, numeracao e cabecalho vao intactos."""
        pasta = os.path.dirname(destino)
        if pasta:
            os.makedirs(pasta, exist_ok=True)
        if os.path.exists(destino):
            os.remove(destino)
        dados = self.xml.encode('utf-8')
        with zipfile.ZipFile(self.origem) as zin, \
             zipfile.ZipFile(destino, 'w', zipfile.ZIP_DEFLATED) as zout:
            for item in zin.infolist():
                conteudo = dados if item.filename == ALVO_XML else zin.read(item.filename)
                zi = zipfile.ZipInfo(item.filename, date_time=item.date_time)
                zi.compress_type = item.compress_type
                zi.external_attr = item.external_attr
                zout.writestr(zi, conteudo)
        return destino

    def relatorio(self):
        linhas = ['  ' + l for l in self._log]
        linhas.append('  paragrafos: %d -> %d' % (self._paragrafos_inicio, self.contar_paragrafos()))
        return '\n'.join(linhas)


# ---------- conferencia ----------

def _pandoc():
    p = shutil.which('pandoc')
    if p:
        return p
    for c in [os.path.expandvars(r'%LOCALAPPDATA%\Pandoc\pandoc.exe'),
              r'C:\Program Files\Pandoc\pandoc.exe']:
        if os.path.isfile(c):
            return c
    return None


def ler_texto(caminho_docx):
    """Le o texto de um .docx usando o pandoc (para conferir o resultado)."""
    pd = _pandoc()
    if not pd:
        return None
    r = subprocess.run([pd, caminho_docx, '-t', 'plain'], capture_output=True)
    return r.stdout.decode('utf-8', errors='replace')


def verificar(original, novo, deve_sumir=(), deve_existir=()):
    """Confere o resultado: estrutura do arquivo, o que saiu e o que continua la.

    deve_sumir   - trechos que NAO podem mais aparecer
    deve_existir - trechos que PRECISAM continuar aparecendo (lore, personagens...)
    """
    print('  conferindo %s' % os.path.basename(novo))
    with zipfile.ZipFile(original) as a, zipfile.ZipFile(novo) as b:
        estrutura_ok = a.namelist() == b.namelist() and b.testzip() is None
    print('    estrutura interna igual ao original: %s' % ('sim' if estrutura_ok else 'NAO'))

    tn = ler_texto(novo)
    if tn is None:
        print('    (pandoc nao encontrado - conferencia de texto pulada)')
        return estrutura_ok

    problemas = 0
    for t in deve_sumir:
        n = tn.lower().count(t.lower())
        if n:
            print('    AINDA APARECE: "%s" (%d vez(es))' % (t, n))
            problemas += 1
    for t in deve_existir:
        if t.lower() not in tn.lower():
            print('    SUMIU: "%s"' % t)
            problemas += 1
    if not problemas:
        print('    texto conferido: nada indevido saiu, nada indevido ficou')
    return estrutura_ok and problemas == 0
