using System;
using System.Collections.Generic;
using UnityEngine;

public class StyleComboManager : MonoBehaviour
{
    #region Variables
    [Header("Combo Variables")]
    [SerializeField] private float comboScore;
    [SerializeField] private int rankIndex = 1;
    [SerializeField] private string currentRankName;

    [Header("Dictionaries")]
    //Key: Rank Index. Value: <Multiplier, Decay>
    private Dictionary<int,Tuple<float,float>> multiplierDict;
    //Key: Rank Index. Value: <Rank name, Points required>
    private Dictionary<int,Tuple<string, float>> rankDict;

    #endregion
    void Awake()
    {
        // Initialize dictionaries
        multiplierDict = new Dictionary<int, Tuple<float, float>>();
        rankDict = new Dictionary<int, Tuple<string, float>>();

        //Rank 1
        multiplierDict.Add(1, new Tuple<float, float>(1f, 0f));
        rankDict.Add(1, new Tuple<string, float>("1",0));

        //Rank 2
        multiplierDict.Add(2, new Tuple<float, float>(1.2f, 1.2f));
        rankDict.Add(2, new Tuple<string, float>("2",10));

        //Rank 3
        multiplierDict.Add(3, new Tuple<float,float>(1.5f,1.2f));
        rankDict.Add(3, new Tuple<string,float>("3",30));

        //Rank 4
        multiplierDict.Add(4, new Tuple<float,float>(2,1.5f));
        rankDict.Add(4, new Tuple<string, float>("4",50));
    }
    void Start()
    {
        currentRankName = rankDict[rankIndex].Item1;
    }
    void Update()
    {
        Decay();
        CheckRank();
    }

    public void AddScore(float points)
    {
        comboScore += points * multiplierDict[rankIndex].Item1;
    }
    private void Decay()
    {
        float scoreDecrement = Time.deltaTime;
        scoreDecrement *= multiplierDict[rankIndex].Item2;
        comboScore -= scoreDecrement;
    }
    private void CheckRank()
    {
        if (rankIndex + 1 >= multiplierDict.Count || rankIndex + 1 >= rankDict.Count)
            return;
            
        if (comboScore >= rankDict[rankIndex+1].Item2)
        {
            currentRankName = rankDict[rankIndex+1].Item1;
            rankIndex += 1;
        }
        else if (comboScore <= rankDict[rankIndex - 1].Item2)
        {
            currentRankName = rankDict[rankIndex-1].Item1;
            rankIndex -= 1;
        }
    }

    #region Getters and Setters
    public float GetComboScore()
    {
        return comboScore;
    }
    public int GetRankIndex()
    {
        return rankIndex;
    }
    public string GetCurrentRankName()
    {
        return currentRankName;
    }
    #endregion
}
