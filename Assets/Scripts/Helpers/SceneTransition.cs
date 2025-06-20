using Assets.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class SceneTransition
    {
        public static Vector3 PlayerWorldPosition;
        public static Inimigo EnemyToFight;

        // New: Store last player position per area
        public static Dictionary<string, Vector3> AreaPlayerPositions = new Dictionary<string, Vector3>();
    }
}