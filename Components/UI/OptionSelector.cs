using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour
{
    [SerializeField] Button baseButton;
    List<Button> buttons = new List<Button>();

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

    public void Setup(OptionUser optionUser, List<string> buttonNames, int defaultOption = 0)
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

        buttons.Add(baseButton);

        SetupButton(baseButton, 0, buttonNames[0]);

        if (buttonNames.Count > 1)
        {
            for (int i = 1; i < buttonNames.Count; i++)
            {
                Button newButton = GameObject.Instantiate(baseButton, baseButton.transform.parent);

                SetupButton(newButton, i, buttonNames[i]);
            }
        }

        SelectOption(defaultOption);
    }

    void SetupButton(Button button, int index, string name)
    {
        button.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = name;

        button.onClick.AddListener(() => SelectOption(index));
    }

    public void SelectOption(int buttonIndex)
    {
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

        // Handle new option
        int newOption = buttonIndex;

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
