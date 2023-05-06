using System;

namespace WildIsland.Data
{
    [Serializable]
    public class PlayerData : IGDDDataTypeString
    {
        public string ID => "MainCharacter";

        public PlayerHeadHealth HeadHealth;
        public PlayerBodyHealth BodyHealth;
        public PlayerLeftArmHealth LeftArmHealth;
        public PlayerRightArmHealth RightArmHealth;
        public PlayerLeftLegHealth LeftLegHealth;
        public PlayerRightLegHealth RightLegHealth;
        public PlayerHealthRegen HealthRegen;
        public PlayerStamina Stamina;
        public PlayerStaminaRegen StaminaRegen;
        public PlayerHunger Hunger;
        public PlayerHungerDecrease HungerDecrease;
        public PlayerThirst Thirst;
        public PlayerThirstDecrease ThirstDecrease;
        public PlayerFatigue Fatigue;
        public PlayerFatigueDecrease FatigueDecrease;
        public PlayerRegularSpeed RegularSpeed;
        public PlayerSprintSpeed SprintSpeed;
        public PlayerTemperature Temperature;
        public PlayerHealthRegenHungerStage1 HealthRegenHungerStage1;
        public PlayerHealthRegenHungerStage2 HealthRegenHungerStage2;
        public PlayerHealthRegenHungerStage3 HealthRegenHungerStage3;
        public PlayerHealthRegenHungerStage4 HealthRegenHungerStage4;
        public PlayerHealthRegenThirstStage1 HealthRegenThirstStage1;
        public PlayerHealthRegenThirstStage2 HealthRegenThirstStage2;
        public PlayerHealthRegenThirstStage3 HealthRegenThirstStage3;
        public PlayerHealthRegenThirstStage4 HealthRegenThirstStage4;

        public void SetDefaults()
        {
            HeadHealth.SetValue(HeadHealth.Default);
            BodyHealth.SetValue(BodyHealth.Default);
            LeftArmHealth.SetValue(LeftArmHealth.Default);
            RightArmHealth.SetValue(RightArmHealth.Default);
            LeftLegHealth.SetValue(LeftLegHealth.Default);
            RightLegHealth.SetValue(RightLegHealth.Default);
            Stamina.SetValue(Stamina.Default);
            StaminaRegen.SetValue(StaminaRegen.Default);
            Hunger.SetValue(Hunger.Default);
            HungerDecrease.SetValue(HungerDecrease.Default);
            Thirst.SetValue(Thirst.Default);
            ThirstDecrease.SetValue(ThirstDecrease.Default);
            Fatigue.SetValue(Fatigue.Default);
            FatigueDecrease.SetValue(FatigueDecrease.Default);
            RegularSpeed.SetValue(RegularSpeed.Default);
            SprintSpeed.SetValue(SprintSpeed.Default);
            Temperature.SetValue(Temperature.Default);
            HealthRegenHungerStage1.SetValue(HealthRegenHungerStage1.Default);
            HealthRegenHungerStage2.SetValue(HealthRegenHungerStage2.Default);
            HealthRegenHungerStage3.SetValue(HealthRegenHungerStage3.Default);
            HealthRegenHungerStage4.SetValue(HealthRegenHungerStage4.Default);
            HealthRegenThirstStage1.SetValue(HealthRegenThirstStage1.Default);
            HealthRegenThirstStage2.SetValue(HealthRegenThirstStage2.Default);
            HealthRegenThirstStage3.SetValue(HealthRegenThirstStage3.Default);
            HealthRegenThirstStage4.SetValue(HealthRegenThirstStage4.Default);
        }
        
        public float HealthSum => HeadHealth.Value + BodyHealth.Value +
                                  LeftArmHealth.Value + RightArmHealth.Value +
                                  LeftLegHealth.Value + RightLegHealth.Value;

        public float CurrentRelativeHealth(float maxHealth)
            => HealthSum / maxHealth;
    }

    [Serializable]
    public class PlayerHeadHealth : PlayerStat { }

    [Serializable]
    public class PlayerBodyHealth : PlayerStat { }

    [Serializable]
    public class PlayerLeftArmHealth : PlayerStat { }

    [Serializable]
    public class PlayerRightArmHealth : PlayerStat { }

    [Serializable]
    public class PlayerLeftLegHealth : PlayerStat { }

    [Serializable]
    public class PlayerRightLegHealth : PlayerStat { }

    [Serializable]
    public class PlayerHealthRegen : VolatilePlayerStat { }

    [Serializable]
    public class PlayerStamina : PlayerStat { }

    [Serializable]
    public class PlayerStaminaRegen : VolatilePlayerStat { }

    [Serializable]
    public class PlayerHunger : PlayerStat { }

    [Serializable]
    public class PlayerHungerDecrease : VolatilePlayerStat { }

    [Serializable]
    public class PlayerThirst : PlayerStat { }

    [Serializable]
    public class PlayerThirstDecrease : VolatilePlayerStat { }

    [Serializable]
    public class PlayerFatigue : PlayerStat { }

    [Serializable]
    public class PlayerFatigueDecrease : VolatilePlayerStat { }

    [Serializable]
    public class PlayerRegularSpeed : VolatilePlayerStat { }

    [Serializable]
    public class PlayerSprintSpeed : VolatilePlayerStat { }

    [Serializable]
    public class PlayerTemperature : VolatilePlayerStat { }

    [Serializable]
    public class PlayerHealthRegenHungerStage1 : PlayerStat { }

    [Serializable]
    public class PlayerHealthRegenHungerStage2 : PlayerStat { }

    [Serializable]
    public class PlayerHealthRegenHungerStage3 : PlayerStat { }

    [Serializable]
    public class PlayerHealthRegenHungerStage4 : PlayerStat { }

    [Serializable]
    public class PlayerHealthRegenThirstStage1 : PlayerStat { }

    [Serializable]
    public class PlayerHealthRegenThirstStage2 : PlayerStat { }

    [Serializable]
    public class PlayerHealthRegenThirstStage3 : PlayerStat { }

    [Serializable]
    public class PlayerHealthRegenThirstStage4 : PlayerStat { }

    [Serializable]
    public abstract class VolatilePlayerStat : PlayerStat { }

    [Serializable]
    public abstract class PlayerStat
    {
        public float Value { get; protected set; }
        public float Default { get; protected set; }

        public void ApplyValue(float applied)
        {
            Value += applied;
            if (Value < 0)
                Value = 0;
        }

        public void SetValue(float value)
            => Value = value;
    }
}