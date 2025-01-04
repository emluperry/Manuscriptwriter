using System;
using System.Collections.Generic;
using Demo.Entity;
using UnityEngine;

namespace Demo.Player
{
    public class PlayerController : ControllerBase
    {
        [SerializeField] private EntityInitialiser playerObject;

        private IEnumerable<IControllerComponent> components;

        public override void SetupController()
        {
            components = this.GetComponentsInChildren<IControllerComponent>();
            foreach (var controllerComponent in components)
            {
                controllerComponent.SetupController(this);
            }
        }

        public void SetupPlayer(EntityInitialiser player)
        {
            if (this.playerObject == null)
            {
                return;
            }

            foreach (var comp in components)
            {
                comp.SetupPlayer(playerObject);
            }
        }
    }
}
