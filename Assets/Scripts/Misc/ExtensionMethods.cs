using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class ExtensionStuff
    {
        public static Vector3 Clamp(this ref Vector3 val, int min, int max) {
            val.x = Mathf.Clamp(val.x, min, max);
            val.y = Mathf.Clamp(val.y, min, max);
            val.z = Mathf.Clamp(val.z, min, max);
            return val;
        }
    }
}