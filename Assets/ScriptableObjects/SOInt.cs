using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SOInt : ScriptableObject
{
    private int _highScore;

    public int Score
    {
        get { return _highScore; }
        set { _highScore = value;  }
    }
}
