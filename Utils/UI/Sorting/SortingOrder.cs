using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils.UI
{
    public enum SortingOrder
    {
        undef = -1,
        
        UI = 1000,
        HUD = 5000,
        
        UILevel2 = 10000,
        HUDLevel2 = 15000,
        
        UILevel3 = 20000,
        HUDLevel3 = 25000,
        
        StoreOrSettings = 29000,
        
        HUDStats = 29500,
        
        ModalDialog = 30000,
        
        DropDownContent = 31500,
        
        Transition = 32000,
        
        DebugInfo = 32100,
        
        AboveAll = 32500
    }

    public static class SortingOrderExtensions
    {
        public static void SetOn(this SortingOrder sortingOrder, Canvas canvas, int orderInGroup = 0)
        {
            if(sortingOrder != SortingOrder.undef)
                canvas.sortingOrder = sortingOrder.intValue() + orderInGroup;  
        } 

    }
}
