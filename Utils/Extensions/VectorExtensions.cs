using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public static class VectorExtensions
    {
        public static Vector2 Rotated(this Vector2 v, float delta)
        {
            return v.RotatedRad(Mathf.Deg2Rad * delta);
        }
        public static Vector2 RotatedRad(this Vector2 v, float delta)
        {
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }

    }
}