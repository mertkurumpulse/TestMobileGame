using System.Collections;
using System.Collections.Generic;
using BansheeGz.BGSpline.Components;
using UnityEngine;
using BansheeGz.BGSpline.Curve;

public class Collectibles : MonoBehaviour
{
    public BGCcMath[] CurveMath;

    private List<Vector2> _path1, _path2;

    public GameObject Coin;

    private PoolManager _coinPoolManager;

    private int _numberOfTotalCoins = 5;

    private List<GameObject> _coins;

    private void Start()
    {
        _coinPoolManager = new PoolManager();
        _coinPoolManager.SetUnits(new List<GameObject>{Coin});
        
        _coins = new List<GameObject>();
        
        _path1 = new List<Vector2>();
        _path2 = new List<Vector2>();
        
        CreatePathBetweenDistance(_path1, CurveMath[0]);
        CreatePathBetweenDistance(_path2, CurveMath[1]);
    }

    public void CreateCoin()
    {
        float scale;
        var random = Random.Range(0, 3);
        if (random == 0) scale = 1 / 1.08f;
        else if (random == 1) scale = 1.15f;
        else scale = 1;

        _numberOfTotalCoins = Random.Range(4, 6);

        var path = GamePlayManager.SelectedPim.transform.position.y > 0 ? _path2 : _path1;
        
        CreateCoinOnPath(path, Random.Range(path.Count/10, path.Count - _numberOfTotalCoins), 1);
    }

    public void CollectCoin(GameObject collectedCoin)
    {
        _coinPoolManager.AddObjectToPool(collectedCoin);
        _coins.Remove(collectedCoin);
    }

    public bool CanCreateCoins()
    {
        return _coins.Count == 0;
    }


    public void RemoveAllCoins()
    {
        foreach (var coin in _coins)
        {
            _coinPoolManager.AddObjectToPool(coin);
        }
        _coins.Clear();
    }

    private void CreateCoinOnPath(List<Vector2> path, int index, float scale)
    {
        for (var i = 0; i < _numberOfTotalCoins; i++)
        {
            var coin = _coinPoolManager.GetObjectFromPool(0);

            coin.transform.position = path[index + i] * scale;

            //coin.transform.SetParent(transform);
            _coins.Add(coin);
            coin.transform.localScale = Vector3.zero;
           
            var coinScript = coin.GetComponent<CoinScript>();
            coinScript.ShowCoin(.12f * i);
        }
    }

    private IEnumerator RemoveCoinIfNotCollected(GameObject coin)
    {
        yield return new WaitForSeconds(5f);

        if (coin.gameObject.activeInHierarchy)
        {
            _coins.Remove(coin);
            _coinPoolManager.AddObjectToPool(coin);
        }
    }
    
    private void CreatePathBetweenDistance(ICollection<Vector2> path, BGCcMath math)
    {
        path.Clear();
        for (var i = math.GetDistance(0); i < math.GetDistance(); i+=.5f)
        {
            path.Add(math.CalcByDistance(BGCurveBaseMath.Field.Position, i));
        }
    }
}
