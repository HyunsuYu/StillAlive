using UnityEngine;

public class MathUtility
{
    public static int CalculateValueByDay(float _day)
    { 
        float y = 580f / ((_day * _day) + 1f);        
        return Mathf.FloorToInt(y);
    }

}
