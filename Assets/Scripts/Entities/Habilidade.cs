namespace Assets.Scripts.Entities
{
    public abstract class Habilidade
    {
        public string Nome;
        public string Descricao;
        public int CustoMana;
        public int Efeito;

        public Habilidade(string nome, string descricao, int custoMana, int efeito)
        {
            Nome = nome;
            Descricao = descricao;
            CustoMana = custoMana;
            Efeito = efeito;
        }

        public abstract void Use(Personagem personagem, Inimigo inimigo);
    }
}