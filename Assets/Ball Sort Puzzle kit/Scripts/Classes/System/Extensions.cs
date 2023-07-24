namespace MyApp.MyBSP
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    public static class Extensions
    {
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
        #region List
        public static void Move<T>(ref List<T> list, int oldIndex, int newIndex)
        {
            T temp = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, temp);
        }
        #endregion
        #region Vectors
        #region middle
        public static Vector3 getMiddle(Vector3 v1, Vector3 v2)
        {
            return (v1 + v2) / 2;
        }
        public static Vector3 ToCenter(this Vector3[] vectors)
        {
            Vector3 sum = Vector3.zero;
            if (vectors == null || vectors.Length < 1) return sum;
            for (int i = 0; i < vectors.Length; i++)
            {
                sum += vectors[i];
            }
            return sum / vectors.Length;
        }
        #endregion
        #region X,Y
        public static Vector2 ToXY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }
        public static Vector2[] ToXY_array(this Vector3[] v)
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < v.Length; i++)
            {
                result.Add(v[i].ToXY());
            }
            return result.ToArray();
        }
        #endregion
        #region X,Z
        public static Vector2 ToXZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
        public static Vector2[] ToXZ_array(this Vector3[] v)
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < v.Length; i++)
            {
                result.Add(v[i].ToXZ());
            }
            return result.ToArray();
        }
        #endregion
        #region X,Y
        public static Vector2 ToYZ(this Vector3 v)
        {
            return new Vector2(v.y, v.z);
        }
        public static Vector2[] ToYZ_array(this Vector3[] v)
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < v.Length; i++)
            {
                result.Add(v[i].ToYZ());
            }
            return result.ToArray();
        }
        #endregion
        #endregion
        public static T CreateDeepCopy<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                formatter.Deserialize(ms);
                return obj;
            }
        }
    }
}