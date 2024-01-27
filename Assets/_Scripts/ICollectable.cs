namespace GGJ24
{
    public interface ICollecetable
    {
        public string InteractionPrompt { get; }

        public void Collect(Collector collector);
    }
}