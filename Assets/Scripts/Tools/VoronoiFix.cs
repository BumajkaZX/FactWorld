using System;
using UnityEngine;



namespace FactWorld.Tools
{
    public class VoronoiFix : MonoBehaviour
    {
        #region params
        [SerializeField]
        int _seed, _dim;
        [SerializeField]
        float _lColor;
        [SerializeField]
        IslandGen _isGen;
        [SerializeField]
        bool _InvertColor, _Divide;
        #endregion
        [ContextMenu("Generate Texture")]
        void Create()
        {
            _seed = _isGen.GetSeed();
            _dim = _isGen.GetDim();
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(NewVoronoi(_Divide), new Rect(0, 0, _dim, _dim), Vector2.one * 0.5f);
        }

        public Texture2D NewVoronoi(bool div)
        {
            _seed = _isGen.GetSeed();
            _dim = _isGen.GetDim();
            Texture2D vor = new Texture2D(_dim, _dim);
            var numbers = GetNumbers(_seed);  // 11111 min 1000000 max
            int regions = Int32.Parse(numbers[0].ToString() + numbers[1].ToString()) / 3;
            if (regions < 15)
            {
                regions = 15;
            }
            if (div)
            {
                regions = 6;
            }
            Vector2Int[] centroids = new Vector2Int[regions];
            for (int i = 0; i < regions; i++)
            {
                centroids[i].x = Mathf.RoundToInt(Mathf.LerpUnclamped(0, _dim - 1, GetRandomSeedNumber(_seed * (i + 1), true)) * numbers[2]); ;
                centroids[i].y = Mathf.RoundToInt(Mathf.LerpUnclamped(0, _dim - 1, GetRandomSeedNumber(_seed * (i + 1), false)) * numbers[3]);
                vor.SetPixel(centroids[i].x, centroids[i].y, Color.black);

            }
            var ind = 0;
            for (int v = 0; v < _dim; v++)
            {
                for (int c = 0; c < _dim; c++)
                {
                    if (vor.GetPixel(v, c) == Color.black)
                    {
                        centroids[ind] = new Vector2Int(v, c);
                        ind++;
                    }
                }
            }
            Color[] pixelColors = new Color[_dim * _dim];
            float[] distances = new float[_dim * _dim];

            float maxDst = float.MinValue;
            for (int x = 0; x < _dim; x++)
            {
                for (int y = 0; y < _dim; y++)
                {
                    int index = x * _dim + y;
                    distances[index] = Vector2.Distance(new Vector2Int(x, y), centroids[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)]);
                    if (distances[index] > maxDst)
                    {
                        maxDst = distances[index];
                    }
                }
            }

            for (int i = 0; i < distances.Length; i++)
            {
                float colorValue = _InvertColor ? Mathf.Abs(1 - (distances[i] / maxDst)) : distances[i] / maxDst;
                pixelColors[i] = new Color(colorValue, colorValue, colorValue, 1f);
            }



            Texture2D tex = new Texture2D(_dim, _dim);
            tex.filterMode = FilterMode.Point;
            tex.SetPixels(pixelColors);
            for (int i = 0; i < _dim; i++)
            {
                for (int v = 0; v < _dim; v++)
                {
                    if (tex.GetPixel(i, v).r > _lColor)
                    {
                        tex.SetPixel(i, v, Color.black);
                    }
                }
            }
            tex.Apply();
            return tex;
        }
        int[] GetNumbers(int seed)
        {
            var i = 0;
            var b = seed;
            while (b / 10 != 0)
            {
                b /= 10;
                i++;
            }
            i++;
            int[] num = new int[i];
            b = seed;
            var x = 0;
            for (int v = 0; v < i; v++)
            {
                if (b % 10 != 0)
                {
                    num[x] = b % 10;

                    x++;
                }
                b /= 10;
            }
            return num;
        }
        float GetRandomSeedNumber(int seed, bool w)
        {
            float v;
            var b = GetNumbers(seed);
            v = float.Parse(b[0].ToString() + b[1].ToString());
            v = w ? Mathf.Sin(v) : (float)Math.Cos(v);
            return v;
        }
        int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
        {
            float smallestDst = float.MaxValue;
            int index = 0;
            for (int i = 0; i < centroids.Length; i++)
            {
                if (Vector2.Distance(pixelPos, centroids[i]) < smallestDst)
                {
                    smallestDst = Vector2.Distance(pixelPos, centroids[i]);
                    index = i;
                }
            }
            return index;
        }


    }
}