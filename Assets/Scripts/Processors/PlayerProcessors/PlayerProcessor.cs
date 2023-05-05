using Zenject;

namespace WildIsland.Processors
{
    public class PlayerProcessor : BaseProcessor, IPlayerProcessor
    {
        public virtual void Initialize() { }
        public virtual void Tick() { }
    }

    public interface IPlayerProcessor : IInitializable
    {
        public void Tick();
    }
}