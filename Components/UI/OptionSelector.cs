using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour
{
    [SerializeField] Button baseButton;
    List<Button> buttons = new List<Button>();

    bool allowDeselect;
    int currentOption = -1;
    OptionUser linkedOptionUser;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(OptionUser optionUser, List<string> buttonNames, bool allowDeselect, int defaultOption = 0)
    {
        if(baseButton == null)
        {
            Debug.LogWarning("Error: Base button not assigned");
            gameObject.name = "[Error] " + gameObject.name;
            Debug.LogWarning("Error: Base button not assigned on " + gameObject.name);
            return;
        }

        // Cleanup
        if (buttons.Count > 1)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                GameObject.Destroy(buttons[i]);
            }

            buttons.Clear();
        }

        linkedOptionUser = optionUser;

        if (buttonNames == null || buttonNames.Count == 0)
            return;

        SetupButton(baseButton, 0, buttonNames[0]);

        if (buttonNames.Count > 1)
        {
            for (int i = 1; i < buttonNames.Count; i++)
            {
                Button newButton = GameObject.Instantiate(baseButton, baseButton.transform.parent);

                SetupButton(newButton, i, buttonNames[i]);
            }
        }

        if(!allowDeselect)
            defaultOption = System.Math.Clamp(defaultOption, 0, buttonNames.Count - 1);
        else if(defaultOption > buttonNames.Count - 1)
            defaultOption = buttonNames.Count - 1;

        SelectOption(defaultOption);
    }

    void SetupButton(Button button, int index, string name)
    {
        button.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = name;

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() => SelectOption(index));

        if(!buttons.Contains(button))
            buttons.Add(button);
    }

    public void SelectOption(int buttonIndex)
    {
        if (!allowDeselect && buttonIndex == currentOption)
            return;

        ColorBlock block;

        // Handle old option
        if (currentOption >= 0)
        {
            Button currentButton = buttons[currentOption];

            block = currentButton.colors;
            block.normalColor = block.disabledColor;
            block.selectedColor = block.disabledColor;
            currentButton.colors = block;
        }

        Button clickedButton = buttons[buttonIndex];

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
