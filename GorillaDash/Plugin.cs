using System;
using BepInEx;
using DashMonke;
using UnityEngine;
using System.ComponentModel;
using Utilla; //dependency for the mod to work

namespace GorillaDash
{
    [Description("HauntedModMenu")]
    [BepInDependency("org.legoandmars.gorillatag.utilla")]
    [BepInPlugin(DashBoostInfo.GUID, DashBoostInfo.Name, DashBoostInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private float dashCooldown = 1f;
        private float lastDashTime = -10f;
        private float dashForce = 20f;

        private void OnEnable()
        {

            GorillaTagger.OnPlayerSpawned(Init);

            Init();
        }

        private void OnDisable()
        {

            if (NetworkSystem.Instance != null)
            {
                NetworkSystem.Instance.OnJoinedRoomEvent -= OnJoinedRoom;
                NetworkSystem.Instance.OnReturnedToSinglePlayer -= OnLeftRoom;
            }
        }

        private void Init()
        {
            if (NetworkSystem.Instance == null)
                return;

            NetworkSystem.Instance.OnJoinedRoomEvent += OnJoinedRoom;
            NetworkSystem.Instance.OnReturnedToSinglePlayer += OnLeftRoom;
        }

        private void OnJoinedRoom()
        {
            if (NetworkSystem.Instance.GameModeString.Contains("MODDED"))
            {
                lastDashTime = -10f;
                Debug.Log("[GorillaDash] you're in a modded! dash away!.");
            }
        }

        private void OnLeftRoom()
        {
            lastDashTime = -10f;
            Debug.Log("[GorillaDash] awwh you left, come back again!");
        }

        private void Update()
        {
            if (NetworkSystem.Instance == null)
                return;

            if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.GameModeString.Contains("MODDED"))
                return;

            var player = GorillaLocomotion.GTPlayer.Instance;
            if (player == null) return;

            bool aPressed = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand)
                .TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool aButton) && aButton;


            if ((aPressed) && Time.time - lastDashTime >= dashCooldown)
            {
                Vector3 dashDirection = player.rightHandFollower.forward.normalized;
                player.GetComponent<Rigidbody>().velocity += dashDirection * dashForce;
                lastDashTime = Time.time;
                Debug.Log("[GorillaDash] dash mod triggered! woooooah!");
            }
        }

        private void FixedUpdate()
        {
            // might add some physics adjustments here in the future or something.
        }
    }
}
