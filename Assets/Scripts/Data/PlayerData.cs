using System;

namespace WildIsland.Data
{
    [Serializable]
    public class PlayerData : IGDDDataTypeString
    {
        public string ID => "MainCharacter";

        public float HeadHealth = 0;
        public float BodyHealth = 0;
        public float LeftArmHealth = 0;
        public float RightArmHealth = 0;
        public float LeftLegHealth = 0;
        public float RightLegHealth = 0;
        public float HealthRegen = 0;
        public float Stamina = 0;
        public float StaminaRegen = 0;
        public float Hunger = 0;
        public float HungerDecrease = 0;
        public float Thirst = 0;
        public float ThirstDecrease = 0;
        public float Fatigue = 0;
        public float FatigueDecrease = 0;
        public float RegularSpeed = 0;
        public float SprintSpeed = 0;
        public float Temperature = 0;
        public float HealthRegenHungerStage1 = 0;
        public float HealthRegenHungerStage2 = 0;
        public float HealthRegenHungerStage3 = 0;
        public float HealthRegenHungerStage4 = 0;
        public float HealthRegenThirstStage1 = 0;
        public float HealthRegenThirstStage2 = 0;
        public float HealthRegenThirstStage3 = 0;
        public float HealthRegenThirstStage4 = 0;
    }
}