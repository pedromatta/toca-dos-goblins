using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Entities;
using Assets.Scripts.World;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class GameSaveData
    {
        public JsonPersonagem personagem;
        public string currentAreaName;
        public List<AreaSaveData> areas;
    }

    [Serializable]
    public class AreaSaveData
    {
        public string nome;
        public List<InimigoSaveData> inimigos;
        public List<NPCSaveData> npcs;
    }

    [Serializable]
    public class InimigoSaveData
    {
        public string id;
        public string nome;
        public int vida;
        public bool morto;
    }

    [Serializable]
    public class NPCSaveData
    {
        public string nome;
        public int missaoProgresso;
        public bool missaoConcluida;
    }
}