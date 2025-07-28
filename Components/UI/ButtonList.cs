using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonList : MonoBehaviour
{
    [SerializeField] Button baseButton;

    List<Button> buttons = new List<Button>();

    IButtonListUser user;

    public void Setup(IButtonListUser user, List<string> buttonNames)
    {
        // Cleanup
        if(buttons.Count > 1)
        {
            for(int i = 0; i < buttons.Count; i++)
            {
                GameObject.Destroy(buttons[i]);
            }

            buttons.Clear();
        }

        this.user = user;

        // First button
        if (buttonNames == null || buttonNames.Count == 0)
            return;

        buttons.Add(baseButton);

        SetupButton(baseButton, 0, buttonNames[0]);

        // Other buttons
        if(buttonNames.Count > 1)
        {
            for(int i = 1; i < buttonNames.Count; i++)
            {
                Button newButton = GameObject.Instantiate(baseButton);

                newButton.transform.parent = baseButton.transform.parent;

                SetupButton(newButton, i, buttonNames[i]);
            }
        }
    }

    void SetupButton(Button button, int index, string name)
    {
        button.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = name;

        button.onClick.AddListener(() => ClickButton(index));
    }

    void ClickButton(int index)
    {
        user.UseButton(this, index);
    }
}
