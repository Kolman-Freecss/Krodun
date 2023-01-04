using System.Collections;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public enum DoorType
    {
        Entrance,
        Exit
    }
    
    public enum DoorState
    {
        Open,
        Closed
    }
    
    public class SceneItemHandler : MonoBehaviour
    {
        [SerializeField] 
        private DoorType doorType;
        public DoorType DoorType => doorType;
        private DoorState _doorState;
        private const float DelayOpenDoor = 0.5f;
        private const float DelayCloseDoor = 0.5f;

        public void OpenEntranceDoors()
        {
            RotateDoor(DoorType.Entrance, DoorState.Open);
        }
        
        public void CloseEntranceDoors()
        {
            RotateDoor(DoorType.Entrance, DoorState.Closed);
        }
        
        public void OpenExitDoors()
        {
            RotateDoor(DoorType.Exit, DoorState.Open);
        }
        
        public void CloseExitDoors()
        {
            RotateDoor(DoorType.Exit, DoorState.Closed);
        }

        private void RotateDoor(DoorType doorT, DoorState doorState)
        {
            if (doorT != doorType)
                return;
            switch (doorState)
            {
                case DoorState.Open:
                    // Close entrance door
                    StartCoroutine(OpenDoorsCoroutine());
                    break;
                case DoorState.Closed:
                    // Close exit door
                    StartCoroutine(CloseDoorsCoroutine());
                    break;
            }
        }
        
        // TODO: Smoothly open the door
        IEnumerator OpenDoorsCoroutine()
        {
            var door1 = transform.GetChild(0);
            var door2 = transform.GetChild(1);
            yield return new WaitForSeconds(DelayOpenDoor);
            door1.transform.Rotate(0f, 90f, 0f);
            door2.transform.Rotate(0f, -90f, 0f);
        }
        
        // TODO: Smoothly close the door
        IEnumerator CloseDoorsCoroutine()
        {
            var door1 = transform.GetChild(0);
            var door2 = transform.GetChild(1);
            yield return new WaitForSeconds(DelayCloseDoor);
            door1.transform.Rotate(0f, -90f, 0f);
            door2.transform.Rotate(0f, 90f, 0f);
        }
    }
}