using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Entities.Itens
{
    public class Arma : Item, IUsableItem
    {
        public int DiceType { get; private set; }
        public Arma(string nome, string descricao, int peso, int diceType)
            : base(nome, descricao, peso)
        {
            DiceType = diceType;
        }
        public void Use(Personagem target)
        {
            target.Inventario.RemoverItem(this);
            target.Inventario.AdicionarItem(target.Inventario.ArmaEquipada);

            target.Inventario.ArmaEquipada = this;
            target.AtualizarAtributosDerivados();
        }
    }
}