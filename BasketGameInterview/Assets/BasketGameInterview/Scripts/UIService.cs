using UnityEngine;

public class UIService : MonoBehaviour
{
    public Canvas Canvas;
    public GameObject MenuUI;
    Vector2 ToCanvasSpace(Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Canvas.GetComponent<RectTransform>(),
            screenPoint,
            null,
            out Vector2 local
        );
        return local;
    }
    public void DrawUILine(RectTransform line, Vector2 from, Vector2 to)
    {
        from = ToCanvasSpace(from);
        to = ToCanvasSpace(to);
        Vector2 dir = to - from;
        line.anchoredPosition = from;
        line.sizeDelta = new Vector2(dir.magnitude, 10f);
        line.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

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
