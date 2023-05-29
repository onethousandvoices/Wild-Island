namespace WildIsland.Processors
{
    public abstract class BaseProcessor
    {
        protected bool Enabled { get; private set; }

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

    public interface IPlayerProcessor : IBaseProcessor { }
}