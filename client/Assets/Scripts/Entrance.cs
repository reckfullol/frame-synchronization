using UnityEngine;
using MainClient;

namespace UnityCompoent
{
    public class Entrance : MonoBehaviour
    {
        void Awake()
        {
            Application.targetFrameRate = -1;
            Input.multiTouchEnabled = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        void Start()
        {
            GameClient.Instance.Init();
        }

        void Update()
        {
            GameClient.Instance.Update(Time.deltaTime);
        }

        void LateUpdate()
        {
            GameClient.Instance.LateUpdate();
        }

        void FixedUpdate()
        {
            GameClient.Instance.FixedUpdate();
        }

        void OnApplicationQuit()
        {
            GameClient.Instance.OnApplicationQuit();
        }

        void OnDestroy()
        {

        }
     }
}
