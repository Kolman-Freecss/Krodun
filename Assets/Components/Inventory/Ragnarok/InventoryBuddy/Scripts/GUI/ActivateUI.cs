using UnityEngine;

public class ActivateUI : MonoBehaviour
{
    private readonly KeyCode _keyCode = KeyCode.I;
    public GameObject uIToToggle;
    bool waiting;
    public bool startOff;

    void Start()
    {
        if (startOff)
        {
            uIToToggle.SetActive(false);
        }
    }

    void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(this._keyCode))
            {
                uIToToggle.SetActive(!uIToToggle.activeSelf);
            }
        }
    }
}
