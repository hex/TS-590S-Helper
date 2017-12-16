using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InternetReachabilityVerifier))]
public class UIStatusBoxes : MonoBehaviour
{
    private InternetReachabilityVerifier _internetReachabilityVerifier;

    [SerializeField] private UIStatusBox _networkStatus;
    [SerializeField] private UIStatusBox _comPortStatus;
    [SerializeField] private UIStatusBox _remoteStatus;
    [SerializeField] private UIStatusBox _midiDeviceStatus;

    // Returns true when there is verified internet access. 
    private bool IsNetVerified()
    {
        return _internetReachabilityVerifier.status == InternetReachabilityVerifier.Status.NetVerified;
    }

    private IEnumerator Start()
    {
        _internetReachabilityVerifier = GetComponent<InternetReachabilityVerifier>();
        _internetReachabilityVerifier.statusChangedDelegate += UpdateNetworkStatusBox;

        yield return new WaitForEndOfFrame();

        CheckBoxes();
    }

    public void CheckBoxes()
    {
        UpdateComPortStatusBox();
        UpdateRemoteStatusBox();
        UpdateMIDIDeviceStatus();
    }

    private void UpdateNetworkStatusBox(InternetReachabilityVerifier.Status status)
    {
        Debug.Log("Network status box changed: " + status);

        _networkStatus.ChangeState(status == InternetReachabilityVerifier.Status.NetVerified
            ? StatusBoxState.Active
            : StatusBoxState.Disabled);
    }

    private void UpdateComPortStatusBox()
    {
        var status = SerialManager.Instance.checkOpen();

        Debug.Log("Serial status box changed: " + status);
        _comPortStatus.ChangeState(status ? StatusBoxState.Active : StatusBoxState.Disabled);
    }

    private void UpdateRemoteStatusBox()
    {
        StartCoroutine(IUpdateRemoteStatus());
    }

    IEnumerator IUpdateRemoteStatus()
    {
        TCPManager.Instance.SetupSocket(
            AppManager.Instance.Settings["Remote Host"].StringValue,
            AppManager.Instance.Settings["Remote Port"].IntValue);

        yield return new WaitForEndOfFrame();
        var status = TCPManager.Instance.SocketReady;
        Debug.Log("Remote status box changed: " + status);
        _remoteStatus.ChangeState(status ? StatusBoxState.Active : StatusBoxState.Disabled);

        TCPManager.Instance.CloseSocket();
    }

    private void UpdateMIDIDeviceStatus()
    {
        _midiDeviceStatus.ChangeState(MIDIManager.Instance.GetDeviceCount() > 0
            ? StatusBoxState.Active
            : StatusBoxState.Disabled);
    }
}