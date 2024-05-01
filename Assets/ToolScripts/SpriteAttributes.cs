using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class MyAttributes
{   
    public static void FitHeight (this GameObject gameObject, float height)
    {
        if (gameObject.GetComponent<SpriteRenderer>() != null)
        {
            gameObject.transform.localScale = new Vector3(height/gameObject.GetHeightPixel() * gameObject.transform.localScale.x, height/gameObject.GetHeightPixel()*gameObject.transform.localScale.y,gameObject.transform.localScale.z);
        }
    }

    public static void FitWidth (this GameObject gameObject, float width)
    {
        if (gameObject.GetComponent<SpriteRenderer>() != null)
        {
            gameObject.transform.localScale = new Vector3(width/gameObject.GetWidthPixel()*gameObject.transform.localScale.x, width/gameObject.GetWidthPixel()*gameObject.transform.localScale.y,gameObject.transform.localScale.z);
        }
    }

    public static void FitWidthAndHeight(this GameObject gameObject, float width, float height)
    {
        if (gameObject.GetComponent<SpriteRenderer>() != null)
        {
            gameObject.FitWidth(width);
            gameObject.FitHeight(height);
        }
    }

    public static float GetWidthPixel(this GameObject gameObject)
    {
        if (gameObject.GetComponent<SpriteRenderer>() == null) return 0;

        var sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        return sprite.texture.width * gameObject.transform.lossyScale.x;
    }

    public static float GetHeightPixel(this GameObject gameObject)
    {
        if (gameObject.GetComponent<SpriteRenderer>() == null) return 0;

        var sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        return sprite.texture.height * gameObject.transform.lossyScale.y;
    }

    public static float GetWidthUnit(this GameObject gameObject)
    {
        if (gameObject.GetComponent<SpriteRenderer>() == null) return 0;

        var sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        return (sprite.texture.width / sprite.pixelsPerUnit) * gameObject.transform.lossyScale.x;
    }

    public static float GetHeightUnit(this GameObject gameObject)
    {
        if (gameObject.GetComponent<SpriteRenderer>() == null) return 0;

        var sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        return (sprite.texture.height / sprite.pixelsPerUnit) * gameObject.transform.lossyScale.y;
    }

    public static Rect GetBoundingBox(this GameObject gameObject)
    {
        if (gameObject.GetComponent<SpriteRenderer>() == null) return Rect.zero;

        var offset = gameObject.GetComponent<SpriteRenderer>().sprite.pivot;

        offset = new Vector2(offset.x / gameObject.GetWidthPixel(), offset.y / gameObject.GetHeightPixel());

        var widthUnit = gameObject.GetWidthUnit();
        var heightUnit = gameObject.GetHeightUnit();
        
        var minX = gameObject.transform.position.x - offset.x * widthUnit;
        var minY = gameObject.transform.position.y - offset.y * heightUnit;
        
        return new Rect(minX, minY, widthUnit, heightUnit);
    }
    
    public static Rect GetBoundingBoxOf2DObject(this GameObject gameObject2D)
    {
        var boundingBox = gameObject2D.GetBoundingBox();
		
        var minX = boundingBox.min.x;
        var minY = boundingBox.min.y;
        var maxX = boundingBox.max.x;
        var maxY = boundingBox.max.y;
		
        if (boundingBox.Equals(Rect.zero))
        {
            minX = minY = 1000000;
            maxX = maxY = -1000000;
        }
		
        for (var i = 0; i < gameObject2D.transform.childCount; i++)
        {
            var childRect = GetBoundingBoxOf2DObject(gameObject2D.transform.GetChild(i).gameObject);

            if (childRect.Equals(Rect.zero))
            {
                continue;
            }
			
            if (childRect.xMax > maxX)
            {
                maxX = childRect.max.x;
            }
            if (childRect.yMax > maxY)
            {
                maxY = childRect.yMax;
            }
            if (childRect.xMin < minX)
            {
                minX = childRect.xMin;
            }
            if (childRect.yMin < minY)
            {
                minY = childRect.yMin;
            }
        }
		
        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    public static Rect GetBoundingBoxOfUiCanvas(this GameObject canvas)
    {
        var boundingBox = canvas.GetComponent<Image>() == null ? new Rect(0,0,0,0) : canvas.GetComponent<Image>().rectTransform.rect;

        var minX = boundingBox.min.x;
        var minY = boundingBox.min.y;
        var maxX = boundingBox.max.x;
        var maxY = boundingBox.max.y;
		
        if (boundingBox.Equals(Rect.zero))
        {
            minX = minY = 1000000;
            maxX = maxY = -1000000;
        }
        
        for (var i = 0; i < canvas.transform.childCount; i++)
        {
            if (canvas.transform.GetChild(i).GetComponent<Image>())
            {
                
                var childRect = GetBoundingBoxOfUiCanvas(canvas.transform.GetChild(i).gameObject);

                if (childRect.Equals(Rect.zero))
                {
                    continue;
                }
			
                if (childRect.xMax > maxX)
                {
                    maxX = childRect.max.x;
                }
                if (childRect.yMax > maxY)
                {
                    maxY = childRect.yMax;
                }
                if (childRect.xMin < minX)
                {
                    minX = childRect.xMin;
                }
                if (childRect.yMin < minY)
                {
                    minY = childRect.yMin;
                }
            }
        }

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    public static List<int> Shuffle<T>(this IList<T> list)
    {
        var newLocation = new List<int> ();

        int n = list.Count;

        for (int i = 0; i < list.Count; i++)
        {
            newLocation.Add (i);
        }

        while (n > 1)
        {
            n--;
            int k = Random.Range(0,n+1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;

            int tempValue = newLocation [n];
            newLocation [n] = newLocation[k];
            newLocation [k] = tempValue;
        }

        return newLocation;
    }

    public static void ShuffleWithGivenArray<T> (this IList<T> list, List<int> shuffleList)
    {
        var tempList = new List<T> (list);

        for (int i = 0; i < list.Count; i++)
        {
            list [i] = tempList [shuffleList [i]];
        }
    }
}
