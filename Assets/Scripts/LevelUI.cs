using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class LevelUI : MonoBehaviour
{
    public UnityAction<int> onClick;

    [SerializeField] private TextMeshProUGUI levelNumber;

    public void SetNumber(int num) {
        levelNumber.text = num.ToString();
    }

    public void ClickButton() {
        onClick?.Invoke(int.Parse(levelNumber.text));
    }
}
