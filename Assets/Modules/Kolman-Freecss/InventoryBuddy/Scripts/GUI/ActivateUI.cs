using UnityEngine;

public class ActivateUI : MonoBehaviour
{
    private readonly KeyCode _keyCode = KeyCode.I;
    public Canvas CanvasToToggle;
    bool waiting;
    public bool startOff;

    void Start()
    {
        if (startOff)
        {
            CanvasToToggle.enabled = false;
        }
    }

    void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(this._keyCode))
            {
                CanvasToToggle.enabled = !CanvasToToggle.enabled;
            }
        }
    }
}
