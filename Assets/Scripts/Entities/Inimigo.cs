using System.Collections.Generic;

namespace Assets.Scripts.Entities
{
    public class Inimigo
    {
        public string Nome;
        public int VidaMaxima, VidaAtual, Ataque, DadoAtaque, Defesa, XP;
        public List<Item> Recompensas;
        public bool PermanentDeath;

        public Inimigo(string nome, int vida, int ataque, int dadoAtaque, int defesa, int xp, List<Item> recompensas)
        {
            Nome = nome;
            VidaMaxima = vida;
            VidaAtual = vida;
            Ataque = ataque;
            DadoAtaque = dadoAtaque;
            Defesa = defesa;
            XP = xp;
            Recompensas = recompensas;
        }
    }
}