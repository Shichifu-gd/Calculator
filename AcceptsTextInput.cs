using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

public class AcceptsTextInput : MonoBehaviour
{
    [SerializeField] InputField InputField;
    [SerializeField] ManagerCalculator managerCalculator;

    string BufferText = "";

    int GlobalIndex;

    [SerializeField]
    bool StopTheProcessForTest = true;
    bool ProtectionPassed;
    bool SwitchFloat;

    char[] SplitInputText;
    public List<string> DistributedText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && InputField.text.Length != 0 || Input.GetKeyDown(KeyCode.KeypadEnter) && InputField.text.Length != 0) AcceptsTheInputText(InputField.text);
    }

    void AcceptsTheInputText(string userInput)
    {
        managerCalculator.FullCleaning();
        FullCleaning();
        InitialDefense(userInput);
        if (ProtectionPassed == true)
        {
            SplitInputText = userInput.ToCharArray();
            DuplicationCheck();
            for (GlobalIndex = 0; GlobalIndex < SplitInputText.Length; GlobalIndex++)
            {
                DistributeNumbers(GlobalIndex);
                CheckingForNegativeNumbers();
                if (GlobalIndex == SplitInputText.Length - 1 && BufferText != "") AddNumberToTheArray();
            }
            ParenthesisCheck();
            if (ProtectionPassed == true && StopTheProcessForTest == true)
            {
                managerCalculator.LaunchesTaskSolvingProcesses(DistributedText);
                managerCalculator.AddStory(userInput);
            }
        }        
    }

    void InitialDefense(string test)
    {
        if (test.Any(c => char.IsLetter(c))) ErrorDetected("калькулятор не поддерживает такие символы");
        else ProtectionPassed = true;
        int endIndex = test.Length - 1;
        if (test.Substring(endIndex, 1) == "+" || test.Substring(endIndex) == "-" || test.Substring(endIndex) == "/" || test.Substring(endIndex) == "*" || test.Substring(endIndex) == "|") ErrorDetected("мат. сим. не могут стоять в конце");
        else ProtectionPassed = true;
        if (test.Contains("+") || test.Contains("-") || test.Contains("*") || test.Contains("/") || test.Contains("|")) ProtectionPassed = true;
        else  ErrorDetected("не хватает символов");
        if (test.Length < 2) ErrorDetected("недостаточно символов");
    }

    void DistributeNumbers(int index)
    {
        for (int LocalIndex = index; LocalIndex < SplitInputText.Length; LocalIndex++)
        {          
            DistributeOperators(LocalIndex);
            DistributeBrackets(LocalIndex);
            for (int number = 0; number != 10; number++)
            {
                if (SplitInputText[LocalIndex].ToString() == number.ToString()) BufferText += SplitInputText[LocalIndex];
            }
            if (SplitInputText[LocalIndex].ToString() == "." && SwitchFloat == false)  BufferText += SplitInputText[LocalIndex];              
            if (SplitInputText[LocalIndex].ToString() == "|" && SwitchFloat == false)  BufferText += SplitInputText[LocalIndex];
            GlobalIndex = LocalIndex;
        }
    }   

    void DuplicationCheck()
    {
        for (int i = 0; i < SplitInputText.Length - 2; i++)
        {
            if (SplitInputText[i].ToString() == "." && SplitInputText[i + 1].ToString() == "." || 
                SplitInputText[i].ToString() == "|" && SplitInputText[i + 1].ToString() == "|")
            {
                ErrorDetected("нельзя ставить два знака одновременно");
            }            
        }
    }

    void DistributeOperators(int index)
    {
        if (SplitInputText[index].ToString() == "+" ||
            SplitInputText[index].ToString() == "-" ||
            SplitInputText[index].ToString() == "/" ||
            SplitInputText[index].ToString() == "*")
        {
            if (BufferText != "")
            {
                AddNumberToTheArray();
                SwitchFloat = false;
            }
            GlobalIndex++;
            DistributedText.Add(SplitInputText[index].ToString());
        }
    }

    void DistributeBrackets(int index)
    {
        if (index >= GlobalIndex)
        {
            if ((SplitInputText[index].ToString() == "("))
            {
                if (BufferText != "") AddNumberToTheArray();
                DistributedText.Add("открытая скобка");
            }
            if ((SplitInputText[index].ToString() == ")"))
            {
                if (BufferText != "") AddNumberToTheArray();
                DistributedText.Add("закрытая скобка");
            }
        }
    }

    void CheckingForNegativeNumbers()
    {
        for (int index = 0; index < DistributedText.Count; index++)
        {
            if (DistributedText[index].ToString() == "+" ||
                DistributedText[index].ToString() == "-" ||
                DistributedText[index].ToString() == "/" ||
                DistributedText[index].ToString() == "*")
            {
                if (index < DistributedText.Count - 1)
                {
                    if (DistributedText[index].ToString() == "+" || DistributedText[index].ToString() == "-" || DistributedText[index].ToString() == "/" || DistributedText[index].ToString() == "*") // +
                    {
                        if (DistributedText[index].ToString() != DistributedText[index + 1].ToString())
                        {
                            if (!DistributedText[index + 1].Contains("открытая скобка") || (!DistributedText[index + 1].Contains("закрытая скобка")))
                            {
                                if (index < 1 && DistributedText[index].ToString() == "-")
                                {
                                    DistributedText[index] += DistributedText[index + 1];
                                    DistributedText[index + 1] = "";
                                }
                            }
                        }
                        else ErrorDetected("нельзя ставить два знака вместе");
                    }
                }
            }
        }
        DistributedText.RemoveAll(str => string.IsNullOrEmpty(str));
    }

    void ParenthesisCheck()
    {
        for (int index = 0; index < DistributedText.Count; index++)
        {
            if (index > 0 && DistributedText[index].Contains("открытая скобка")) CheckNeighbors(index - 1);
            if (index < DistributedText.Count - 1 && DistributedText[index].Contains("закрытая скобка")) CheckNeighbors(index + 1);
        }
    }

    void CheckNeighbors(int index)
    {
        if (DistributedText[index].ToString() != "+" &&
            DistributedText[index].ToString() != "-" &&
            DistributedText[index].ToString() != "/" &&
            DistributedText[index].ToString() != "*" &&
            !DistributedText[index].Contains("открытая скобка") &&
            !DistributedText[index].Contains("закрытая скобка"))
        {
            ErrorDetected("перед скобками должен стоять мат. знак");
        }
    }

    void ErrorDetected(string error)
    {
        managerCalculator.ErrorOutput(error);
        ProtectionPassed = false;
    }

    void AddNumberToTheArray()
    {
        DistributedText.Add(BufferText);
        BufferText = "";
    }

    void FullCleaning()
    {
        InputField.ActivateInputField();
        ProtectionPassed = false;
        DistributedText.Clear();
        SplitInputText = null;
        InputField.text = "";
        SwitchFloat = false;
        GlobalIndex = 0;
        BufferText = "";
    }
}