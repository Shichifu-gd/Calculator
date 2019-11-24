using UnityEngine;

public class AdditionalInterface : MonoBehaviour
{
    [SerializeField] GameObject PanelUH;

    private void Awake()
    {
        PanelUH.SetActive(false);
    }

    public void UsageHistory()
    {
        if (PanelUH.activeSelf == true ) PanelUH.SetActive(false);
        else PanelUH.SetActive(true);
    }
}
