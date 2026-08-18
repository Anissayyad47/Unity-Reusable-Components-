using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Show(GameObject panel)
    {
        if (panel == null)
            return;

        panel.SetActive(true);
    }

    public void Hide(GameObject panel)
    {
        if (panel == null)
            return;

        panel.SetActive(false);
    }

    public void Toggle(GameObject panel)
    {
        if (panel == null)
            return;

        panel.SetActive(!panel.activeSelf);
    }

    public void ShowOnly(GameObject panel, params GameObject[] panels)
    {
        HideAll(panels);
        Show(panel);
    }

    public void HideAll(params GameObject[] panels)
    {
        foreach (GameObject panel in panels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }
}
