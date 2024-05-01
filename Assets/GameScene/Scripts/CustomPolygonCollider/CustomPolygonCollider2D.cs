using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomPolygonCollider
{
    public enum ColliderType
    {
        PolygonCollider,
        EdgeCollider
    }

    [ExecuteInEditMode]
    public class CustomPolygonCollider2D : MonoBehaviour
    {
        private VerticeGenerator _verticeGenerator;

        public ColliderType ColliderType = ColliderType.PolygonCollider;
        [Range(0f, 2f)] public float Tolerance = 0f;

        private PolygonCollider2D _polygonCollider2D;
        private EdgeCollider2D[] _edgeCollider2D;

        private Sprite _sprite;


#if UNITY_EDITOR
        private void Reset()
        {
            _sprite = GetComponent<SpriteRenderer>().sprite;

            _verticeGenerator = new VerticeGenerator(_sprite);

            var collider2Ds = GetComponents<Collider2D>();
            foreach (var collider2D in collider2Ds)
            {
                DestroyAtEndOfFrame(collider2D);
            }

            UnityEditor.EditorApplication.delayCall += _verticeGenerator.GenerateVertices;
            UnityEditor.EditorApplication.delayCall += CreatePolygonCollider2D;
        }


        private void OnValidate()
        {
            switch (ColliderType)
            {
                case ColliderType.EdgeCollider:
                    CreateEdgeCollider();
                    break;
                case ColliderType.PolygonCollider:
                    CreatePolygonCollider2D();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        
        private void CreatePolygonCollider2D()
        {

            try
            {
                var collider2Ds = GetComponents<Collider2D>();
                foreach (var collider2D in collider2Ds.Where(x =>
                    x.GetType() != typeof(PolygonCollider2D) ||
                    x != collider2Ds.First(item => item is PolygonCollider2D)))
                {
                    DestroyAtEndOfFrame(collider2D);
                }

                if (GetComponent<PolygonCollider2D>())
                {
                    _polygonCollider2D = GetComponent<PolygonCollider2D>();
                }
                else
                {
                    _polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
                    _polygonCollider2D.offset = -_sprite.pivot / _sprite.pixelsPerUnit;
                }


                _polygonCollider2D.pathCount = _verticeGenerator.VerticePaths.Count;
                _verticeGenerator.SetSmoothness(Tolerance);

                for (var j = 0; j < _verticeGenerator.VerticePaths.Count; j++)
                {
                    var verticePath = new List<Vector2>(_verticeGenerator.VerticePaths[j]);

                    var points = new Vector2[verticePath.Count];

                    for (var i = 0; i < verticePath.Count; i++)
                    {
                        points[i] = verticePath[i];
                    }

                    _polygonCollider2D.SetPath(j, points);
                }
            }
            catch (Exception e)
            {
                print("You should Reset the component");
            }
        }

        private void CreateEdgeCollider()
        {
            try
            {

                var collider2Ds = GetComponents<Collider2D>();
                foreach (var collider2D in collider2Ds.Where(x => x.GetType() != typeof(EdgeCollider2D)))
                {
                    DestroyAtEndOfFrame(collider2D);
                }

                if (GetComponents<EdgeCollider2D>().Length == _verticeGenerator.VerticePaths.Count)
                {
                    _edgeCollider2D = GetComponents<EdgeCollider2D>();
                }
                else
                {
                    var edgeCollider2Ds = GetComponents<EdgeCollider2D>();
                    foreach (var edgeCollider2D in edgeCollider2Ds)
                    {
                        DestroyAtEndOfFrame(edgeCollider2D);
                    }

                    _edgeCollider2D = new EdgeCollider2D[_verticeGenerator.VerticePaths.Count];

                    for (int i = 0; i < _edgeCollider2D.Length; i++)
                    {
                        _edgeCollider2D[i] = gameObject.AddComponent<EdgeCollider2D>();
                        _edgeCollider2D[i].offset = -_sprite.pivot / _sprite.pixelsPerUnit;
                    }
                }

                _verticeGenerator.SetSmoothness(Tolerance);
                for (var j = 0; j < _verticeGenerator.VerticePaths.Count; j++)
                {
                    var verticePath = new List<Vector2>(_verticeGenerator.VerticePaths[j]);

                    var points = new Vector2[verticePath.Count];

                    for (var i = 0; i < verticePath.Count; i++)
                    {
                        points[i] = verticePath[i];
                    }

                    _edgeCollider2D[j].points = points;
                }
            }
            catch (Exception e)
            {
                print("You should Reset the component");
            }
        }

        private static void DestroyAtEndOfFrame(Object destroyObject)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(destroyObject);
            };
        }
#endif
        
    }
}

