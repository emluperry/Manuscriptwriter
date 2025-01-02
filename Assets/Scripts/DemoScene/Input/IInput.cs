using System;
using UnityEngine;

namespace Demo.Input
{
    public interface IInput
    {
        Action<string> SwitchControlMap { get; set; }
        
        void SetupInput(InputSystem_Actions inputs);
        void EnableInput();
        void DisableInput();
    }
}
