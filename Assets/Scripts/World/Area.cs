using Assets.Scripts.Entities;
using System.Collections.Generic;

namespace Assets.Scripts.World
{
    public class Area
    {
        public string Nome;
        public string Descricao;
        public List<NPC> NPCs;
        public List<Inimigo> Inimigos;

        public Area(string nome, string descricao)
        {
            Nome = nome;
            Descricao = descricao;
            NPCs = new List<NPC>();
            Inimigos = new List<Inimigo>();
        }
    }
}