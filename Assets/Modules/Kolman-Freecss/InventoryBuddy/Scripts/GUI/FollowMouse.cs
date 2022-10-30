using UnityEngine;

namespace Ragnarok
{
    public class FollowMouse : MonoBehaviour
    {
        void Update()
        {
            transform.position = Input.mousePosition; //keep the UI attached to the mouses position...soft parenting 
        }
    }
}