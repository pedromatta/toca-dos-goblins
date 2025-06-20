using System.Collections.Generic;

namespace Assets.Scripts.World
{
    public class Mapa
    {
        public string Nome;
        public List<Area> Areas;

        public Mapa(string nome)
        {
            Nome = nome;
            Areas = new List<Area>();
        }
    }
}