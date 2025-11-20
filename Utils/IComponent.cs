using UnityEngine;

namespace Replay.Utils
{
    public interface IComponent
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }
        public bool enabled { get; set; }
       
    }
    public static class IComponentExtensions
    {
        
    }
}