using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FactWorld;

namespace FactWorld.Tools
{

    public class IslandGen : MonoBehaviour
    {
        #region Constants
        const float _CHeightScale = 100;
        #endregion
        #region Params
        [SerializeField] GameObject _Hex, _ActiveHex, _MainHex, _MainActiveHex, _MainPlaces;
        [SerializeField] int _HexRad;
        [SerializeField, Range(11111, 999999)] int _seed;
        [SerializeField] int _id;
        [SerializeField] float _height, _noiseScale, _voronoiScale, _voronoi2Scale, _thickness, _activeHexDefaultOffsetY;
        [SerializeField] List<GameObject> _HexIsland = new List<GameObject>();
        [SerializeField] List<GameObject> _Places = new List<GameObject>();
        [SerializeField] List<GameObject> _CurrentPlaces = new List<GameObject>(); //Places on Scene
        [SerializeField] List<GameObject> _ActiveHexList = new List<GameObject>();
        [SerializeField, Range(0f, 100f)] List<float> _ChanceForPlaces = new List<float>();  // 1 Should be always 100
        [SerializeField] VoronoiFix _voronoi;
        [SerializeField] Gradient _grad;
        [SerializeField] MainController _mainController;
        bool allActive;
        #endregion

        List<PlacesChance> places = new List<PlacesChance>();
        [ContextMenu("Generate")]
        private void GenerateIsland()
        {
            if (_HexIsland.Count != 0) return;
            places.Clear();
            allActive = false;
            var flag = StaticEditorFlags.BatchingStatic;
            var scale = _Hex.transform.localScale.x / 100;
            var c = -_HexRad + 1;
            var x = Mathf.Sqrt(3) / 2 * scale;
            var dim = _HexRad * 2 - 1;
            Texture2D vor = _voronoi.NewVoronoi(false);
            Texture2D vor2 = _voronoi.NewVoronoi(true);
            Texture2D grad = _grad.NewGradient();
            ChanceActive(places);
            for (int v = 1; v < 2 * _HexRad; v++)
            {
                for (int i = 1; i < ((_HexRad > v - 1) ? _HexRad + v : (2 * _HexRad) - c); i++)
                {
                    var b = (v == _HexRad) ? 0 : x;
                    var _H = Instantiate(_Hex, new Vector3(v * 1.5f * scale, 0, i * x * 2 + (b * Mathf.Abs(c))), Quaternion.Euler(-90, 0, 0));
                    CalculateHeight(_H, v, i, vor, vor2, grad);
                    GameObjectUtility.SetStaticEditorFlags(_H, flag);
                    var bounds = _H.GetComponent<Renderer>().bounds;
                    var maxPoint = bounds.max.y;
                    SpawnActiveHex(_H, maxPoint, _MainActiveHex);
                    _H.transform.parent = _MainHex.transform;
                    _HexIsland.Add(_H);
                    _id++;
                }
                c++;
            }
            _mainController.AddList(_ActiveHexList);
        }

        [ContextMenu("Delete")]
        private void DeleteIsland()
        {
            for (int i = 0; i < _HexIsland.Count; i++)
            {
                DestroyImmediate(_HexIsland[i]);
                DestroyImmediate(_ActiveHexList[i]);
                DestroyImmediate(_CurrentPlaces[i]);
            }
            _HexIsland.Clear();
            _CurrentPlaces.Clear();
            _ActiveHexList.Clear();
            _mainController.ClearList();
        }
        

        [ContextMenu("Set Active")]
        private void SetActive()
        {
            for (int i = 0; i < _HexIsland.Count; i++)
            {
                _HexIsland[i].SetActive(allActive);
            }
            allActive = !allActive;
        }

        private void CalculateHeight(GameObject _h, int x, int y, Texture2D voronoi, Texture2D voronoi2, Texture2D gradient)
        {
            var voronoiScale = voronoi.GetPixel(x - 1, y - 1).r;
            var voronoi2Scale = voronoi2.GetPixel(x - 1, y - 1).r;
            var grad = gradient.GetPixel(x, y).r;
            float xCoord = (float)x / _HexRad * _noiseScale + _seed;
            float yCoord = (float)y / _HexRad * _noiseScale + _seed;
            float PerlinScale = Mathf.PerlinNoise(xCoord, yCoord);
            float _zScale = _h.transform.localScale.z + (PerlinScale * _height * _CHeightScale * (voronoiScale * _voronoiScale) * (voronoi2Scale * _voronoi2Scale) * grad) + _thickness;
            if (_zScale < 0) _zScale = 0.1f;
            _h.transform.localScale = new Vector3(_h.transform.localScale.x, _h.transform.localScale.y, _zScale);
        }

        private void SpawnPlace(GameObject _h, float maxPoint, InteractWithField par)
        {
            var flag = StaticEditorFlags.BatchingStatic;
            GameObject spawnPlace = null;
            var random = Random.Range(0f, 100f);
            var mas = places;
            for (int i = mas.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var tmp = mas[i];
                mas[i] = mas[j];
                mas[j] = tmp;
            }
            for (int i = 0; i < _Places.Count; i++)
            {
                if (mas[i].chance <= random)
                    spawnPlace = mas[i].place;
            }
            var place = Instantiate(spawnPlace, new Vector3(_h.transform.position.x, _h.transform.position.y + maxPoint, _h.transform.position.z), Quaternion.identity);
            for (int i = 0; i < place.transform.childCount; i++)
            {
                GameObjectUtility.SetStaticEditorFlags(place.transform.GetChild(i).gameObject, flag);
            }
            GameObjectUtility.SetStaticEditorFlags(place, flag);
            place.SetActive(false);
            place.transform.parent = _MainPlaces.transform;
            par.SetChild(place);
            _CurrentPlaces.Add(place);
        }

        private void SpawnActiveHex(GameObject _h, float maxPoint, GameObject par)
        {
            var ActiveHex = Instantiate(_ActiveHex, new Vector3(_h.transform.position.x, _h.transform.position.y + maxPoint + _activeHexDefaultOffsetY, _h.transform.position.z), Quaternion.Euler(-90, 0, 0));
            ActiveHex.transform.parent = par.transform;
            var cl = ActiveHex.GetComponent<InteractWithField>();
            cl.SetOffset(_activeHexDefaultOffsetY);
            cl.SetMaxPoint(maxPoint);
            cl.SetID(_id);
            _ActiveHexList.Add(ActiveHex);
            SpawnPlace(_h, maxPoint, cl);
        }

        private void ChanceActive(List<PlacesChance> placesChance)
        {
            
            for (int i = 0; i < _Places.Count; i++)
            {
                placesChance.Add(new PlacesChance());
                var chance = Mathf.Abs(100 - _ChanceForPlaces[i]);
                placesChance[i].place = _Places[i];
                placesChance[i].chance = chance;
            }
        }

        [ContextMenu("InActiveHex")]
        public void InActiveHex()
        {
            for (int i = 0; i < _ActiveHexList.Count; i++)
            {
                if (!_ActiveHexList[i].GetComponent<InteractWithField>().IsActivePosition())
                    _ActiveHexList[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }

        #region incaps
        public int GetSeed()
        {
            return _seed;
        }
        public int GetDim()
        {
            return _HexRad * 2 - 1;
        }
        public int GetRad()
        {
            return _HexRad;
        }
        public float GetNoiseScale()
        {
            return _noiseScale;
        }
        #endregion
    }

}