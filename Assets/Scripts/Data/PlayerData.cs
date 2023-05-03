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

        public float HealthSum => HeadHealth.Value + BodyHealth.Value +
                                  LeftArmHealth.Value + RightArmHealth.Value +
                                  LeftLegHealth.Value + RightLegHealth.Value;

        public float CurrentRelativeHealth(float maxHealth)
            => HealthSum / maxHealth;

        public PlayerStat GetStatByType(Type type)
        {
            if (type == typeof(PlayerHeadHealth))
                return HeadHealth;
            if (type == typeof(PlayerBodyHealth))
                return BodyHealth;
            if (type == typeof(PlayerLeftArmHealth))
                return LeftArmHealth;
            if (type == typeof(PlayerRightArmHealth))
                return RightArmHealth;
            if (type == typeof(PlayerLeftLegHealth))
                return LeftLegHealth;
            if (type == typeof(PlayerRightLegHealth))
                return RightLegHealth;
            if (type == typeof(PlayerHealthRegen))
                return HealthRegen;
            if (type == typeof(PlayerStamina))
                return Stamina;
            if (type == typeof(PlayerStaminaRegen))
                return StaminaRegen;
            if (type == typeof(PlayerHunger))
                return Hunger;
            if (type == typeof(PlayerHungerDecrease))
                return HungerDecrease;
            if (type == typeof(PlayerThirst))
                return Thirst;
            if (type == typeof(PlayerThirstDecrease))
                return ThirstDecrease;
            if (type == typeof(PlayerFatigue))
                return Fatigue;
            if (type == typeof(PlayerFatigueDecrease))
                return FatigueDecrease;
            if (type == typeof(PlayerRegularSpeed))
                return RegularSpeed;
            if (type == typeof(PlayerSprintSpeed))
                return SprintSpeed;
            if (type == typeof(PlayerTemperature))
                return Temperature;
            if (type == typeof(PlayerHealthRegenHungerStage1))
                return HealthRegenHungerStage1;
            if (type == typeof(PlayerHealthRegenHungerStage2))
                return HealthRegenHungerStage2;
            if (type == typeof(PlayerHealthRegenHungerStage3))
                return HealthRegenHungerStage3;
            if (type == typeof(PlayerHealthRegenHungerStage4))
                return HealthRegenHungerStage4;
            if (type == typeof(PlayerHealthRegenThirstStage1))
                return HealthRegenThirstStage1;
            if (type == typeof(PlayerHealthRegenThirstStage2))
                return HealthRegenThirstStage2;
            if (type == typeof(PlayerHealthRegenThirstStage3))
                return HealthRegenThirstStage3;
            if (type == typeof(PlayerHealthRegenThirstStage4))
                return HealthRegenThirstStage4;

            return null;
        }
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
    public class PlayerHealthRegen : PlayerStat { }

    [Serializable]
    public class PlayerStamina : PlayerStat { }

    [Serializable]
    public class PlayerStaminaRegen : PlayerStat { }

    [Serializable]
    public class PlayerHunger : PlayerStat { }

    [Serializable]
    public class PlayerHungerDecrease : PlayerStat { }

    [Serializable]
    public class PlayerThirst : PlayerStat { }

    [Serializable]
    public class PlayerThirstDecrease : PlayerStat { }

    [Serializable]
    public class PlayerFatigue : PlayerStat { }

    [Serializable]
    public class PlayerFatigueDecrease : PlayerStat { }

    [Serializable]
    public class PlayerRegularSpeed : PlayerStat { }

    [Serializable]
    public class PlayerSprintSpeed : PlayerStat { }

    [Serializable]
    public class PlayerTemperature : PlayerStat { }

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
    public abstract class PlayerStat
    {
        public float Value;
    }
}