using UnityEngine;

namespace WildIsland.Views.UI
{
    public class PlayerViewStatsHolder : MonoBehaviour
    {
        [field: SerializeField] public PlayerBodyStatView PlayerBodyStatView { get; private set; }
        [field: SerializeField] public PlayerHeadStatView PlayerHeadStatView { get; private set; }
        [field: SerializeField] public PlayerLeftArmStatView PlayerLeftArmStatView { get; private set; }
        [field: SerializeField] public PlayerRightArmStatView PlayerRightArmStatView { get; private set; }
        [field: SerializeField] public PlayerLeftLegStatView PlayerLeftLegStatView { get; private set; }
        [field: SerializeField] public PlayerRightLegStatView PlayerRightLegStatView { get; private set; }
        [field: SerializeField] public PlayerStaminaStatView PlayerStaminaStatView { get; private set; }
        [field: SerializeField] public PlayerHungerStatView PlayerHungerStatView { get; private set; }
        [field: SerializeField] public PlayerThirstStatView PlayerThirstStatView { get; private set; }
        [field: SerializeField] public PlayerFatigueStatView PlayerFatigueStatView { get; private set; }
        
        private void OnValidate()
        {
            PlayerBodyStatView = GetComponentInChildren<PlayerBodyStatView>();
            PlayerHeadStatView = GetComponentInChildren<PlayerHeadStatView>();
            PlayerLeftArmStatView = GetComponentInChildren<PlayerLeftArmStatView>();
            PlayerRightArmStatView = GetComponentInChildren<PlayerRightArmStatView>();
            PlayerLeftLegStatView = GetComponentInChildren<PlayerLeftLegStatView>();
            PlayerRightLegStatView = GetComponentInChildren<PlayerRightLegStatView>();
            PlayerStaminaStatView = GetComponentInChildren<PlayerStaminaStatView>();
            PlayerHungerStatView = GetComponentInChildren<PlayerHungerStatView>();
            PlayerThirstStatView = GetComponentInChildren<PlayerThirstStatView>();
            PlayerFatigueStatView = GetComponentInChildren<PlayerFatigueStatView>();

            // var gos = GetComponentsInChildren<Transform>();
            //
            // foreach (Transform go in gos)
            // {
            //     if (go.name != "Fill Area")
            //         continue;
            //     go.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            // }
        }
    }
}