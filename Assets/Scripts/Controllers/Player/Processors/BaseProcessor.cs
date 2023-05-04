namespace WildIsland.Controllers
{
    public class BaseProcessor
    {
        public bool Enabled { get; private set; }

        public void Enable()
            => Enabled = true;

        public void Disable()
            => Enabled = false;
    }

    public interface ITickableProcessor
    {
        public void Tick();
    }
}