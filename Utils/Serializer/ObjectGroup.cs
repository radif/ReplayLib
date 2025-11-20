using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Replay.Utils
{

    
    
    [Serializable]
    public class ObjectGroup<T> where T : Object
    {
        public List<T> objects;

        public T activeObject => objects[_activeIndex];
        
        
        private int _activeIndex = 0;
        public int activeIndex
        {
            get => _activeIndex;
            set
            {
                _activeIndex = value;
                for (int i = 0; i < objects.Count; i++)
                {
                    T obj = objects[i];
                    if (obj == null) continue;
                    
                    GameObject o = null;
                    
                    if (obj is GameObject)
                        o = obj as GameObject;
                    else if(obj is Component)
                    {
                        var c = obj as Component;
                        o = c?.gameObject;
                    }
                     
                    o?.SetActive(i == _activeIndex);
                }
                    
            }
        }
    }
    
    
}

