using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : Button
{
    public DJControllerControl Control = DJControllerControl.None;
    private Image _buttonImage;
    private TMP_Text _buttonLabel;

    private readonly Color _normalLabelColor = new Color(234 / 255f, 250 / 255f, 255 / 255f, 255 / 255f);
    private readonly Color _pressedLabelColor = new Color(107 / 255f, 161 / 255f, 178 / 255f, 255 / 255f);
    private float _initialY;

    protected override void Awake()
    {
        base.Awake();

        _initialY = transform.localPosition.y;

        this.onClick.AddListener(OnClick);

        _buttonImage = GetComponentInChildren<Image>();
        _buttonLabel = GetComponentInChildren<TMP_Text>();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        switch (state)
        {
            case SelectionState.Normal:
                SetNormal();
                break;
            case SelectionState.Pressed:
                if (AppManager.Instance.IsEditControlsActive)
                    SetPressed();
                break;
            case SelectionState.Highlighted:
                SetHighlighted();
                break;
        }
    }

    public void SetPressed()
    {
        transform.DOLocalMoveY(_initialY - 3f, .1f);
        //_buttonImage.sprite = spriteState.pressedSprite;
        _buttonLabel.DOColor(ColorManager.Instance.Blue, .1f);

        _buttonLabel.color = ColorManager.Instance.Blue;
        _buttonLabel.font = AppManager.Instance.BoldFont;
    }

    public void SetNormal()
    {
        transform.DOScale(1f, .1f);
        transform.DOLocalMoveY(_initialY, .1f);
        //_buttonImage.sprite = spriteState.highlightedSprite;
        if (ColorManager.Instance != null)
            _buttonLabel.DOColor(ColorManager.Instance.MidBrown, .1f);
        _buttonLabel.font = AppManager.Instance.RegularFont;
    }

    public void SetHighlighted()
    {
        if (AppManager.Instance.IsEditControlsActive)
        {
            transform.DOScale(1.1f, .1f);
            _buttonLabel.DOColor(ColorManager.Instance.Green, .1f);
            _buttonLabel.font = AppManager.Instance.BoldFont;
        }
    }

    void OnClick()
    {
        AppManager.Instance.ControlEditor.EditControl(this);
    }
}