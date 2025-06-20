using Assets.Scripts.Entities;
using System.Collections.Generic;

namespace Assets.Scripts.World
{
    public class NPC
    {
        public string Nome;
        public List<string> Dialogos;
        public Missao MissaoDisponivel;

        public NPC(string nome, List<string> dialogos, Missao missao = null)
        {
            Nome = nome;
            Dialogos = dialogos;
            MissaoDisponivel = missao;
        }
    }
}