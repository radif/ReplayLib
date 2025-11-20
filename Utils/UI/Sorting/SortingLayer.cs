using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils.UI
{
    public enum SortingLayer
    {
        undef = -1,
        Foundation = -20,
        Behind = -10,
        Default = 0,
        UI = 10,
        HUD = 20,
        Store = 30,
        Dialog = 40,
        Debug = 50,
        AboveAll = 60
    }
    public static class SortingLayerExtensions
    {
        public static string ToName(this SortingLayer sortingLayer) => sortingLayer.ConvertToString();
        public static int ToID(this SortingLayer sortingLayer) => UnityEngine.SortingLayer.NameToID(sortingLayer.ToName());

        public static void SetOn(this SortingLayer sortingLayer, Canvas canvas) => canvas.sortingLayerID = sortingLayer.ToID();

    }
}
