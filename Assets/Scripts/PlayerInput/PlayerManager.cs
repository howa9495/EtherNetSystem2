using UnityEngine;
using canvasSystem;

namespace PlayerSystems
{
    [DefaultExecutionOrder(-1)]
    public class PlayerManager : Singleton<PlayerManager>
    {
        public PlayerInputMapping playerInputMapping;
        public StaminaManager staminaManager;
        public InventoryCanvas inventoryCanvas;
        public InventoryManager inventoryManager;
        public Interactor interactor;


        private void Awake()
        {
            playerInputMapping = GetComponentInChildren<PlayerInputMapping>();
            staminaManager = GetComponentInChildren<StaminaManager>();
            inventoryManager = GetComponentInChildren<InventoryManager>();
            inventoryCanvas = GetComponentInChildren<InventoryCanvas>();
            interactor = GetComponentInChildren<Interactor>();
        }
    }

}

