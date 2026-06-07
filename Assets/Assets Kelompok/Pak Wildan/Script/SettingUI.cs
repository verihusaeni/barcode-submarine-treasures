using UnityEngine;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private GameObject panelSetting;

    public void OpenSetting()
    {
        panelSetting.SetActive(true);
    }

    public void CloseSetting()
    {
        panelSetting.SetActive(false);
    }
}