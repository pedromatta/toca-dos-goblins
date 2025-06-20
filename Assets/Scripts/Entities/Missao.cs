using Assets.Scripts.Entities.Itens;

namespace Assets.Scripts.Entities
{
    public class Missao
    {
        public string Nome, Descricao;
        public string Objetivo;
        public int Progresso, QuantidadeNecessaria;
        public bool Concluida;
        public int RecompensaXP;
        public Item RecompensaItem;

        public Missao(string nome, string descricao, string objetivo, int quantidade, int recompensaXP, Item recompensaItem)
        {
            Nome = nome;
            Descricao = descricao;
            Objetivo = objetivo;
            QuantidadeNecessaria = quantidade;
            Progresso = 0;
            Concluida = false;
            RecompensaXP = recompensaXP;
            RecompensaItem = recompensaItem;
        }

        protected void AtualizarProgresso(int quantidade)
        {
            Progresso += quantidade;
        }

        public void Completar(Personagem personagem)
        {
            if (Progresso >= QuantidadeNecessaria && personagem.Inventario.AdicionarItem(RecompensaItem))
            {
                personagem.GanharExperiencia(RecompensaXP);
                Concluida = true;
            }
        }
    }
}