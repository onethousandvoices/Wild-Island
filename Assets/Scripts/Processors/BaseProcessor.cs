namespace WildIsland.Processors
{
    public class BaseProcessor : IBaseProcessor
    {
        public bool Enabled { get; private set; }

        public void Enable()
            => Enabled = true;

        public void Disable()
            => Enabled = false;
    }

    public interface IBaseProcessor
    {
        public void Enable();
        public void Disable();
    }
}