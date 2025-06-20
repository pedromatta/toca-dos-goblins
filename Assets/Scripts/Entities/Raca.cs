namespace Assets.Scripts.Entities
{
    [System.Serializable]
    public class Raca
    {
        public string Nome;
        public int BaseForca, BaseDestreza, BaseInteligencia, BaseConstituicao;
        public Habilidade HabilidadeBase;

        public Raca(string nome, int forca, int destreza, int inteligencia, int constituicao, Habilidade habilidadeBase)
        {
            Nome = nome;
            BaseForca = forca;
            BaseDestreza = destreza;
            BaseInteligencia = inteligencia;
            BaseConstituicao = constituicao;
            HabilidadeBase = habilidadeBase;
        }
    }
}