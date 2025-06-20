namespace Assets.Scripts.Entities
{
    public abstract class Item
    {
        public string Nome, Descricao;
        public int Peso;

        public Item(string nome, string descricao, int peso)
        {
            Nome = nome;
            Descricao = descricao;
            Peso = peso;
        }
    }
}