using UnityEngine;



namespace FactWorld.Tools
{
    public class Gradient : MonoBehaviour
    {
        #region params
        [SerializeField]
        int _dim, _mainLayer, _rad, _seed;
        [SerializeField]
        float _brightness, _noiseScale;
        [SerializeField]
        IslandGen _isGen;
        [SerializeField]
        bool _InvertColor;
        #endregion
        [ContextMenu("Generate Texture")]
        void Create()
        {
            _dim = _isGen.GetDim() + 2;
            _rad = _isGen.GetRad();
            _seed = _isGen.GetSeed();
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(NewGradient(), new Rect(0, 0, _dim, _dim), Vector2.one * 0.5f);
        }

        public Texture2D NewGradient()
        {
            _dim = _isGen.GetDim() + 2;
            _rad = _isGen.GetRad();
            _seed = _isGen.GetSeed();
            Texture2D grad = new Texture2D(_dim, _dim);
            for (int v = 0; v < _dim; v++)
            {
                for (int c = 0; c < _dim; c++)
                {
                    grad.SetPixel(v, c, Color.black);
                }
            }
            int centre = (_dim - 1) / 2;
            int layers = centre + 1;

            float colorG = _InvertColor ? 0 : 1;
            Color gr = new Color(colorG, colorG, colorG);
            float step = 1 / ((((float)layers - (float)_mainLayer)) / _brightness);
            grad.SetPixel(centre, centre, Color.black);
            for (int i = _mainLayer, k = _mainLayer; i < layers; i++, k++)
            {
                int def = k;
                k++;
                for (int j = 0; j < (i * 2) + 1; j++)
                {

                    k--;
                    grad.SetPixel(centre + k, centre - i, gr);


                }

                for (int v = i - 1; v >= 0; v--)
                {
                    grad.SetPixel(centre - k, centre - v, gr);
                    grad.SetPixel(centre + k, centre - v, gr);
                }

                for (int b = 1; b <= i - 1; b++)
                {
                    grad.SetPixel(centre - k - b, centre + b, gr);
                    grad.SetPixel(centre + k + b, centre + b, gr);
                }



                grad.SetPixel(centre, centre + i, gr);

                k = def;
                colorG = _InvertColor ? colorG + step : colorG - step;
                gr = new Color(colorG, colorG, colorG);
            }
            for (int y = 0; y < _dim; y++)
            {
                for (int x = 0; x < _dim; x++)
                {
                    float xCoord = (float)x / _rad * _noiseScale + _seed;
                    float yCoord = (float)y / _rad * _noiseScale + _seed;
                    float PerlinScale = Mathf.PerlinNoise(xCoord, yCoord);
                    float scaleGrad = grad.GetPixel(x, y).r;
                    float scale = PerlinScale * scaleGrad;
                    grad.SetPixel(x, y, new Color(scale, scale, scale));
                }
            }
            grad.Apply();
            grad.filterMode = FilterMode.Point;
            return grad;
        }


    }
}
