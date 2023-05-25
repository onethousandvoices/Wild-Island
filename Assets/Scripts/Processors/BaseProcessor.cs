namespace WildIsland.Processors
{
    public abstract class BaseProcessor : IBaseProcessor
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
    
    public interface IPlayerProcessor : IBaseProcessor
    {
        public void Tick();
    }

    public interface IFixedPlayerProcessor : IBaseProcessor
    {
        public void FixedTick();
    }

    public interface ILatePlayerProcessor : IBaseProcessor
    {
        public void LateTick();
    }
}