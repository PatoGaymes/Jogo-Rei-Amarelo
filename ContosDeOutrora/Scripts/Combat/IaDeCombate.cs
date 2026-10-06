using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Alteração de IA - Revisar
// O que faz: o turno de um inimigo — escolhe quem atacar, anda até uma casa de onde alcança e ataca.
// Por quê: simples de propósito, para testar a mecânica: os inimigos de hoje só têm o ataque
//          básico. A escolha do alvo já respeita a Furtividade ("reduz o agro inimigo", GDD) e a
//          Alma imaculada (se a intenção foi revelada, o inimigo cumpre).
public sealed class IaDeCombate
{
	private readonly GerenciadorDeCombate _combate;

	public IaDeCombate(GerenciadorDeCombate combate)
	{
		_combate = combate;
	}

	// Alteração de IA - Revisar
	// O que faz: escolhe o alvo — o aliado mais fácil de alcançar; a Furtividade de cada aliado o
	//            faz parecer mais longe; empate fica com quem tem menos vida.
	public Combatente? EscolherAlvo(Combatente inimigo)
	{
		if (inimigo.Casa == null)
		{
			return null;
		}

		Combatente? melhor = null;
		float menor = float.MaxValue;
		foreach (Combatente aliado in _combate.Combatentes.Where(c => c.Vivo && c.Lado != inimigo.Lado && c.Casa != null))
		{
			int passos = _combate.PassosAte(inimigo, aliado.Casa!);
			float custo = passos + aliado.Ficha.Furtividade * 0.5f + aliado.Vida * 0.01f;
			if (custo < menor)
			{
				menor = custo;
				melhor = aliado;
			}
		}
		return melhor;
	}

	// Alteração de IA - Revisar
	// O que faz: joga o turno inteiro do inimigo.
	public async Task JogarTurno(Combatente inimigo)
	{
		Habilidade? ataque = inimigo.Ficha.AtaqueBasico;
		Combatente? alvo = inimigo.AlvoPrevisto is { Vivo: true } previsto ? previsto : EscolherAlvo(inimigo);
		if (alvo?.Casa == null || inimigo.Casa == null || ataque == null)
		{
			return;
		}

		await _combate.Esperar(0.35f);

		if (!_combate.PodeAtingir(inimigo, ataque, inimigo.Casa, alvo.Casa) && inimigo.MovimentoRestante > 0)
		{
			Casa? destino = MelhorCasaParaAtacar(inimigo, ataque, alvo);
			if (destino != null && destino != inimigo.Casa)
			{
				await _combate.Mover(inimigo, destino);
			}
		}

		if (!inimigo.Vivo || !alvo.Vivo || alvo.Casa == null || inimigo.Casa == null)
		{
			return;
		}

		if (inimigo.AcaoDisponivel && !inimigo.HabilidadeBloqueada(ataque)
			&& _combate.PodeAtingir(inimigo, ataque, inimigo.Casa, alvo.Casa))
		{
			await _combate.Esperar(0.15f);
			await _combate.Executor.Usar(inimigo, ataque, alvo.Casa);
		}
		await _combate.Esperar(0.3f);
	}

	// Alteração de IA - Revisar
	// O que faz: entre as casas que ele alcança neste turno, a mais perto de onde ele está que já
	//            permite atacar; se nenhuma permite, a que mais o aproxima do alvo.
	private Casa? MelhorCasaParaAtacar(Combatente inimigo, Habilidade ataque, Combatente alvo)
	{
		Dictionary<Casa, int> alcancaveis = _combate.CasasAlcancaveis(inimigo);
		var paradas = alcancaveis.Where(par => _combate.PodeParar(inimigo, par.Key)).ToList();

		var deOndeAtaca = paradas
			.Where(par => _combate.PodeAtingir(inimigo, ataque, par.Key, alvo.Casa!))
			.OrderBy(par => par.Value)
			.Select(par => par.Key)
			.FirstOrDefault();
		if (deOndeAtaca != null)
		{
			return deOndeAtaca;
		}

		Casa? maisPerto = null;
		int menor = int.MaxValue;
		foreach (var par in paradas)
		{
			int passos = _combate.PassosEntre(par.Key, alvo.Casa!, inimigo);
			if (passos < menor)
			{
				menor = passos;
				maisPerto = par.Key;
			}
		}
		return maisPerto;
	}
}
