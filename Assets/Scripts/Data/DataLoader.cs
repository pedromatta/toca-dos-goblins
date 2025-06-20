using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Habilidades;
using Assets.Scripts.Entities.Itens;
using Assets.Scripts.Entities.Missoes;
using Assets.Scripts.World;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// DTOs for deserialization
[Serializable]
public struct JsonItem
{
    public string id;
    public string nome;
    public string descricao;
    public int peso;
    public string tipo;
    public int diceType;
    public int healingAmount;
}

[Serializable]
public struct JsonClasse
{
    public string id;
    public string nome;
    public string tipoAtaque;
    public string armaInicialId;
    public string[] habilidadesClasseId;
}

[Serializable]
public struct JsonRaca
{
    public string id;
    public string nome;
    public int baseForca, baseDestreza, baseInteligencia, baseConstituicao;
    public string habilidadeBaseId;
}

[Serializable]
public struct JsonHabilidade
{
    public string id;
    public string nome;
    public string tipo;
    public string descricao;
    public int custoMana;
    public int efeito;
}

[Serializable]
public struct JsonMissao
{
    public string id;
    public string nome;
    public string descricao;
    public string tipo;
    public string objetivo;
    public int quantidadeNecessaria;
    public int recompensaXP;
    public string recompensaItemId;
    public string inimigoParaExterminioId;
    public string itemParaColetaId;
}

[Serializable]
public struct JsonNPC
{
    public string id;
    public string nome;
    public string[] dialogos;
    public string missaoId;
}

[Serializable]
public struct JsonInimigo
{
    public string id;
    public string nome;
    public int vidaMaxima;
    public int ataque;
    public int dadoAtaque;
    public int defesa;
    public int xp;
    public bool permanentDeath;
    public string[] recompensaItemIds;
}

[Serializable]
public struct JsonArea
{
    public string id;
    public string nome;
    public string descricao;
    public string[] npcIds;
    public string[] inimigoIds;
}

[Serializable]
public struct JsonMapa
{
    public string nome;
    public JsonArea[] areas;
}

[Serializable]
public struct JsonPersonagem
{
    public string nome;
    public string classe;
    public string raca;
    public int forca, destreza, inteligencia, constituicao;
    public int vidaMaxima, vidaAtual;
    public int manaMaxima, manaAtual;
    public int ataque, defesa;
    public int nivel, experiencia, experienciaParaProximoNivel;
    public int capacidadeCarga;
    public int pontosAtributosDisponiveis;
    public List<string> habilidades;
    public List<string> inventario;
    public List<string> missoes;
    public string armaEquipada;
}

// Wrappers for Unity's JsonUtility
[Serializable] public struct JsonItemList { public List<JsonItem> items; }
[Serializable] public struct JsonClasseList { public List<JsonClasse> classes; }
[Serializable] public struct JsonRacaList { public List<JsonRaca> racas; }
[Serializable] public struct JsonHabilidadeList { public List<JsonHabilidade> habilidades; }
[Serializable] public struct JsonMissaoList { public List<JsonMissao> missoes; }
[Serializable] public struct JsonNPCList { public List<JsonNPC> npcs; }
[Serializable] public struct JsonInimigoList { public List<JsonInimigo> inimigos; }
[Serializable] public struct JsonAreaList { public List<JsonArea> areas; }

