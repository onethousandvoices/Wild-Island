using System;
using UnityEngine;

namespace WildIsland.Data
{
    [Serializable]
    public class PlayerData : IGameData
    {
        [Sheet("headHealth")] public PlayerHeadHealth HeadHealth;
        [Sheet("bodyHealth")] public PlayerBodyHealth BodyHealth;
        [Sheet("leftArmHealth")] public PlayerLeftArmHealth LeftArmHealth;
        [Sheet("rightArmHealth")] public PlayerRightArmHealth RightArmHealth;
        [Sheet("leftLegHealth")] public PlayerLeftLegHealth LeftLegHealth;
        [Sheet("rightLegHealth")] public PlayerRightLegHealth RightLegHealth;
        [Sheet("healthRegen")] public PlayerHealthRegen HealthRegen;
        [Sheet("stamina")] public PlayerStamina Stamina;
        [Sheet("staminaRegen")] public PlayerStaminaRegen StaminaRegen;
        [Sheet("hunger")] public PlayerHunger Hunger;
        [Sheet("hungerDecrease")] public PlayerHungerDecrease HungerDecrease;
        [Sheet("thirst")] public PlayerThirst Thirst;
        [Sheet("thirstDecrease")] public PlayerThirstDecrease ThirstDecrease;
        [Sheet("fatigue")] public PlayerFatigue Fatigue;
        [Sheet("fatigueDecrease")] public PlayerFatigueDecrease FatigueDecrease;
        [Sheet("regularSpeed")] public PlayerRegularSpeed RegularSpeed;
        [Sheet("sprintSpeed")] public PlayerSprintSpeed SprintSpeed;
        [Sheet("temperature")] public PlayerTemperature Temperature;
        [Sheet("healthRegenHungerStage1")] public PlayerHealthRegenHungerStage1 HealthRegenHungerStage1;
        [Sheet("healthRegenHungerStage2")] public PlayerHealthRegenHungerStage2 HealthRegenHungerStage2;
        [Sheet("healthRegenHungerStage3")] public PlayerHealthRegenHungerStage3 HealthRegenHungerStage3;
        [Sheet("healthRegenHungerStage4")] public PlayerHealthRegenHungerStage4 HealthRegenHungerStage4;
        [Sheet("healthRegenThirstStage1")] public PlayerHealthRegenThirstStage1 HealthRegenThirstStage1;
        [Sheet("healthRegenThirstStage2")] public PlayerHealthRegenThirstStage2 HealthRegenThirstStage2;
        [Sheet("healthRegenThirstStage3")] public PlayerHealthRegenThirstStage3 HealthRegenThirstStage3;
        [Sheet("healthRegenThirstStage4")] public PlayerHealthRegenThirstStage4 HealthRegenThirstStage4;

        public void SetDefaults()
        {
            HeadHealth.SetValue(HeadHealth.Default);
            BodyHealth.SetValue(BodyHealth.Default);
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

        public float HealthSum => HeadHealth.Value + BodyHealth.Value +
                                  LeftArmHealth.Value + RightArmHealth.Value +
                                  LeftLegHealth.Value + RightLegHealth.Value;

        public float CurrentRelativeHealth(float maxHealth)
            => HealthSum / maxHealth;

        public PlayerData(PlayerData data)
        {
            HeadHealth = new PlayerHeadHealth(data.HeadHealth.Value);
            BodyHealth = new PlayerBodyHealth(data.BodyHealth.Value);
            LeftArmHealth = new PlayerLeftArmHealth(data.LeftArmHealth.Value);
            RightArmHealth = new PlayerRightArmHealth(data.RightArmHealth.Value);
            LeftLegHealth = new PlayerLeftLegHealth(data.LeftLegHealth.Value);
            RightLegHealth = new PlayerRightLegHealth(data.RightLegHealth.Value);
            HealthRegen = new PlayerHealthRegen(data.HealthRegen.Value);
            Stamina = new PlayerStamina(data.Stamina.Value);
            StaminaRegen = new PlayerStaminaRegen(data.StaminaRegen.Value);
            Hunger = new PlayerHunger(data.Hunger.Value);
            HungerDecrease = new PlayerHungerDecrease(data.HungerDecrease.Value);
            Thirst = new PlayerThirst(data.Thirst.Value);
            ThirstDecrease = new PlayerThirstDecrease(data.ThirstDecrease.Value);
            Fatigue = new PlayerFatigue(data.Fatigue.Value);
            FatigueDecrease = new PlayerFatigueDecrease(data.FatigueDecrease.Value);
            RegularSpeed = new PlayerRegularSpeed(data.RegularSpeed.Value);
            SprintSpeed = new PlayerSprintSpeed(data.SprintSpeed.Value);
            Temperature = new PlayerTemperature(data.Temperature.Value);
            HealthRegenHungerStage1 = data.HealthRegenHungerStage1;
            HealthRegenHungerStage2 = data.HealthRegenHungerStage2;
            HealthRegenHungerStage3 = data.HealthRegenHungerStage3;
            HealthRegenHungerStage4 = data.HealthRegenHungerStage4;
            HealthRegenThirstStage1 = data.HealthRegenThirstStage1;
            HealthRegenThirstStage2 = data.HealthRegenThirstStage2;
            HealthRegenThirstStage3 = data.HealthRegenThirstStage3;
            HealthRegenThirstStage4 = data.HealthRegenThirstStage4;
        }
    }

