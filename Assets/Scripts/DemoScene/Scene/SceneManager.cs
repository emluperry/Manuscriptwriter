using System;
using Demo.Camera;
using Demo.Entity;
using Demo.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Demo.Scene
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private EntityInitialiser player;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private FollowCamera Camera;

        private void Awake()
        {
            playerController.SetupController();
            playerController.SetupPlayer(player);
            
            Camera.SetTarget(player.transform);
        }
    }
}
