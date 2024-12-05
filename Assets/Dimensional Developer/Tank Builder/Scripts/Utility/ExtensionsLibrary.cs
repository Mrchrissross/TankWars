using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DimensionalDeveloper.TankBuilder.Utility
{
    public static class ExtensionsLibrary
    {
        /// <summary>
        /// Checks whether value is near to zero within a tolerance.
        /// </summary>

        public static bool IsZero(this float value) => !(Mathf.Abs(value) > 0.0f);
        
        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>

        public static bool IsZero(this Vector2 vector) => vector.sqrMagnitude < 9.99999943962493E-11;
        
        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>

        public static bool IsZero(this Vector3 vector) => vector.sqrMagnitude < 9.99999943962493E-11;
        
        /// <summary>
        /// Checks whether vector is equal to the input.
        /// </summary>

        public static bool IsEqual(this Vector2 vector, Vector2 value) => vector == value;
        
        /// <summary>
        /// Checks whether vector is equal to the input.
        /// </summary>

        public static bool IsEqual(this Vector3 vector, Vector3 value) => vector == value;
        
        /// <summary>
        /// Returns x and y as a vector2.
        /// </summary>
        
        public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);
        
        /// <summary>
        /// Returns y and z as a vector2.
        /// </summary>
        
        public static Vector2 yz(this Vector3 v) => new Vector2(v.y, v.z);
        
        /// <summary>
        /// Returns x and z as a vector2.
        /// </summary>
        
        public static Vector2 xz(this Vector3 v) => new Vector2(v.x, v.z);

        /// <summary>
        /// Returns a copy of given vector with only X component of the vector.
        /// </summary>

        public static Vector3 OnlyX(this Vector3 vector3) => new Vector3(vector3.x, 0.0f, 0.0f);

        /// <summary>
        /// Returns a copy of given vector with only Y component of the vector.
        /// </summary>

        public static Vector3 OnlyY(this Vector3 vector3) => new Vector3(0.0f, vector3.y, 0.0f);

        /// <summary>
        /// Returns a copy of given vector with only Z component of the vector.
        /// </summary>

        public static Vector3 OnlyZ(this Vector3 vector3) => new Vector3(0.0f, 0.0f, vector3.z);

        /// <summary>
        /// Returns a copy of given vector with only X and Z components of the vector.
        /// </summary>

        public static Vector3 OnlyXZ(this Vector3 vector3) => new Vector3(vector3.x, 0.0f, vector3.z);
        
        /// <summary>
        /// Returns a copy of given vector with the X component replaced with the given value.
        /// </summary>
        
        public static Vector3 WithX(this Vector3 vector3, float x) => new Vector3(x, vector3.y, vector3.z);

        /// <summary>
        /// Returns a copy of given vector with the Y component replaced with the given value.
        /// </summary>
        
        public static Vector3 WithY(this Vector3 vector3, float y) => new Vector3(vector3.x, y, vector3.z);
        
        /// <summary>
        /// Returns a copy of given vector with the Z component replaced with the given value.
        /// </summary>
        
        public static Vector3 WithZ(this Vector3 vector3, float z) => new Vector3(vector3.x, vector3.y, z);

        /// <summary>
        /// Returns a copy of given vector with the X component replaced with the given value.
        /// </summary>

        public static Vector2 WithX(this Vector2 vector3, float x) => new Vector2(x, vector3.y);
	
        /// <summary>
        /// Returns a copy of given vector with the Y component replaced with the given value.
        /// </summary>
        
        public static Vector2 WithY(this Vector2 vector3, float y) => new Vector2(vector3.x, y);
	
        /// <summary>
        /// Returns a copy of given vector as a Vector3 with its Z component as the given value.
        /// </summary>
        
        public static Vector3 WithZ(this Vector2 vector3, float z) => new Vector3(vector3.x, vector3.y, z);

        /// <summary>
        /// Checks whether a vector is lower than other vector. Useful on extremely small vectors.
        /// </summary>

        public static bool IsLowerThan(this Vector3 v1, Vector3 v2)
        {
            return v1.sqrMagnitude < v2.sqrMagnitude;
        }
        
        /// <summary>
        /// Checks whether a vector is higher than other vector. Useful on extremely small vectors.
        /// </summary>

        public static bool IsHigherThan(this Vector3 v1, Vector3 v2)
        {
            return v1.sqrMagnitude > v2.sqrMagnitude;
        }
        
        /// <summary>
        /// Checks whether vector is exceeding the magnitude within a small error tolerance.
        /// </summary>

        public static bool IsExceeding(this Vector2 vector, float magnitude)
        {
            const float errorTolerance = 1.01f;
            return vector.sqrMagnitude > (magnitude * magnitude) * errorTolerance;
        }
        
        /// <summary>
        /// Checks whether vector is exceeding the magnitude within a small error tolerance.
        /// </summary>

        public static bool IsExceeding(this Vector3 vector, float magnitude)
        {
            const float errorTolerance = 1.01f;
            return vector.sqrMagnitude > (magnitude * magnitude) * errorTolerance;
        }

        /// <summary>
        /// Checks if any of the elements within the vector return as NaN (not a number).
        /// </summary>
        
        public static bool IsNaN(this Vector2 vector) => float.IsNaN(vector.x) || float.IsNaN(vector.y);
        
        /// <summary>
        /// Checks if any of the elements within the vector return as NaN (not a number).
        /// </summary>
        
        public static bool IsNaN(this Vector3 vector) => float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);

        /// <summary>
        /// Returns a copy of the given vector squared.
        /// </summary>

        public static float Pow(this float f, float power)
        {
            return Mathf.Pow(f, power);
        }
        
        /// <summary>
        /// Dot product of two vectors. (Calculates how much this vector is pointing in the same direction as the other.)
        /// Returns positive if the two vectors are pointing in similar directions, zero if they are perpendicular,
        /// and negative if the two vectors are pointing in nearly opposite directions
        /// </summary>

        public static float Dot(this Vector3 v1, Vector3 v2)
        {
            return Vector3.Dot(v1, v2);
        }
        
        /// <summary>
        /// Cross product of two vectors.
        /// </summary>

        public static Vector3 Cross(this Vector3 v1, Vector3 v2)
        {
            return Vector3.Cross(v1, v2);
        }

        /// <summary>
        /// Returns the vector adjusted to be tangent to a specified surface normal.
        /// </summary>
        
        public static Vector3 Tangent(this Vector3 vector3, Vector3 normal)
        {
            var right = Vector3.Cross(vector3, Vector3.up);
            var tangent = Vector3.Cross(normal, right);

            return tangent.normalized;
        }
        
        /// <summary>
        /// Projects a given point onto the plane defined by plane origin and plane normal.
        /// </summary>
        /// <param name="point">The point to be projected.</param>
        /// <param name="planeOrigin">A point on the plane.</param>
        /// <param name="planeNormal">The plane normal.</param>

        public static Vector3 ProjectPointOnPlane(Vector3 point, Vector3 planeOrigin, Vector3 planeNormal)
        {
            var toPoint = point - planeOrigin;
            var toPointProjected = Vector3.Project(toPoint, planeNormal.normalized);

            return point - toPointProjected;
        }

        /// <summary>
        /// Rounds to the given number of decimal places.
        /// </summary>

        public static float Round(this float f, int places)
        {
            var power = Mathf.Pow(10, places);
            return Mathf.Round(Mathf.Abs(f) * power) / power;
        }
        
        /// <summary>
        /// Completely resets the transform to its default state.
        /// </summary>
        
        public static void ResetTransform(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }
        
        /// <summary>
        /// Destroys all objects that are parented to the transform.
        /// </summary>
        
        public static void DestroyChildren(this Transform trans)
        {
            while (trans.childCount > 0)
            {
                foreach (Transform child in trans)
                {
                    if (child) UnityEngine.Object.DestroyImmediate(child.gameObject);
                }
            }
        }
        
        /// <summary>
        /// Applies an impulse in the given direction.
        /// </summary>
        
        public static void ApplyImpulse(this Rigidbody rigidbody, float impulse, Vector3 direction)
        {
            var verticalImpulse = direction * impulse;
            rigidbody.linearVelocity = rigidbody.linearVelocity.OnlyXZ() + verticalImpulse;
        }
        
        /// <summary>
        /// Applies an upward impulse along the y-axis.
        /// </summary>
        
        public static void ApplyUpwardImpulse(this Rigidbody rigidbody, float impulse)
        {
            var verticalImpulse = Vector3.up * impulse;
            rigidbody.linearVelocity = rigidbody.linearVelocity.OnlyXZ() + verticalImpulse;
        }
        
        /// <summary>
        /// Gradually changes a quaternion towards a desired goal over time.
        /// </summary>
        
        public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time) 
        {
            if (Time.deltaTime < Mathf.Epsilon) return rot;
            
            // account for double-cover
            var dot = Quaternion.Dot(rot, target);
            var multi = dot > 0f ? 1f : -1f;
            target.x *= multi;
            target.y *= multi;
            target.z *= multi;
            target.w *= multi;
            
            // smooth damp (nlerp approx)
            var result = new Vector4(
                Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
                Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
                Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
                Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
            ).normalized;
		
            // ensure deriv is tangent
            var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), result);
            deriv.x -= derivError.x;
            deriv.y -= derivError.y;
            deriv.z -= derivError.z;
            deriv.w -= derivError.w;		
		
            return new Quaternion(result.x, result.y, result.z, result.w);
        }

        /// <summary>
        /// Initialises the ignore raycast layer and then ensures that the provided layer mask
        /// does not include the ignore raycast layer.
        /// </summary>
        /// <param name="ignoreLayer">The ignore layer to initialise.</param>
        /// <param name="layerMask">Layer mask to initialise.</param>
        
        public static void SetLayerMask(ref LayerMask layerMask, ref int ignoreLayer)
        {
            // Initialise the ignore for easy access.
            ignoreLayer = LayerMask.NameToLayer("Ignore Raycast");
            
            // Ensure the the layermask does not include the 'Ignore Raycast' layer.
            layerMask &= ~(1 << ignoreLayer);
        }
        
        /// <summary>
        /// Used on awake to ensure that vital components are attached to this gameobject.
        /// </summary>
        /// <param name="component">The component to check.</param>
        /// <param name="typeName">The components name and its type.
        /// (eg. component, scriptable object)
        /// (ex: RotationController component)</param>
        /// <param name="name">The name of object.</param>
        /// <typeparam name="T"></typeparam>
        
        public static void CheckComponent<T>(T component, string typeName, string name)
        {
            if (component != null) return;
                
            Debug.LogError("Infomation: A " + typeName +
                           " has not been attached to the \""
                           + name + "\" game object. Please ensure one is added.");
            Quit();
        }
        
        /// <summary>
        /// Quits the application.
        /// </summary>
        
        private static void Quit()
        {
            Debug.LogWarning("*****Quitting Game*****");
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif

            Application.Quit();
        }
    }
}

