using Zenject;

namespace WildIsland.Processors
{
    public abstract class PlayerProcessor : BaseProcessor, IInitializable
    {
        public virtual void Initialize() { }
    }

    public interface IPlayerProcessor : IBaseProcessor
    {
        public void Tick();
    }
}