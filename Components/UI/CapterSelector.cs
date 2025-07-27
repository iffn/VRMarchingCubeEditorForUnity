using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CapterSelector : MonoBehaviour
{
    [SerializeField] List<Button> chapterButtons;
    [SerializeField] List<GameObject> chapters;

    [SerializeField] int defaultChapter = 1;

    int currentChapter = -1;

    // Start is called before the first frame update
    void Start()
    {
        // Error checking
        if(chapterButtons.Count != chapters.Count || chapterButtons.Count == 0)
        {
            Debug.LogError("Chapter setup incorrect");

            gameObject.name = "[Setup Error] " + gameObject.name;
            return;
        }

        foreach (Button button in chapterButtons)
        {
            button.onClick.AddListener(() => SelectChapter(button));
        }

        foreach(GameObject chapter in chapters)
        {
            chapter.SetActive(false);
        }

        SelectChapter(chapterButtons[Mathf.Clamp(defaultChapter, 0, chapterButtons.Count - 1)]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectChapter(Button clickedButton)
    {
        ColorBlock block;

        // Handle old chapter
        if (currentChapter >= 0)
        {
            chapters[currentChapter].SetActive(false);

            Button currentButton = chapterButtons[currentChapter];

            block = currentButton.colors;
            block.normalColor = block.disabledColor;
            block.selectedColor = block.disabledColor;
            currentButton.colors = block;
        }

        // Handle new chapter
        int newChapter = chapterButtons.IndexOf(clickedButton);

        if (newChapter == currentChapter)
        {
            currentChapter = -1;
        }
        else
        {
            block = clickedButton.colors;
            block.normalColor = block.pressedColor;
            block.selectedColor = block.pressedColor;
            clickedButton.colors = block;

            currentChapter = newChapter;
            chapters[currentChapter].SetActive(true);
        }
    }
}
