using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Entities.Itens
{
    public interface IUsableItem
    {
        public void Use(Personagem target) { }
    }
}