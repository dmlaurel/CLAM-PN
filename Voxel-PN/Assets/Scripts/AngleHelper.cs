using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AngleHelper
{
    public static float ClampAngles(float angle, float minAngle, float maxAngle)
    {
        while (angle < -360f)
        {
            angle += 360f;
        }

        while (angle > 360f)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(angle, minAngle, maxAngle);
    }
}
