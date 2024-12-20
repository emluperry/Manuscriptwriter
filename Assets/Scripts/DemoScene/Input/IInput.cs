using UnityEngine;

namespace Demo.Input
{
    public interface IInput
    {
        void SetupInput(InputSystem_Actions inputs);
        
        void EnableInput();
        void DisableInput();
    }
}
