namespace DevMeter.Core.Models
{
    public class Contributor(string name, int contributions)
    {
        public string Name => name;
        public int Contributions => contributions;
    }
}
