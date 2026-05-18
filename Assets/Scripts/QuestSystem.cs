using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Keeps track of all quests within the level
/// </summary>
public class QuestSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject questPrefab;
    [SerializeField]
    private List<Quest> quests;

    /// <summary>
    /// Represents a single quest, needs to be refactored TODO
    /// </summary>
    [Serializable]
    private class Quest
    {
        public int pointRequirement;
        public string GetText() => $"Get {this.pointRequirement} points";
    }
    
    private void Start() => StartCoroutine(LoadQuests());

    private IEnumerator LoadQuests()
    {
        yield return new WaitForSeconds(5);
        foreach (Quest quest in this.quests)
        {
            GameObject questObj = Instantiate(this.questPrefab, this.transform);
            questObj.GetComponentInChildren<TextMeshProUGUI>().text = quest.GetText();
            yield return new WaitForSeconds(0.2f);
        }
    }
}