    [Serializable]
    public abstract class VolatilePlayerStat : PlayerStat
    {
        protected VolatilePlayerStat(float value) : base(value) { }
    }

    [Serializable]
    public abstract class PlayerStat
    {
        [field: SerializeField] public float Value { get; protected set; }
        [field: SerializeField] public float Default { get; protected set; }

        protected PlayerStat(float value)
        {
            Value = value;
            Default = value;
        }

        public void ApplyValue(float applied)
        {
            Value += applied;
            if (Value < 0)
                Value = 0;
        }

        public void SetValue(float value)
            => Value = value;
    }

    [Serializable]
    public class PlayerHeadHealth : PlayerStat
    {
        public PlayerHeadHealth(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerBodyHealth : PlayerStat
    {
        public PlayerBodyHealth(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerLeftArmHealth : PlayerStat
    {
        public PlayerLeftArmHealth(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerRightArmHealth : PlayerStat
    {
        public PlayerRightArmHealth(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerLeftLegHealth : PlayerStat
    {
        public PlayerLeftLegHealth(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerRightLegHealth : PlayerStat
    {
        public PlayerRightLegHealth(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerHealthRegen : VolatilePlayerStat
    {
        public PlayerHealthRegen(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerStamina : PlayerStat
    {
        public PlayerStamina(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerStaminaRegen : VolatilePlayerStat
    {
        public PlayerStaminaRegen(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerHunger : PlayerStat
    {
        public PlayerHunger(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerHungerDecrease : VolatilePlayerStat
    {
        public PlayerHungerDecrease(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerThirst : PlayerStat
    {
        public PlayerThirst(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerThirstDecrease : VolatilePlayerStat
    {
        public PlayerThirstDecrease(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerFatigue : PlayerStat
    {
        public PlayerFatigue(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerFatigueDecrease : VolatilePlayerStat
    {
        public PlayerFatigueDecrease(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerRegularSpeed : VolatilePlayerStat
    {
        public PlayerRegularSpeed(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerSprintSpeed : VolatilePlayerStat
    {
        public PlayerSprintSpeed(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerTemperature : VolatilePlayerStat
    {
        public PlayerTemperature(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerHealthRegenHungerStage1 : PlayerStat
    {
        public PlayerHealthRegenHungerStage1(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerHealthRegenHungerStage2 : PlayerStat
    {
        public PlayerHealthRegenHungerStage2(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerHealthRegenHungerStage3 : PlayerStat
    {
        public PlayerHealthRegenHungerStage3(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerHealthRegenHungerStage4 : PlayerStat
    {
        public PlayerHealthRegenHungerStage4(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerHealthRegenThirstStage1 : PlayerStat
    {
        public PlayerHealthRegenThirstStage1(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerHealthRegenThirstStage2 : PlayerStat
    {
        public PlayerHealthRegenThirstStage2(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerHealthRegenThirstStage3 : PlayerStat
    {
        public PlayerHealthRegenThirstStage3(float value) : base(value) { }
    }

    [Serializable]
    public class PlayerHealthRegenThirstStage4 : PlayerStat
    {
        public PlayerHealthRegenThirstStage4(float value) : base(value) { }
    }
}