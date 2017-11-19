using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype02
{
    public static class PhysicsExt
    {
        public static float CalculateMovementDistance(float velocity, float acceleration, float deltaTime)
        {
            return (velocity * deltaTime) + (0.5f * acceleration * Mathf.Pow(deltaTime, 2));
        }

        public static float CalculateVelocity(float currentVelocity, float acceleration, float deltaTime)
        {
            return currentVelocity + (acceleration * deltaTime);
        }
    }
}
