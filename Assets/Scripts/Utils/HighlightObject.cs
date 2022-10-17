using UnityEngine;

namespace StarterAssets.Utils
{
    public class HighlightObject : MonoBehaviour
    {
        public Color highlightColor = Color.yellow;
        public Color defaultColor = Color.white;
        public bool highlightOnMouseOver = true;
        
        public void Highlight()
        {
            gameObject.GetComponent<Renderer>().material.color = highlightColor;
        }
        
    }
}