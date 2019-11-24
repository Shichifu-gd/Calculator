using UnityEngine.UI;
using UnityEngine;
using System;

public class ButtonPress : MonoBehaviour
{
    ManagerCalculator managerCalculator;

    public Text Label;

    bool Switch;

    private void Awake()
    {
        managerCalculator = GameObject.FindGameObjectWithTag("managerCalculator").GetComponent<ManagerCalculator>();
    }

    public void TappedButton()
    {
        float Number;
        if (float.TryParse(Label.text, out Number)) managerCalculator.AcceptsNumbers(Label.text);
        else
        {
            Switch = true;
            if (Label.text != "C" && Label.text != "=" && Label.text != "," && Label.text != "±")
            {
                Switch = false;
                managerCalculator.AcceptsOperators(Label.text);
            }
            if (Label.text == "=")
            {
                Switch = false;
                managerCalculator.SumNumbers();
            }
            if (Label.text == "±")
            {
                Switch = false;
                managerCalculator.ReturnsTheOppositeValue();
            }
            if (Label.text == ",")
            {
                Switch = false;
                managerCalculator.FloatingPoint();
            }
            if (Label.text == "C")
            {
                Switch = false;
                managerCalculator.FullCleaning();
            }
        }
    }
}