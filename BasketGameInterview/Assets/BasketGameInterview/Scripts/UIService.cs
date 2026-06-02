using UnityEngine;

public class UIService : MonoBehaviour
{
    public GameObject MenuUI;

    public void HideMenu()
    {
        MenuUI.SetActive(false);
    }
    public void ShowMenu()
    {
        MenuUI.SetActive(true);
    }

    public void ShowEndScreen()
    {

    }
    public void HideEndScreen()
    {
    }
}
