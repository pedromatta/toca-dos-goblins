using Assets.Scripts.Entities.Itens;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Entities.Missoes
{
    public class MissaoExterminio : Missao
    {
        public Inimigo InimigoParaExterminio { get; set; }
        public MissaoExterminio(string nome, string descricao, string objetivo, int quantidade, int recompensaXP, Item recompensaItem) : base(nome, descricao, objetivo, quantidade, recompensaXP, recompensaItem)
        {
        }

        public void Derrotou(Inimigo inimigo)
        {
            if (inimigo.Nome.Equals(InimigoParaExterminio.Nome)) 
                AtualizarProgresso(1);
        }
    }
}