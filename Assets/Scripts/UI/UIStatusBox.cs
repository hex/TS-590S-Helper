using DG.Tweening;
using Prime31.StateKitLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum StatusBoxState
{
    Active,
    Disabled
}

public enum StatusBoxType
{
    Network,
    Com,
    Remote,
    Midi
}

public class UIStatusBox : StateKitLite<StatusBoxState>
{
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private StatusBoxType _type;
    [SerializeField] private Image _loader;

    private Sequence _sequence;

    private void Start()
    {
        initialState = StatusBoxState.Disabled;
        AnimateLoading();
    }

    private void AnimateLoading()
    {
        _sequence = DOTween.Sequence();

        _sequence
            .Append(_loader.DOFade(.5f, .2f))
            .AppendInterval(.1f)
            .Append(_loader.DOFade(1f, .1f))
            .SetLoops(1);
    }

    private void Active_Enter()
    {
        _statusText.color = ColorManager.Instance.Green;
        var successText = "";

        switch (_type)
        {
            case StatusBoxType.Network:
                successText = "Online";
                break;

            case StatusBoxType.Com:
                successText = SerialManager.Instance.GetPortName();
                break;

            case StatusBoxType.Remote:
                successText = "Reachable";
                break;

            case StatusBoxType.Midi:
                successText = MIDIManager.Instance.GetDeviceName();
                break;
        }

        _statusText.text = successText;
        
        _sequence.OnComplete(() =>
        {
            _loader.DOBlendableColor(ColorManager.Instance.Green, .2f);
        });
    }

    private void Disabled_Enter()
    {
        _statusText.color = ColorManager.Instance.Red;
        var successText = "";

        switch (_type)
        {
            case StatusBoxType.Network:
                successText = "Offline";
                break;

            case StatusBoxType.Com:
                successText = "Error";
                break;

            case StatusBoxType.Remote:
                successText = "Unreachable";
                break;

            case StatusBoxType.Midi:
                successText = "No MIDI input";
                break;
        }

        _statusText.text = successText;
        
        _loader.DOBlendableColor(ColorManager.Instance.Red, .2f);
    }
}