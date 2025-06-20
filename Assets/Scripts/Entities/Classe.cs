using Assets.Scripts.Entities.Itens;
using NUnit.Framework;
using System.Collections.Generic;

namespace Assets.Scripts.Entities
{
    [System.Serializable]
    public class Classe
    {
        public string Nome;
        public string TipoAtaque;
        public Arma ArmaInicial;
        public List<Habilidade> HabilidadesDeClasse;

        public Classe(string nome, string tipoAtaque, Arma armaInicial, List<Habilidade> habilidadesDeClasse)
        {
            Nome = nome;
            TipoAtaque = tipoAtaque;
            ArmaInicial = armaInicial;
            HabilidadesDeClasse = habilidadesDeClasse;
        }

        public void GetHabilidadesDeClasse(Personagem personagem)
        {
            switch(personagem.Nivel)
            {
                case 1:
                    personagem.Habilidades.Add(HabilidadesDeClasse[0]);
                    break;
                case 3:
                    personagem.Habilidades.Add(HabilidadesDeClasse[1]);
                    break;
                case 6:
                    personagem.Habilidades.Add(HabilidadesDeClasse[3]);
                    break;
                default:
                    break;
            }
        }
    }
}