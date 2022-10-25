using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    private static MenuManager Instance;
    private bool _isPaused = false;
    
    private void Awake()
    {
        if (mainMenu == null)
        {
            mainMenu = GameObject.FindGameObjectWithTag("Menu");
        }
        ManageSingleton();
    }
    
    void ManageSingleton()
    {
        if (Instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        ToggleMenu();
    }

    public void ToggleMenu()
    {
        _isPaused = !_isPaused;
        mainMenu.SetActive(_isPaused);
    }

    public void CloseMenu()
    {
        mainMenu.SetActive(false);
    }
}
