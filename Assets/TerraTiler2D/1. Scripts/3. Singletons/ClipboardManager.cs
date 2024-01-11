#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace TerraTiler2D
{
    ///<summary>Handles copy, cut, and paste actions.</summary>
    public class ClipboardManager : Singleton<ClipboardManager>
    {
        private List<GraphElement> clipboard = new List<GraphElement>();

        public void AddToClipboard(GraphElement element)
        {
            clipboard.Add(element);
        }

        public void SetClipboard(List<GraphElement> elements)
        {
            clipboard = elements;
        }

        public List<GraphElement> GetClipboard()
        {
            return clipboard;
        }

        public void ClearClipboard()
        {
            clipboard = new List<GraphElement>();
        }
    }
}
#endif
