using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI.WorldExplorationPanels
{
    public interface IPanel
    {
        event Action OnPanelClosed;
        void Open();
        void Close();
        bool IsOpen { get; }
    }
}