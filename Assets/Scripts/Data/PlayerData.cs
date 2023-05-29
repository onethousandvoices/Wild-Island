using System;
using UnityEngine;

namespace WildIsland.Data
{
    [Serializable]
    public class PlayerData : IGameData
    {
        [Sheet("headHealth")] public PlayerHeadHealth HeadHealth = new PlayerHeadHealth();
        [Sheet("bodyHealth")] public PlayerBodyHealth BodyHealth = new PlayerBodyHealth();
        [Sheet("leftArmHealth")] public PlayerLeftArmHealth LeftArmHealth = new PlayerLeftArmHealth();
        [Sheet("rightArmHealth")] public PlayerRightArmHealth RightArmHealth = new PlayerRightArmHealth();
        [Sheet("leftLegHealth")] public PlayerLeftLegHealth LeftLegHealth = new PlayerLeftLegHealth();
        [Sheet("rightLegHealth")] public PlayerRightLegHealth RightLegHealth = new PlayerRightLegHealth();
        [Sheet("healthRegen")] public PlayerHealthRegen HealthRegen = new PlayerHealthRegen();
        [Sheet("stamina")] public PlayerStamina Stamina = new PlayerStamina();
        [Sheet("staminaRegen")] public PlayerStaminaRegen StaminaRegen = new PlayerStaminaRegen();
        [Sheet("hunger")] public PlayerHunger Hunger = new PlayerHunger();
        [Sheet("hungerDecrease")] public PlayerHungerDecrease HungerDecrease = new PlayerHungerDecrease();
        [Sheet("thirst")] public PlayerThirst Thirst = new PlayerThirst();
        [Sheet("thirstDecrease")] public PlayerThirstDecrease ThirstDecrease = new PlayerThirstDecrease();
        [Sheet("fatigue")] public PlayerFatigue Fatigue = new PlayerFatigue();
        [Sheet("fatigueDecrease")] public PlayerFatigueDecrease FatigueDecrease = new PlayerFatigueDecrease();
        [Sheet("regularSpeed")] public PlayerRegularSpeed RegularSpeed = new PlayerRegularSpeed();
        [Sheet("sprintSpeed")] public PlayerSprintSpeed SprintSpeed = new PlayerSprintSpeed();
        [Sheet("temperature")] public PlayerTemperature Temperature = new PlayerTemperature();
        [Sheet("healthRegenHungerStage1")] public PlayerHealthRegenHungerStage1 HealthRegenHungerStage1 = new PlayerHealthRegenHungerStage1();
        [Sheet("healthRegenHungerStage2")] public PlayerHealthRegenHungerStage2 HealthRegenHungerStage2 = new PlayerHealthRegenHungerStage2();
        [Sheet("healthRegenHungerStage3")] public PlayerHealthRegenHungerStage3 HealthRegenHungerStage3 = new PlayerHealthRegenHungerStage3();
        [Sheet("healthRegenHungerStage4")] public PlayerHealthRegenHungerStage4 HealthRegenHungerStage4 = new PlayerHealthRegenHungerStage4();
        [Sheet("healthRegenThirstStage1")] public PlayerHealthRegenThirstStage1 HealthRegenThirstStage1 = new PlayerHealthRegenThirstStage1();
        [Sheet("healthRegenThirstStage2")] public PlayerHealthRegenThirstStage2 HealthRegenThirstStage2 = new PlayerHealthRegenThirstStage2();
        [Sheet("healthRegenThirstStage3")] public PlayerHealthRegenThirstStage3 HealthRegenThirstStage3 = new PlayerHealthRegenThirstStage3();
        [Sheet("healthRegenThirstStage4")] public PlayerHealthRegenThirstStage4 HealthRegenThirstStage4 = new PlayerHealthRegenThirstStage4();

        public void SetValues()
        {
            HeadHealth.SetValue(HeadHealth.Default);
            BodyHealth.SetValue( BodyHealth.Default);
            LeftArmHealth.SetValue(LeftArmHealth.Default);
            RightArmHealth.SetValue(RightArmHealth.Default);
            LeftLegHealth.SetValue(LeftLegHealth.Default);
            RightLegHealth.SetValue(RightLegHealth.Default);
            HealthRegen.SetValue(HealthRegen.Default);
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

        public void SetDefaults(PlayerData data)
        {
            HeadHealth.SetValueAndDefault(data.HeadHealth.Value);
            BodyHealth.SetValueAndDefault(data.BodyHealth.Value);
            LeftArmHealth.SetValueAndDefault(data.LeftArmHealth.Value);
            RightArmHealth.SetValueAndDefault(data.RightArmHealth.Value);
            LeftLegHealth.SetValueAndDefault( data.LeftLegHealth.Value);
            RightLegHealth.SetValueAndDefault(data.RightLegHealth.Value);
            HealthRegen.SetValueAndDefault(data.HealthRegen.Value);
            Stamina.SetValueAndDefault(data.Stamina.Value);
            StaminaRegen.SetValueAndDefault(data.StaminaRegen.Value);
            Hunger.SetValueAndDefault(data.Hunger.Value);
            HungerDecrease.SetValueAndDefault(data.HungerDecrease.Value);
            Thirst.SetValueAndDefault(data.Thirst.Value);
            ThirstDecrease.SetValueAndDefault(data.ThirstDecrease.Value);
            Fatigue.SetValueAndDefault(data.Fatigue.Value);
            FatigueDecrease.SetValueAndDefault(data.FatigueDecrease.Value);
            RegularSpeed.SetValueAndDefault(data.RegularSpeed.Value);
            SprintSpeed.SetValueAndDefault(data.SprintSpeed.Value);
            Temperature.SetValueAndDefault(data.Temperature.Value);
            HealthRegenHungerStage1.SetValueAndDefault(data.HealthRegenHungerStage1.Value);
            HealthRegenHungerStage2.SetValueAndDefault(data.HealthRegenHungerStage2.Value);
            HealthRegenHungerStage3.SetValueAndDefault(data.HealthRegenHungerStage3.Value);
            HealthRegenHungerStage4.SetValueAndDefault(data.HealthRegenHungerStage4.Value);
            HealthRegenThirstStage1.SetValueAndDefault(data.HealthRegenThirstStage1.Value);
            HealthRegenThirstStage2.SetValueAndDefault(data.HealthRegenThirstStage2.Value);
            HealthRegenThirstStage3.SetValueAndDefault(data.HealthRegenThirstStage3.Value);
            HealthRegenThirstStage4.SetValueAndDefault(data.HealthRegenThirstStage4.Value);
        }

        public float HealthSum => HeadHealth.Value + BodyHealth.Value +
                                  LeftArmHealth.Value + RightArmHealth.Value +
                                  LeftLegHealth.Value + RightLegHealth.Value;

        public float CurrentRelativeHealth(float maxHealth)
            => HealthSum / maxHealth;
    }

    [Serializable]
    public abstract class VolatilePlayerStat : PlayerStat { }

    [Serializable]
    public abstract class PlayerStat
    {
        [field: SerializeField] public float Value { get; protected set; }
        [field: SerializeField] public float Default { get; protected set; }

        public void ApplyValue(float applied)
        {
            Value += applied;
            if (Value < 0)
                Value = 0;
        }

        public void SetValue(float value)
            => Value = value;

        public void SetValueAndDefault(float value)
        {
            Value = value;
            Default = value;
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
}