using System;
using Managers;
using UnityEngine;

namespace UI.Cleaning
{
    public class PopupTextManager : Manager
    {
        [SerializeField] private GameObject Prefab;

        protected override void Start()
        {
            var go = Instantiate(Prefab, transform.position, Quaternion.identity, transform);
            go.GetComponent<PopupTextUI>().Setup("Test", Color.black, TextSize.Medium);
        }
    }
}