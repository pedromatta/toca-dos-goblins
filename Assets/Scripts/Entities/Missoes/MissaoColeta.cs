using Assets.Scripts.Entities.Itens;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Entities.Missoes
{
    public class MissaoColeta : Missao
    {
        public Coletavel ItemParaColeta { get; set; }
        public MissaoColeta(string nome, string descricao, string objetivo, int quantidade, int recompensaXP, Item recompensaItem) : base(nome, descricao, objetivo, quantidade, recompensaXP, recompensaItem)
        {
        }

        public void Coletou(Item item)
        {
            if(item.Nome.Equals(ItemParaColeta.Nome))
                AtualizarProgresso(1);
        }

        public void Removeu(Item item)
        {
            if(item.Nome.Equals(ItemParaColeta.Nome))
                AtualizarProgresso(-1);
        }
    }
}