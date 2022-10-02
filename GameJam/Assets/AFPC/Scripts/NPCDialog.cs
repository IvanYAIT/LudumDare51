using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCDialog : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private GameObject questPanel;

    private int item;
    private int dialogCount;
    private int dialogLine;
    private int questCount;
    private int questLine;
    private bool isDialog;
    private bool isQuest;
    private bool isRest;
    private Transform dialog;
    private Transform quest;

    void Start()
    {
        dialogCount = 0;
        questCount = 0;
        dialogLine = 1;
        questLine = 1;
        isQuest = false;
    }

    void Update()
    {
        isRest = ProjectManager.instance.isRest;
        item = ProjectManager.instance.item;
        isDialog = ProjectManager.instance.isDialog;

        if (isRest)
        {
            isQuest = false;
            isRest = false;
            quest.gameObject.SetActive(false);
            ProjectManager.instance.isRest = isRest;
            questCount += 1;
            questLine = 1;
        }
        
        dialog = GetPanelChild(dialogPanel, dialogCount);
        quest = GetPanelChild(questPanel, questCount);
        if (isQuest && item == 1)
        {
            NextTask();
            item -= 1;
            ProjectManager.instance.item = item;
            isQuest = false;
        }

        if (isDialog)
        {
            Dialog(true);
            if (!isQuest)
            {
                if (dialog.childCount + 1 == dialogLine)
                {
                    dialogLine = 1;
                    dialogCount += 1;
                    dialog.gameObject.SetActive(false);
                    NextTask();
                    isQuest = true;
                    isDialog = false;
                    ProjectManager.instance.isDialog = isDialog;
                }
                if ((Input.anyKeyDown && !Input.GetKey(KeyCode.E)) && dialog.childCount != dialogLine)
                {
                    dialog.GetChild(dialogLine - 1).gameObject.SetActive(false);
                    dialog.GetChild(dialogLine).gameObject.SetActive(true);
                    dialogLine += 1;
                } else if ((Input.anyKeyDown && !Input.GetKey(KeyCode.E)) && dialog.childCount == dialogLine)
                {
                    dialogLine += 1;
                }
                
            }
            else
            {
                Dialog(false);
                isDialog = false;
                ProjectManager.instance.isDialog = isDialog;
            }
        }
        else
        {
            Dialog(false);
            Time.timeScale = 1;
        }
    }

    public Transform GetPanelChild(GameObject panel, int count) => panel.transform.GetChild(count);
    public void NextTask()
    {
        quest.transform.GetChild(questLine - 1).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        quest.transform.GetChild(questLine).gameObject.SetActive(true);
        questLine++;
    }
    public void Dialog(bool onOff)
    {
        dialogPanel.SetActive(onOff);
        dialog.gameObject.SetActive(onOff);
        questPanel.SetActive(!onOff);
        quest.gameObject.SetActive(!onOff);
    }
}
