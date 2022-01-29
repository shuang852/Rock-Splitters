using System.Globalization;
using Managers;
using RockSystem.Chunks;
using RockSystem.Fossils;
using ToolSystem;
using UI.Core;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace UI.Cleaning
{
    public class FossilExposure : DialogueComponent<CleaningDialogue>
    {
        [SerializeField] private Text text;

        private ToolManager toolManager;
        private FossilShape fossilShape;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            toolManager = M.GetOrThrow<ToolManager>();
            fossilShape = M.GetOrThrow<FossilShape>();
            
            toolManager.toolUsed.AddListener(UpdateText);
            
            // BUG: Race condition with FossilShape leads to NaN being displayed.
            UpdateText();
        }

        private void UpdateText()
        {
            text.text = Mathf.Round(fossilShape.FossilExposure() * 100).ToString(CultureInfo.InvariantCulture);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }
    }
}