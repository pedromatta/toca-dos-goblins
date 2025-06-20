using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Entities.Itens
{
    public class Inventario
    {
        public int CapacidadeMaxima;
        public List<Item> Itens;
        public Arma ArmaEquipada;

        public Inventario()
        {
            Itens = new List<Item>();
        }

        public int PesoAtual()
        {
            int peso = 0;
            foreach (var item in Itens)
                peso += item.Peso;
            return peso;
        }

        public bool AdicionarItem(Item item)
        {
            if (PesoAtual() + item.Peso > CapacidadeMaxima)
                return false;
            Itens.Add(item);
            return true;
        }

        public void RemoverItem(Item item)
        {
            var itemToRemove = Itens.FirstOrDefault(i => i.Nome == item.Nome && i.GetType() == item.GetType());

            if (itemToRemove != null)
                Itens.Remove(itemToRemove);
        }
    }
}