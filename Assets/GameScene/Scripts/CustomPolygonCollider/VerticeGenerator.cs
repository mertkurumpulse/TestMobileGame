using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomPolygonCollider
{
    public class VerticeGenerator
    {

        private readonly List<List<Vector2>> _verticePaths;
        public List<List<Vector2>> VerticePaths;

        private readonly bool[,] _isAlreadyAdded;

        private readonly Sprite _sprite;
        private readonly Color32[,] _color32S;
        
        [Flags]
        private enum OnEdgeType
        {
            NotOnEdge = 0,
            Bottom = 1,
            Top = 2,
            Left = 4,
            Right = 8
        }

        private OnEdgeType[,] _onEdgeTypes;
        
        public VerticeGenerator(Sprite sprite)
        {
            _sprite = sprite;
            var texture2D = _sprite.texture;
            _color32S = ConvertTo2Dimension(texture2D.GetPixels32(), texture2D.width, texture2D.height);
            _isAlreadyAdded = new bool[_color32S.GetLength(0), _color32S.GetLength(1)];

            _verticePaths = new List<List<Vector2>>();
        }

        public void GenerateVertices()
        {
            
            for (var i = 0; i < _color32S.GetLength(0); i++)
            {
                for (var j = 0; j < _color32S.GetLength(1); j++)
                {
                    if (_isAlreadyAdded[i, j] || _color32S[i, j].a <= 15 || IsOnEdge(i, j) == OnEdgeType.NotOnEdge) continue;
                    var path = new List<Vector2>();
                    _verticePaths.Add(path);

                    GeneratePath(i, j, path);
                }
            }

            VerticePaths = new List<List<Vector2>>(_verticePaths);
        }

        private void GeneratePath(int i, int j, List<Vector2> path)
        {   
            path.Add(new Vector2(j / _sprite.pixelsPerUnit, i / _sprite.pixelsPerUnit));
            _isAlreadyAdded[i, j] = true;

            var neighbours = GetNeighbours(i, j, true);
            
            int[] nextNeighbour = null;
            foreach (var neighbourCoord in neighbours)
            {
                if (neighbourCoord == null || _isAlreadyAdded[neighbourCoord[0], neighbourCoord[1]] ||
                    IsOnEdge(neighbourCoord[0], neighbourCoord[1]) == OnEdgeType.NotOnEdge) continue;

                nextNeighbour = neighbourCoord;
                if ((IsOnEdge(i,j) & IsOnEdge(neighbourCoord[0], neighbourCoord[1])) == IsOnEdge(i,j))
                {
                    break;
                }
                
            }
            if (nextNeighbour != null)
            {
                GeneratePath(nextNeighbour[0], nextNeighbour[1], path);
            }
        }

        private OnEdgeType IsOnEdge(int x, int y)
        {
            var onEdgeType = OnEdgeType.NotOnEdge;
            
            if (_color32S[x, y].a < 15) return onEdgeType;

            if (x == 0 || _color32S[x-1, y].a < 15)
            {
                onEdgeType = onEdgeType | OnEdgeType.Bottom;
            }
            if (x == _color32S.GetLength(0)-1 || _color32S[x+1, y].a < 15)
            {
                onEdgeType = onEdgeType | OnEdgeType.Top;
            }
            if (y == 0 || _color32S[x, y-1].a < 15)
            {
                onEdgeType = onEdgeType | OnEdgeType.Left;
            }
            if (y == _color32S.GetLength(1)-1 || _color32S[x, y+1].a < 15)
            {
                onEdgeType = onEdgeType | OnEdgeType.Right;
            }

            return onEdgeType;

        }

        private List<int[]> GetNeighbours(int x, int y, bool getDiagonal)
        {
            var neighbours = new List<int[]>();

            for (var i = 0; i < 9; i++)
            {
                if (i == 4) continue;

                if (!getDiagonal && (i == 0 || i == 2 || i == 6 || i == 8))
                {
                    neighbours.Add(null);
                    continue;
                }
                
                int[] neighbourCoords = {x - 1 + i / 3, y - 1 + i % 3};
                
                if (neighbourCoords[0] >= 0 && neighbourCoords[0] < _color32S.GetLength(0) &&
                    neighbourCoords[1] >= 0 && neighbourCoords[1] < _color32S.GetLength(1))
                {
                    neighbours.Add(neighbourCoords);
                }
                else
                {
                    neighbours.Add(null);
                }
            }

            return neighbours;
        }

        public void SetSmoothness(float smoothness)
        {
            smoothness = smoothness * 100 / _sprite.pixelsPerUnit;

            VerticePaths = new List<List<Vector2>>();
            foreach (var verticePath in _verticePaths)
            {
                VerticePaths.Add(verticePath.ToList());
            }

            foreach (var path in VerticePaths)
            {
                //path.Add(path[0]);
                
                for (var i = 0; i < path.Count - 3; i++)
                {
                    var prevVector = path[i + 1] - path[i];
                    var currentVector = path[i + 2] - path[i + 1];
                    var nextVector = path[i + 3] - path[i + 2];

                    var crossCurNext = Vector3.Cross(prevVector, currentVector);
                    var crossNextThird = Vector3.Cross(currentVector, nextVector);

                    if (Mathf.Abs(crossCurNext.z) < Mathf.Epsilon)
                    {
                        path.RemoveAt(i + 1);
                        i--;
                        continue;
                    }
                    if (Mathf.Abs(crossNextThird.z) < Mathf.Epsilon)
                    {
                        path.RemoveAt(i + 2);
                        i--;
                        continue;
                    }
                    

                    if ((Mathf.Abs(crossCurNext.z) > 0 || Mathf.Abs(crossNextThird.z) > 0) &&
                        (int) Mathf.Sign(crossCurNext.z) == (int) Mathf.Sign(crossNextThird.z))
                    {
                        continue;
                    }

                    if (prevVector.sqrMagnitude > smoothness * smoothness) continue;

                    path.RemoveAt(i + 1);
                    i--;
                }
            }
        }

        private static T[,] ConvertTo2Dimension<T>(T[] array, int width, int height)
        {
            var array2Dim = new T[height, width];
            var column = 0;
            var row = height - 1;
            for (var i = 0; i < array.GetLength(0); i++)
            {
                array2Dim[row, column] = array[i];
                column++;
                if (column != width) continue;
                column = 0;
                row--;
            }

            return MirrorAxisY(array2Dim);
        }

        private static T[,] MirrorAxisY<T>(T[,] color32)
        {
            var axisArray = new T[color32.GetLength(0), color32.GetLength(1)];

            for (var i = 0; i < axisArray.GetLength(0); i++)
            {
                for (var j = 0; j < axisArray.GetLength(1); j++)
                {
                    axisArray[i, j] = color32[color32.GetLength(0) - i - 1, j];
                }
            }

            return axisArray;
        }
    }
}