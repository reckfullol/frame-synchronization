using MainClient;
using UnityEngine;

public class Entrance : MonoBehaviour {
    private void Awake() {
        Application.targetFrameRate = -1;
        Input.multiTouchEnabled = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Start() {
        GameClient.Instance.Init();
    }

    private void Update() {
        GameClient.Instance.Update(Time.deltaTime);
    }

    private void LateUpdate() {
        GameClient.Instance.LateUpdate();
    }

    private void FixedUpdate() {
        GameClient.Instance.FixedUpdate();
    }

    private void OnApplicationQuit() {
        GameClient.OnApplicationQuit();
    }

    private void OnDestroy() {

    }
}