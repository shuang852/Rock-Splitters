using Managers;
using ToolSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Effects
{
    public class ToolVibrator : MonoBehaviour
    {
        [Tooltip("How long the device will vibrate for Tapping tools")]
        [SerializeField] private long onceDelay = 100;
        [Tooltip("How long the device will vibrate for Holding tools")]
        [SerializeField] private long continuousDelay = 10;
        private ToolManager toolManager;

        // TODO: Uncomment to reenable. Needs more refinement in timings
        // private void Start()
        // {
        //     toolManager = M.GetOrThrow<ToolManager>();
        //     
        //     toolManager.toolDown.AddListener(VibrateOnce);
        //     toolManager.toolInUse.AddListener(VibrateContinuous);
        // }

        private void VibrateOnce(Vector2 pos)
        {
            Vibrator.Vibrate(onceDelay);
        }

        private void VibrateContinuous(Vector2 pos)
        {
            Vibrator.Vibrate(continuousDelay);
        }
    }
}
