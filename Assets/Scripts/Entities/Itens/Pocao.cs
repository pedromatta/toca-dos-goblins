using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Entities.Itens
{
    public class Pocao : Item, IUsableItem
    {
        public int HealingAmount;

        public Pocao(string nome, string descricao, int peso, int healingAmount)
            : base(nome, descricao, peso)
        {
            HealingAmount = healingAmount;
        }

        public void Use(Personagem target)
        {
            target.VidaAtual = Mathf.Min(target.VidaAtual + HealingAmount, target.VidaMaxima);
            target.Inventario.RemoverItem(this);
        }
    }
}