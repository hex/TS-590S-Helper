using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControlEditor : MonoBehaviour
{
    [Header("Components")] [SerializeField] private TMP_Text _controlName;
    [SerializeField] private TMP_Dropdown _functionsDropdown;
    [SerializeField] private Button _applyButton;

    [SerializeField] private Button _editControlsButton;

    private void Awake()
    {
        _editControlsButton.onClick.AddListener(ToggleControlEditor);
        gameObject.SetActive(false);
    }

    public void ToggleControlEditor()
    {
        var cb = _editControlsButton.colors;

        if (gameObject.activeSelf)
        {
            cb.normalColor = ColorManager.Instance.Blue;
            _editControlsButton.colors = cb;
            _editControlsButton.GetComponentInChildren<TMP_Text>().text = "Edit controls";
            gameObject.SetActive(false);

            AppManager.Instance.IsEditControlsActive = false;
        }
        else
        {
            cb.normalColor = ColorManager.Instance.Green;
            _editControlsButton.colors = cb;
            _editControlsButton.GetComponentInChildren<TMP_Text>().text = "Done";
            gameObject.SetActive(true);

            AppManager.Instance.IsEditControlsActive = true;
        }
    }

    public void EditControl(UIControl control)
    {
        _controlName.text = control.Control.ToString();
    }
}