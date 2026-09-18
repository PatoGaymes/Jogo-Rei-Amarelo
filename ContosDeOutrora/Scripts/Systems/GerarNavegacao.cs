using Godot;

// Alteração de IA - Revisar
// O que faz: calcula o "mapa de caminhos" da sala assim que o jogo abre.
// Por quê: o inimigo precisa saber por onde dá para andar, senão tentaria atravessar as
//          paredes em linha reta e ficaria preso nelas. Esse mapa é o que permite a ele
//          contornar obstáculos.
//
//          É calculado na hora de abrir, em vez de ficar salvo no arquivo da cena, porque
//          assim ele acompanha qualquer mudança no cenário: mover uma parede no editor já
//          reflete no caminho, sem ninguém precisar lembrar de recalcular.
public partial class GerarNavegacao : NavigationRegion3D
{
	public override void _Ready()
	{
		if (NavigationMesh == null)
		{
			GD.PushWarning("GerarNavegacao: não há malha de navegação configurada neste nó.");
			return;
		}

		BakeNavigationMesh();
	}
}
