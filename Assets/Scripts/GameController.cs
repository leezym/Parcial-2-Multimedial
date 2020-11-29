using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameController : MonoBehaviour
{ 
    public float sliderLifePlayer, sliderDamage;
    public int countLife;
    public Image[] lifes;
    public GameObject objectPool;

    public GameController(float valueSliderPlayer, float valueSliderDamage, GameObject objectPoolEnemys, int valueCountLife)
    {
        sliderLifePlayer = valueSliderPlayer;
        sliderDamage = valueSliderDamage;
        objectPool = objectPoolEnemys;
        countLife = valueCountLife;
    }    
}
