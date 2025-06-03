using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteSaveFileButton : MonoBehaviour
{
    [CacheComponent]
    [SerializeField] private Button button;

    private bool _showingWarning;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (_showingWarning)
        {
            return;
        }

        _showingWarning = true;

        PopupWindow.Open("Warning!",
            "Are you sure you want to PERMANENTLY DELETE your save file?\nThis action cannot be undone.",
            ("No", () => _showingWarning = false),
            ("Yes", () =>
            {
                SceneSwitcher.SwitchScene(SceneSwitcher.CurrentSceneBuildIndex, () =>
                {
                    SaveManager.DeleteSave();
                    DragonActive.isDragonActive = false;
                    DragonActive.doCheck = true;
                });
            }
        ));
    }
}
