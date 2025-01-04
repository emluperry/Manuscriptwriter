using UnityEngine;

namespace Demo.Interaction
{
    [CreateAssetMenu(fileName = "II_InteractionIcon", menuName = "Scriptable Objects/Interactions/Create Interaction Icon", order = 0)]
    public class InteractionIcon : ScriptableObject
    {
        // Base Image
        [SerializeField] public Sprite InteractionImage;
    }
}
