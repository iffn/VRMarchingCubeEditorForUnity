using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour
{
    [SerializeField] List<Button> optionButtons;
    OptionUser linkedOptionUser;

    int currentOption = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(OptionUser optionUser, int defaultOption = 0)
    {
        linkedOptionUser = optionUser;

        // Error checking
        if (optionButtons.Count == 0)
        {
            Debug.LogWarning("No option buttons set up for OptionSelector");
            gameObject.name = "[Setup Error] " + gameObject.name;
            return;
        }

        for (int i = 0; i < optionButtons.Count; i++)
        {
            int index = i; // capture the current value
            optionButtons[i].onClick.AddListener(() => SelectOption(index));
        }

        SelectOption(Mathf.Clamp(defaultOption, 0, optionButtons.Count - 1));
    }

    public void SelectOption(int buttonIndex)
    {
        ColorBlock block;

        // Handle old option
        if (currentOption >= 0)
        {
            Button currentButton = optionButtons[currentOption];

            block = currentButton.colors;
            block.normalColor = block.disabledColor;
            block.selectedColor = block.disabledColor;
            currentButton.colors = block;
        }

        // Handle new option
        int newOption = buttonIndex;

        Button clickedButton = optionButtons[buttonIndex];

        if (buttonIndex == currentOption)
        {
            currentOption = -1;
        }
        else
        {
            block = clickedButton.colors;
            block.normalColor = block.pressedColor;
            block.selectedColor = block.pressedColor;
            clickedButton.colors = block;

            currentOption = buttonIndex;
        }

        if(linkedOptionUser != null)
            linkedOptionUser.SelectOption(this, currentOption);
    }
}
