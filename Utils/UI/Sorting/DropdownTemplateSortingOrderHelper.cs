using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils.UI
{
    public class DropdownTemplateSortingOrderHelper : MonoBehaviour
    {
        public int sortingOrderToOverride = 32530;
        private void OnEnable()
        {
            Canvas canvas = GetComponent<Canvas>();
            if(canvas != null)
            {
                canvas.sortingOrder = sortingOrderToOverride;
 
            }
        }
    }

}
