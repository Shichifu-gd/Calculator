using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ManagerCalculator : MonoBehaviour
{
    [SerializeField] Text HistoryText;
    [SerializeField] Text CurrentOperator;
    [SerializeField] Text CurrentNumbers;
    [SerializeField] Text FinalResult;

    int OperatorNumberOutOfBrackets;
    int MinOperatorNumberOutOfBrackets;
    int MedOperatorNumberOutOfBrackets;
    int NumberOfOperations;
    int GlobalIndex;
    int localIndex;
    int StartPoint;
    int EndPoint;

    float Answer;
    float NumberA;
    float NumberB;

    string Operator;
    string InputNumber;

    bool SwitchingForFloat;
    bool SwitchForOperator;
    bool PushButtonOption;
    bool Switch = false;

    public List<string> DecisionHistory;
    public List<string> OrderedValues;

    private void Awake()
    {
        FullCleaning();
    }

    public void LaunchesTaskSolvingProcesses(List<string> listReceived)
    {
        OrderedValues = listReceived;
        DistributeTheOrderOfDecision(1);
        DistributeTheOrderOfDecision(2);
        DistributeTheOrderOfDecision(3);
        DistributeTheOrderOfDecision(4);
        ResetOperatorNumber("полный сброс");
    }

    void DistributeTheOrderOfDecision(int decisionProcess)
    {
        for (int index = 0; index < OrderedValues.Count; index++)
        {
            if (decisionProcess == 1) MaximumPriority(index);
            if (decisionProcess == 2) MediumPriority("вне скобок", index);
            if (decisionProcess == 3) MinimumPriority("вне скобок", index);
        }
        if (decisionProcess == 4) SolveTheExampleInParentheses();
    }

    void MaximumPriority(int index)
    {
        if (OrderedValues[index].Contains("открытая скобка"))
        {
            OrderedValues[index] += "(" + OperatorNumberOutOfBrackets + ")";
            for (int indexA = index; indexA < OrderedValues.Count; indexA++)
            {
                MediumPriority("в скобке", indexA);
                MinimumPriority("в скобке", indexA);
                if (OrderedValues[indexA].Contains("закрытая скобка"))
                {
                    ResetOperatorNumber("сброс поумолчание");
                    OrderedValues[indexA] += "(" + OperatorNumberOutOfBrackets + ")";
                    indexA = OrderedValues.Count;
                    OperatorNumberOutOfBrackets++;
                }
            }
        }
    }

    void MediumPriority(string part, int index)
    {
        if (OrderedValues[index] == "*" || OrderedValues[index] == "/")
        {
            if (part == "вне скобок")
            {
                OrderedValues[index] += "(" + OperatorNumberOutOfBrackets + ")";
                OperatorNumberOutOfBrackets++;
            }
            else
            {
                OrderedValues[index] += "(" + MedOperatorNumberOutOfBrackets + ")";
                MedOperatorNumberOutOfBrackets++;
            }
        }
    }

    void MinimumPriority(string part, int index)
    {
        if (OrderedValues[index] == "+" || OrderedValues[index] == "-")
        {
            if (part == "вне скобок")
            {
                OrderedValues[index] += "(" + OperatorNumberOutOfBrackets + ")";
                OperatorNumberOutOfBrackets++;
            }
            else
            {
                OrderedValues[index] += "(" + MinOperatorNumberOutOfBrackets + ")";
                MinOperatorNumberOutOfBrackets++;
            }
        }
    }

    void ResetOperatorNumber(string part)
    {
        if (part == "полный сброс")
        {
            OperatorNumberOutOfBrackets = 0;
            MedOperatorNumberOutOfBrackets = 0;
            MinOperatorNumberOutOfBrackets = 0;
        }
        else
        {
            MedOperatorNumberOutOfBrackets = 0;
            MinOperatorNumberOutOfBrackets = 0;
        }
    }

    void SolveTheExampleInParentheses()
    {
        MaxiumPriorityFraction();
        for (GlobalIndex = 0; GlobalIndex < OrderedValues.Count; GlobalIndex++)
        {
            for (int a = 0; a < 20; a++)
            {
                if (OrderedValues[GlobalIndex].Contains("открытая скобка" + "(" + a))
                {
                    bool limiter = true;
                    StartPoint = GlobalIndex;
                    for (localIndex = GlobalIndex; limiter == true; localIndex++)
                    {
                        if (OrderedValues[localIndex].Contains("закрытая скобка" + "(" + a))
                        {
                            limiter = false;
                            EndPoint = localIndex;
                            MediumPrioritySolution(StartPoint);
                            MinimumPrioritySolution(StartPoint);
                        }
                    }
                }
            }
        }
        SolutionOutOfBrackets();
    }

    void MaxiumPriorityFraction()
    {
        float numA;
        float numB;
        float numC;
        for (int index = 0; index < OrderedValues.Count; index++)
        {
            if (OrderedValues[index].Contains("|"))
            {
                string test = OrderedValues[index];
                for (int a = 0; a < test.Length; a++)
                {
                    if (test[a].ToString() == "|")
                    {
                        numA = float.Parse(test.Substring(0, a));
                        numB = float.Parse(test.Substring(a + 1, test.Length - 1 - a));
                        numC = numA / numB;
                        OrderedValues[index] = numC.ToString();
                    }
                }
            }
        }
    }

    void SolutionOutOfBrackets()
    {
        for (int index = 0; index < OrderedValues.Count; index++)
        {
            if (OrderedValues[index].Contains("открытая скобка")) OrderedValues[index] = "";
            if (OrderedValues[index].Contains("закрытая скобка")) OrderedValues[index] = "";
        }
        CleanList();
        Switch = true;
        EndPoint = OrderedValues.Count - 1;
        MediumPrioritySolution(0);
        MinimumPrioritySolution(0);
        FinalResult.text = OrderedValues[0];
    }

    void MediumPrioritySolution(int start)
    {
        for (int localIndex = start; localIndex < EndPoint; localIndex++)
        {
            for (int a = 0; a < 20; a++)
            {
                if (OrderedValues.Count > 1 && localIndex < EndPoint)
                {
                    if (OrderedValues[localIndex].Contains("*" + "(" + a + ")") || OrderedValues[localIndex].Contains("/" + "(" + a + ")"))
                    {
                        WorkWithAList(localIndex, "Medium");
                    }
                }
            }
        }
    }

    void MinimumPrioritySolution(int start)
    {
        for (int localIndex = start; localIndex < EndPoint; localIndex++)
        {
            for (int a = 0; a < 20; a++)
            {
                if (OrderedValues.Count > 1 && localIndex < EndPoint)
                {
                    if (OrderedValues[localIndex].Contains("+" + "(" + a + ")") || OrderedValues[localIndex].Contains("-" + "(" + a + ")"))
                    {
                        WorkWithAList(localIndex, "Minimum");
                    }
                }
            }
        }
    }

    void WorkWithAList(int index, string priority)
    {
        NumberA = float.Parse(OrderedValues[index - 1]);
        Operator = OrderedValues[index].Substring(0, 1);
        NumberB = float.Parse(OrderedValues[index + 1]);
        ExampleSolution();
        OrderedValues[index] = Answer.ToString();
        OrderedValues[index - 1] = "";
        OrderedValues[index + 1] = "";
        CleanList();
        if (Switch == false) EndPoint -= 2;
        else EndPoint = OrderedValues.Count - 1;
    }

    void CleanList()
    {
        OrderedValues.RemoveAll(str => string.IsNullOrEmpty(str));
    }

    public void AcceptsNumbers(string acceptNumber)
    {
        PushButtonOption = true;
        InputNumber += acceptNumber;
        CurrentNumbers.text = "";
        CurrentNumbers.text += InputNumber;
        SwitchForOperator = false;
    }

    public void FloatingPoint()
    {
        if (SwitchingForFloat == false) InputNumber += ".";
        SwitchingForFloat = true;
        CurrentNumbers.text = "";
        CurrentNumbers.text += InputNumber;
    }

    public void AcceptsOperators(string acceptOperator)
    {
        NumberB = float.Parse(InputNumber);
        Operator = acceptOperator;
        InputiStory();
        CurrentOperator.text = Operator;
        OperatorCheck();
        InputNumber = "";
        SwitchingForFloat = false;
        SwitchForOperator = true;
    }

    void InputiStory()
    {
        if (SwitchForOperator == false)
        {
            DecisionHistory.Add(InputNumber);
            DecisionHistory.Add(Operator);
            AddToStory();
        }
        else
        {
            DecisionHistory[DecisionHistory.Count - 1] = Operator;
            HistoryText.text = "";
            for (int index = 0; index < DecisionHistory.Count; index++) HistoryText.text += DecisionHistory[index];
        }
    }

    public void SumNumbers()
    {
        CurrentNumbers.text = "";
        NumberB = float.Parse(InputNumber);
        OperatorCheck();
        Operator = "=";
        CurrentOperator.text = Operator;
        AddToStory();
        InputNumber = "";
    }

    public void AddStory(string inputText)
    {
        HistoryText.text = inputText;
    }

    void AddToStory()
    {
        if (Operator != "=") HistoryText.text += InputNumber + " " + Operator + " ";
        else HistoryText.text += InputNumber + " " + Operator + " " + Answer;
        CurrentNumbers.text = "";
    }

    void OperatorCheck()
    {
        if (NumberOfOperations < 1 && PushButtonOption == true) NumberA = NumberB;
        else
        {
            ExampleSolution();
            FinalResult.text = Answer.ToString();
        }
        NumberOfOperations++;
    }

    void ExampleSolution()
    {
        switch (Operator)
        {
            case "+":
                Answer = NumberA + NumberB;
                break;
            case "-":
                Answer = NumberA - NumberB;
                break;
            case "/":
                Answer = NumberA / NumberB;
                break;
            case "*":
                Answer = NumberA * NumberB;
                break;
        }
    }

    public void ReturnsTheOppositeValue()
    {
        float number = float.Parse(InputNumber);
        if (number > 0)
        {
            number = -Mathf.Abs(number);
            InputNumber = number.ToString();
            CurrentNumbers.text = InputNumber;
        }
        else
        {
            number = Mathf.Abs(number);
            InputNumber = number.ToString();
            CurrentNumbers.text = InputNumber;
        }
    }

    public void FullCleaning()
    {
        Answer = 0;
        NumberA = 0;
        NumberB = 0;
        NumberOfOperations = 0;
        InputNumber = "";
        CurrentNumbers.text = "";
        CurrentOperator.text = "";
        HistoryText.text = "";
        FinalResult.text = "";
        DecisionHistory.Clear();
        OrderedValues.Clear();
        PushButtonOption = false;
        Switch = false;
    }

    public void ErrorOutput(string error)
    {
        CurrentNumbers.text = error;
        CurrentNumbers.resizeTextMaxSize = 25;
        Invoke("ClearErrorDisplay", 4f);
    }

    void ClearErrorDisplay()
    {
        CurrentNumbers.text = "";
    }
}