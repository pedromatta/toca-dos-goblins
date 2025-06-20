using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Entities.Habilidades
{
    public class HabilidadeCura : Habilidade
    {
        public HabilidadeCura(string nome, string descricao, int custoMana, int efeito) : base(nome, descricao, custoMana, efeito) { }

        public override void Use(Personagem personagem, Inimigo inimigo)
        {
            if (personagem.ManaAtual < CustoMana) return;

            personagem.VidaAtual += Efeito;
            personagem.ManaAtual -= CustoMana;

            if (personagem.VidaAtual > personagem.VidaMaxima)
                personagem.VidaAtual = personagem.VidaMaxima;
        }
    }
}