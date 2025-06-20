using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Entities;
using Assets.Scripts.World;
using System;
using System.Linq;
using Assets.Scripts.Entities.Missoes;
using Assets.Scripts.Entities.Itens;

namespace Assets.Scripts.Combat
{
    public enum EstadoCombate { TurnoJogador, TurnoInimigo, Vitoria, Derrota, Fuga }

    public class SistemaCombate
    {
        public Personagem Jogador;
        public Inimigo InimigoAtual;
        public EstadoCombate Estado;

        public event Action<EstadoCombate> OnStateChanged;

        public SistemaCombate(Personagem jogador, Inimigo inimigo)
        {
            Jogador = jogador;
            InimigoAtual = inimigo;

            if (DiceSystem.RollInitiative(jogador.Destreza, InimigoAtual.Defesa - 10))
            {
                Estado = EstadoCombate.TurnoJogador;
            }
            else
            {
                Estado = EstadoCombate.TurnoInimigo;
            }

            OnStateChanged?.Invoke(Estado);
        }

        public string JogadorAtacar()
        {
            if (DiceSystem.RollAttack(Jogador.Ataque, InimigoAtual.Defesa))
            {
                int dano = DiceSystem.RollDamage(Jogador.Ataque, Jogador.Inventario.ArmaEquipada.DiceType);
                InimigoAtual.VidaAtual -= dano;
                if (InimigoAtual.VidaAtual <= 0)
                {
                    Estado = EstadoCombate.Vitoria;
                    OnStateChanged?.Invoke(Estado);
                    return $"{Jogador.Nome} atacou {InimigoAtual.Nome} causando {dano} de dano!\n{Jogador.Nome} derrotou {InimigoAtual.Nome}!";
                }
                else
                {
                    Estado = EstadoCombate.TurnoInimigo;
                    OnStateChanged?.Invoke(Estado);
                    return $"{Jogador.Nome} atacou {InimigoAtual.Nome} causando {dano} de dano!";
                }
            }
            else
            {
                Estado = EstadoCombate.TurnoInimigo;
                OnStateChanged?.Invoke(Estado);
                return $"{Jogador.Nome} errou o ataque!";
            }
        }

        public string JogadorHabilidadeCura(Habilidade habilidade)
        {
            if (Jogador.ManaAtual < habilidade.CustoMana)
            {
                return "Mana Insuficiente!";
            }

            habilidade.Use(Jogador, InimigoAtual);

            Estado = EstadoCombate.TurnoInimigo;
            OnStateChanged?.Invoke(Estado);
            return $"{Jogador.Nome} usou {habilidade.Nome} e curou {habilidade.Efeito} pontos de vida!";
        }

        public string JogadorHabilidadeDano(Habilidade habilidade)
        {
            if (Jogador.ManaAtual < habilidade.CustoMana)
            {
                return "Mana Insuficiente!";
            }

            habilidade.Use(Jogador, InimigoAtual);

            if (InimigoAtual.VidaAtual <= 0)
            {
                Estado = EstadoCombate.Vitoria;
                OnStateChanged?.Invoke(Estado);
                return $"{Jogador.Nome} usou {habilidade.Nome} causando {habilidade.Efeito} de dano!\n{Jogador.Nome} venceu!";
            }
            else
            {
                Estado = EstadoCombate.TurnoInimigo;
                OnStateChanged?.Invoke(Estado);
                return $"{Jogador.Nome} usou {habilidade.Nome} causando {habilidade.Efeito} de dano!";
            }
        }

        public string JogadorUsarPocao(Pocao pocao)
        {
            if (!Jogador.Inventario.Itens.Contains(pocao)) return "";

            pocao.Use(Jogador);

            Estado = EstadoCombate.TurnoInimigo;
            OnStateChanged?.Invoke(Estado);
            return $"{Jogador.Nome} usou {pocao.Nome} e curou {pocao.HealingAmount} pontos de vida!";
        }

        public string InimigoAtacar()
        {
            if (DiceSystem.RollAttack(InimigoAtual.Ataque, Jogador.Defesa))
            {
                int dano = DiceSystem.RollDamage(InimigoAtual.Ataque);
                Jogador.VidaAtual -= dano;
                if (Jogador.VidaAtual <= 0)
                {
                    Estado = EstadoCombate.Derrota;
                    OnStateChanged?.Invoke(Estado);
                    return $"{InimigoAtual.Nome} atacou {Jogador.Nome} causando {dano} de dano!\n{InimigoAtual.Nome} venceu!";
                }
                else
                {
                    Estado = EstadoCombate.TurnoJogador;
                    OnStateChanged?.Invoke(Estado);
                    return $"{InimigoAtual.Nome} atacou {Jogador.Nome} causando {dano} de dano!";
                }
            }
            else
            {
                Estado = EstadoCombate.TurnoJogador;
                OnStateChanged?.Invoke(Estado);
                return $"{InimigoAtual.Nome} errou o ataque!";
            }
        }

        public string Run()
        {
            if (DiceSystem.RollInitiative(Jogador.Destreza, InimigoAtual.Defesa - 10))
            {
                Estado = EstadoCombate.Fuga;
                OnStateChanged?.Invoke(Estado);
                return $"{Jogador.Nome} conseguiu fugir do combate!";
            }
            else
            {
                Estado = EstadoCombate.TurnoInimigo;
                OnStateChanged?.Invoke(Estado);
                return $"{Jogador.Nome} não conseguiu fugir e o combate continua!";
            }
        }
    }
}