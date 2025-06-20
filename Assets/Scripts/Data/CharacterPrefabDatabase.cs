using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "CharacterPrefabDatabase", menuName = "Game/CharacterPrefabDatabase")]
    public class CharacterPrefabDatabase : ScriptableObject
    {
        [System.Serializable]
        public struct CharacterPrefabEntry
        {
            public string Race;
            public string Classe;
            public GameObject Prefab;
        }

        public List<CharacterPrefabEntry> PrefabEntries;

        public GameObject GetPrefab(string race, string classe)
        {
            foreach (var entry in PrefabEntries)
            {
                if (entry.Race == race && entry.Classe == classe)
                    return entry.Prefab;
            }
            return null;
        }
    }

}