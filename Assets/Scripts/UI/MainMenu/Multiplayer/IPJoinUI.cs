using TMPro;
using UnityEngine;

namespace Kolman_Freecss.Krodun
{
    public class IPJoinUI : MonoBehaviour
    {

        [SerializeField] private TMP_InputField ipInput;
        [SerializeField] private TMP_InputField portInput;
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

        public void OnClickJoin()
        {
            if (_mediator != null)
            {
                _mediator.JoinWithIp(ipInput.text, portInput.text);
            }
        }
        
    }
}