using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    public static class DiceSystem
    {
        private const int mainDice = 20;

        public static bool RollInitiative(int playerDex, int enemyDex)
        {
            int playerRoll = Random.Range(1, mainDice + 1) + playerDex;
            int enemyRoll = Random.Range(1, mainDice + 1) + enemyDex;

            if(playerRoll > enemyRoll)
            {
                return true;
            }
            else if (playerRoll < enemyRoll)
            {
                return false;
            }
            else
            {
                return playerDex >= enemyDex;
            }
        }

        public static bool RollAttack(int attackerAttack, int defenderDefense)
        {
            int attackRoll = Random.Range(1, mainDice + 1) + attackerAttack;
            return attackRoll >= defenderDefense;
        }

        public static int RollDamage(int damageBonus, int dice = 6)
        {
            return Random.Range(1, dice + 1) + damageBonus;
        }
    }
}