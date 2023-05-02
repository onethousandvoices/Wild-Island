using WildIsland.Data;

namespace Effects
{
    public abstract class BaseEffect
    {
        public readonly PlayerDataEffect[] Effects;

        public abstract bool IsApplying();
        
        protected BaseEffect(params PlayerDataEffect[] effects)
        {
            Effects = new PlayerDataEffect[effects.Length];

            for (int i = 0; i < effects.Length; i++)
                Effects[i] = effects[i];
        }
    }

    public class PlayerDataEffect
    {
        public readonly PlayerStat PlayerStat;
        public readonly float EffectValue;

        public PlayerDataEffect(PlayerStat stat, float effectValue)
        {
            PlayerStat = stat;
            EffectValue = effectValue;
        }
    }
}