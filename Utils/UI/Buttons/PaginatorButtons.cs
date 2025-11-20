using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Replay.Utils.UI
{
    public class PaginatorButtons : MonoBehaviour
    {
        [SerializeField] ToggleGroup toggleGroup;
        [SerializeField] Toggle toggleDonnor;
        
        
        //events
        public Action<PaginatorButtons, int> onPaginatorPageSelectedHandler;
        [SerializeField] public UnityEvent onPaginatorPageSelected;

        private List<Toggle> _toggles = new();
        
        private int _currentPage = 0;
        public int currentPage {
            get => _currentPage;
            set
            {
                if(_currentPage != value)
                    SetCurrentPage(value, true);
            }
        }

        public int pageCount
        {
            get => _toggles.Count;
            set
            {
                if(_toggles.Count != value)
                    Populate(value);
            }
        }

        private void Awake()
        {
            toggleDonnor.gameObject.SetActive(false);
        }

        #region page support
        void Clear() {
            _toggles.DestoryContentsAndClear();
        } 
        void Populate(int pageCount)
        {
            Clear();
            toggleDonnor.gameObject.SetActive(true);
            for (int i = 0; i < pageCount; i++)
            {
                var toggle = Instantiate(toggleDonnor, toggleGroup.transform);
                
                toggle.group = toggleGroup;
                toggle.isOn = false;
                int pageIndex = i;
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                        OnPaginatorButtonClicked(pageIndex);
                });
                _toggles.Add(toggle);
            }
            toggleDonnor.gameObject.SetActive(false);
        }

        public void SetCurrentPageWithoutNotifying(int pageIndex)
        {
            SetCurrentPage(pageIndex, false);
        }
        void SetCurrentPage(int pageIndex, bool notifyDelegate = true)
        {
            _currentPage = pageIndex;
            RenderCurrentPage(notifyDelegate);
            if (notifyDelegate)
            {
                //call handlers
                onPaginatorPageSelected?.Invoke();
                onPaginatorPageSelectedHandler?.Invoke(this, _currentPage);
            }
        }
        void RenderCurrentPage(bool notifyDelegate = true) {
            for (int i = 0; i < _toggles.Count; i++)
            {
                if(notifyDelegate)
                    _toggles[i].isOn = i == _currentPage;
                else
                    _toggles[i].SetIsOnWithoutNotify(i == _currentPage);
            }
                
        }
        bool _interactable = true;
        public bool interactable
        {
            get => _interactable;
            set
            {
                if (_interactable != value)
                {
                    _interactable = value;
                    foreach (var toggle in _toggles)
                        toggle.enabled = value;
                }
            }
        }
#endregion

#region Callbacks
        public void OnPaginatorButtonClicked(int pageIndex)
        {
            currentPage = pageIndex;
        }
#endregion
        
    }
}