public static class DataLoader
{
    public static Dictionary<string, Item> LoadItems()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("items");
        if (jsonAsset == null)
        {
            Debug.LogError("Items JSON not found in Resources!");
            return new Dictionary<string, Item>();
        }
        var list = JsonUtility.FromJson<JsonItemList>(jsonAsset.text);
        var dict = new Dictionary<string, Item>();
        foreach (var j in list.items)
        {
            Item item;
            if (j.tipo == "Arma")
                item = new Arma(j.nome, j.descricao, j.peso, j.diceType);
            else if (j.tipo == "Pocao")
                item = new Pocao(j.nome, j.descricao, j.peso, j.healingAmount);
            else if (j.tipo == "Coletavel")
                item = new Coletavel(j.nome, j.descricao, j.peso);
            else
                continue;

            dict[j.id] = item;
        }
        return dict;
    }

    public static Dictionary<string, Habilidade> LoadHabilidades()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("habilidades");
        if (jsonAsset == null)
        {
            Debug.LogError("Habilidades JSON not found in Resources!");
            return new Dictionary<string, Habilidade>();
        }
        var list = JsonUtility.FromJson<JsonHabilidadeList>(jsonAsset.text);
        var dict = new Dictionary<string, Habilidade>();
        foreach (var j in list.habilidades)
        {
            switch(j.tipo)
            {
                case "dano":
                    dict[j.id] = new HabilidadeDano(j.nome, j.descricao, j.custoMana, j.efeito);
                    break;
                case "cura":
                    dict[j.id] = new HabilidadeCura(j.nome, j.descricao, j.custoMana, j.efeito);
                    break;
            }
        }
        return dict;
    }

    public static Dictionary<string, Classe> LoadClasses(Dictionary<string, Item> items, Dictionary<string, Habilidade> habilidades)
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("classes");
        if (jsonAsset == null)
        {
            Debug.LogError("Classes JSON not found in Resources!");
            return new Dictionary<string, Classe>();
        }
        var list = JsonUtility.FromJson<JsonClasseList>(jsonAsset.text);
        var dict = new Dictionary<string, Classe>();
        foreach (var j in list.classes)
        {
            Arma arma = (j.armaInicialId != null && items.ContainsKey(j.armaInicialId)) ? items[j.armaInicialId] as Arma : null;
            List<Habilidade> habilidadesClasse = new List<Habilidade>();

            foreach(var h in j.habilidadesClasseId)
            {
                habilidadesClasse.Add(habilidades.ContainsKey(h) ? habilidades[h] : null);
            }
            var classe = new Classe(j.nome, j.tipoAtaque, arma, habilidadesClasse);
            dict[j.id] = classe;
        }
        return dict;
    }

    public static Dictionary<string, Raca> LoadRacas(Dictionary<string, Habilidade> habilidades)
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("racas");
        if (jsonAsset == null)
        {
            Debug.LogError("Racas JSON not found in Resources!");
            return new Dictionary<string, Raca>();
        }
        var list = JsonUtility.FromJson<JsonRacaList>(jsonAsset.text);
        var dict = new Dictionary<string, Raca>();
        foreach (var j in list.racas)
        {
            Habilidade habilidade = (j.habilidadeBaseId != null && habilidades.ContainsKey(j.habilidadeBaseId)) ? habilidades[j.habilidadeBaseId] : null;
            var raca = new Raca(j.nome, j.baseForca, j.baseDestreza, j.baseInteligencia, j.baseConstituicao, habilidade);
            dict[j.id] = raca;
        }
        return dict;
    }

    public static Dictionary<string, Missao> LoadMissoes(Dictionary<string, Item> items, Dictionary<string, Inimigo> inimigos)
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("missoes");
        if (jsonAsset == null)
        {
            Debug.LogError("Missoes JSON not found in Resources!");
            return new Dictionary<string, Missao>();
        }
        var list = JsonUtility.FromJson<JsonMissaoList>(jsonAsset.text);
        var dict = new Dictionary<string, Missao>();
        foreach (var j in list.missoes)
        {
            Item recompensa = (j.recompensaItemId != null && items.ContainsKey(j.recompensaItemId)) ? items[j.recompensaItemId] : null;
            Inimigo inimigoParaColeta = (j.inimigoParaExterminioId != null && inimigos.ContainsKey(j.inimigoParaExterminioId)) ? inimigos[j.inimigoParaExterminioId] : null;
            Coletavel itemParaColeta = (j.itemParaColetaId != null && items.ContainsKey(j.itemParaColetaId)) ? items[j.itemParaColetaId] as Coletavel : null;
            switch (j.tipo)
            {
                case "coleta":
                    dict[j.id] = new MissaoColeta(j.nome, j.descricao, j.objetivo, j.quantidadeNecessaria, j.recompensaXP, recompensa);
                    (dict[j.id] as MissaoColeta).ItemParaColeta = itemParaColeta;
                    break;
                case "exterminio":
                    dict[j.id] = new MissaoExterminio(j.nome, j.descricao, j.objetivo, j.quantidadeNecessaria, j.recompensaXP, recompensa);
                    (dict[j.id] as MissaoExterminio).InimigoParaExterminio = inimigoParaColeta;
                    break;
            }
        }
        return dict;
    }

    public static Dictionary<string, NPC> LoadNPCs(Dictionary<string, Missao> missoes)
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("npcs");
        if (jsonAsset == null)
        {
            Debug.LogError("NPCs JSON not found in Resources!");
            return new Dictionary<string, NPC>();
        }
        var list = JsonUtility.FromJson<JsonNPCList>(jsonAsset.text);
        var dict = new Dictionary<string, NPC>();
        foreach (var j in list.npcs)
        {
            Missao missao = (j.missaoId != null && missoes.ContainsKey(j.missaoId)) ? missoes[j.missaoId] : null;

            var dialogos = new List<string>();

            dialogos.AddRange(j.dialogos);

            var npc = new NPC(j.nome, dialogos, missao);
            dict[j.id] = npc;
        }
        return dict;
    }

    public static Dictionary<string, Inimigo> LoadInimigos(Dictionary<string, Item> items)
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("inimigos");
        if (jsonAsset == null)
        {
            Debug.LogError("Inimigos JSON not found in Resources!");
            return new Dictionary<string, Inimigo>();
        }
        var list = JsonUtility.FromJson<JsonInimigoList>(jsonAsset.text); 
        var dict = new Dictionary<string, Inimigo>();
        foreach (var j in list.inimigos)
        {
            var recompensas = new List<Item>();
            if (j.recompensaItemIds != null)
            {
                foreach (var id in j.recompensaItemIds)
                {
                    if (items.ContainsKey(id))
                        recompensas.Add(items[id]);
                }
            }
            var inimigo = new Inimigo(j.nome, j.vidaMaxima, j.ataque, j.dadoAtaque, j.defesa, j.xp, recompensas);
            inimigo.PermanentDeath = j.permanentDeath;
            dict[j.id] = inimigo;
        }
        return dict;
    }

    public static Dictionary<string, Area> LoadAreas(Dictionary<string, NPC> npcs, Dictionary<string, Inimigo> inimigos)
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("areas");
        if (jsonAsset == null)
        {
            Debug.LogError("Areas JSON not found in Resources!");
            return new Dictionary<string, Area>();
        }
        var list = JsonUtility.FromJson<JsonAreaList>(jsonAsset.text);
        var dict = new Dictionary<string, Area>();
        foreach (var j in list.areas)
        {
            var area = new Area(j.nome, j.descricao);
            if (j.npcIds != null)
            {
                foreach (var id in j.npcIds)
                {
                    if (npcs.ContainsKey(id))
                        area.NPCs.Add(npcs[id]);
                }
            }
            if (j.inimigoIds != null)
            {
                foreach (var id in j.inimigoIds)
                {
                    if (inimigos.ContainsKey(id))
                        area.Inimigos.Add(inimigos[id]);
                }
            }
            dict[j.id] = area;
        }
        return dict;
    }

    public static Mapa LoadMapa(Dictionary<string, Area> areas)
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("world");
        if (jsonAsset == null)
        {
            Debug.LogError("World JSON not found in Resources!");
            return new Mapa("Mundo Vazio");
        }
        var jsonMapa = JsonUtility.FromJson<JsonMapa>(jsonAsset.text); 
        var mapa = new Mapa(jsonMapa.nome);
        foreach (var jArea in jsonMapa.areas)
        {
            if (areas.ContainsKey(jArea.id))
                mapa.Areas.Add(areas[jArea.id]);
        }
        return mapa;
    }

    public static void SavePersonagem(Personagem personagem)
    {
        var dto = ToJsonPersonagem(personagem);

        string json = JsonUtility.ToJson(dto, true);
        string path = Path.Combine(Application.streamingAssetsPath, "personagem.json");
        File.WriteAllText(path, json);
    }

    public static JsonPersonagem ToJsonPersonagem(Personagem personagem)
    {
        return new JsonPersonagem
        {
            nome = personagem.Nome,
            classe = personagem.Classe.Nome,
            raca = personagem.Raca.Nome,
            forca = personagem.Forca,
            destreza = personagem.Destreza,
            inteligencia = personagem.Inteligencia,
            constituicao = personagem.Constituicao,
            vidaMaxima = personagem.VidaMaxima,
            vidaAtual = personagem.VidaAtual,
            manaMaxima = personagem.ManaMaxima,
            manaAtual = personagem.ManaAtual,
            ataque = personagem.Ataque,
            defesa = personagem.Defesa,
            nivel = personagem.Nivel,
            experiencia = personagem.Experiencia,
            experienciaParaProximoNivel = personagem.ExperienciaParaProximoNivel,
            capacidadeCarga = personagem.CapacidadeCarga,
            pontosAtributosDisponiveis = personagem.PontosAtributosDisponiveis,
            habilidades = personagem.Habilidades?.Select(h => h.Nome).ToList() ?? new List<string>(),
            inventario = personagem.Inventario?.Itens?.Select(i => i.Nome).ToList() ?? new List<string>(),
            missoes = personagem.Missoes?.Select(m => m.Nome).ToList() ?? new List<string>(),
            armaEquipada = personagem.Inventario?.ArmaEquipada?.Nome
        };
    }

    public static Personagem LoadPersonagem(
        Dictionary<string, Classe> classes,
        Dictionary<string, Raca> racas,
        Dictionary<string, Habilidade> habilidades,
        Dictionary<string, Item> itens,
        Dictionary<string, Missao> missoes)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "personagem.json");
        if (!File.Exists(path))
        {
            Debug.LogError("Personagem JSON not found: " + path);
            return null;
        }
        string json = File.ReadAllText(path);
        var dto = JsonUtility.FromJson<JsonPersonagem>(json);

        return FromJsonPersonagem(dto, classes, racas, habilidades, itens, missoes);
    }

    public static Personagem FromJsonPersonagem(
        JsonPersonagem dto,
        Dictionary<string, Classe> classes,
        Dictionary<string, Raca> racas,
        Dictionary<string, Habilidade> habilidades,
        Dictionary<string, Item> itens,
        Dictionary<string, Missao> missoes)
    {
        var classe = classes.Values.FirstOrDefault(c => c.Nome == dto.classe);
        var raca = racas.Values.FirstOrDefault(r => r.Nome == dto.raca);

        if (classe == null || raca == null)
        {
            Debug.LogError("Classe or Raca not found for loaded personagem.");
            return null;
        }

        var personagem = new Personagem(dto.nome, classe, raca)
        {
            Forca = dto.forca,
            Destreza = dto.destreza,
            Inteligencia = dto.inteligencia,
            Constituicao = dto.constituicao,
            VidaMaxima = dto.vidaMaxima,
            VidaAtual = dto.vidaAtual,
            ManaMaxima = dto.manaMaxima,
            ManaAtual = dto.manaAtual,
            Ataque = dto.ataque,
            Defesa = dto.defesa,
            Nivel = dto.nivel,
            Experiencia = dto.experiencia,
            ExperienciaParaProximoNivel = dto.experienciaParaProximoNivel,
            CapacidadeCarga = dto.capacidadeCarga,
            PontosAtributosDisponiveis = dto.pontosAtributosDisponiveis,
            Habilidades = dto.habilidades
                .Where(hn => habilidades.Values.Any(h => h.Nome == hn))
                .Select(hn => habilidades.Values.First(h => h.Nome == hn))
                .ToList(),
            Inventario = new Inventario(),
            Missoes = dto.missoes?
                .Where(mn => missoes.Values.Any(m => m.Nome == mn))
                .Select(mn => missoes.Values.First(m => m.Nome == mn))
                .ToList() ?? new List<Missao>()
        };


        personagem.AtualizarAtributosDerivados();

        if (dto.inventario != null)
        {
            foreach (var itemNome in dto.inventario)
            {
                var item = CloneItem(itens.Values.FirstOrDefault(i => i.Nome == itemNome));
                if (item != null)
                    personagem.Inventario.AdicionarItem(item);
                else
                {
                    Debug.Log($"Itme not found in dictionary: {item.Nome}");
                }
            }
        }

        if (!string.IsNullOrEmpty(dto.armaEquipada))
        {
            var arma = CloneItem(itens.Values.FirstOrDefault(i => i.Nome == dto.armaEquipada)) as Arma;
            if (arma != null)
                personagem.Inventario.ArmaEquipada = arma;
        }

        return personagem;
    }

    public static Item CloneItem(Item item)
    {
        if (item is Arma arma)
            return new Arma(arma.Nome, arma.Descricao, arma.Peso, arma.DiceType);
        if (item is Pocao pocao)
            return new Pocao(pocao.Nome, pocao.Descricao, pocao.Peso, pocao.HealingAmount);
        if (item is Coletavel coletavel)
            return new Coletavel(coletavel.Nome, coletavel.Descricao, coletavel.Peso);
        return null;
    }
}