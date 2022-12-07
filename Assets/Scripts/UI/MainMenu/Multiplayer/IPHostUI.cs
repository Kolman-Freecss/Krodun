using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kolman_Freecss.Krodun
{
    public class IPHostUI : MonoBehaviour
    {

        [SerializeField] private InputField ipInput;
        [SerializeField] private InputField portInput;
        [SerializeField] private IPUIMediator _mediator;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_mediator != null)
            {
                ipInput.text = _mediator.Ip;
                portInput.text = _mediator.Port.ToString();
            }
        }
        
        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
        
        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnClickHost()
        {
            if (_mediator != null)
            {
                _mediator.HostIpRequest(ipInput.text, portInput.text);
            }
        }
        
    }
}