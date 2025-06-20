using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Entities.Habilidades
{
    public class HabilidadeDano : Habilidade
    {
        public HabilidadeDano(string nome, string descricao, int custoMana, int efeito) : base(nome, descricao, custoMana, efeito) { }

        public override void Use(Personagem personagem, Inimigo inimigo)
        {
            if (personagem.ManaAtual < CustoMana) return;

            inimigo.VidaAtual -= Efeito;
            personagem.ManaAtual -= CustoMana;
        }
    }
}