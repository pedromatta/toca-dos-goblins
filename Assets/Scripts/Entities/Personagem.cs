using Assets.Scripts.Entities.Itens;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class Personagem
    {
        public string Nome;
        public Classe Classe;
        public Raca Raca;
        public int Forca, Destreza, Inteligencia, Constituicao;
        public int VidaMaxima, VidaAtual;
        public int ManaMaxima, ManaAtual;
        public int Ataque, Defesa;
        public int Nivel, Experiencia, ExperienciaParaProximoNivel;
        public int CapacidadeCarga;
        public int PontosAtributosDisponiveis;
        public List<Missao> Missoes;
        public List<Habilidade> Habilidades;
        public Inventario Inventario;

        public Personagem(string nome, Classe classe, Raca raca)
        {
            Nome = nome;
            Classe = classe;
            Raca = raca;
            Nivel = 1;
            Experiencia = 0;
            ExperienciaParaProximoNivel = 100;
            PontosAtributosDisponiveis = 8;
            Missoes = new List<Missao>();
            Habilidades = new List<Habilidade>();
            Inventario = new Inventario();
            InicializarAtributos();
            AtualizarAtributosDerivados();
        }

        private void InicializarAtributos()
        {
            Forca = 10 + Raca.BaseForca;
            Destreza = 10 + Raca.BaseDestreza;
            Inteligencia = 10 + Raca.BaseInteligencia;
            Constituicao = 10 + Raca.BaseConstituicao;
            Inventario.ArmaEquipada = Classe.ArmaInicial;
            Habilidades.Add(Raca.HabilidadeBase);
            Classe.GetHabilidadesDeClasse(this);
        }

        public void AtualizarAtributosDerivados()
        {
            VidaMaxima = (10 + GetModifier(Constituicao) * Nivel);
            ManaMaxima = (5 + GetModifier(Inteligencia) * Nivel);
            Ataque = GetAttackMod();
            Defesa = 10 + GetModifier(Destreza) + Nivel;
            CapacidadeCarga = (10 + GetModifier(Forca));
            VidaAtual = VidaMaxima;
            ManaAtual = ManaMaxima;
            Inventario.CapacidadeMaxima = CapacidadeCarga;
        }

        public void GanharExperiencia(int xp)
        {
            Experiencia += xp;
            while (Experiencia >= ExperienciaParaProximoNivel)
            {
                SubirNivel();
            }
        }

        public void SubirNivel()
        {
            Nivel++;
            Experiencia -= ExperienciaParaProximoNivel;
            ExperienciaParaProximoNivel = (Nivel * Nivel) * 100;
            PontosAtributosDisponiveis += 1;

            AtualizarAtributosDerivados();

            this.Classe.GetHabilidadesDeClasse(this);
        }

        public void DistribuirPontos(string atributo)
        {
            if (PontosAtributosDisponiveis <= 0) return;
            switch (atributo.ToLower())
            {
                case "forca": Forca++; break;
                case "destreza": Destreza++; break;
                case "inteligencia": Inteligencia++; break;
                case "constituicao": Constituicao++; break;
            }
            PontosAtributosDisponiveis--;
            AtualizarAtributosDerivados();
        }

        public int GetModifier(int attributeValue)
        {
            return Mathf.FloorToInt((attributeValue - 10) / 2f);
        }

        public int GetAttackMod()
        {
            return Classe.TipoAtaque.ToLower() switch
            {
                "forca" => GetModifier(Forca),
                "destreza" => GetModifier(Destreza),
                "inteligencia" => GetModifier(Inteligencia),
                _ => 0,
            };
        }
    }
}