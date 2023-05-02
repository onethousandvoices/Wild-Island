namespace Effects
{
    public class PermanentEffect : BaseEffect
    {
        public PermanentEffect(params PlayerDataEffect[] effects) : base(effects) { }

        public override bool IsApplying()
            => true;
    }
}