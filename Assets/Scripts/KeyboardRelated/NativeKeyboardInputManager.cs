using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NativeKeyboardInputManager : MonoBehaviour
{
    [SerializeField] public TMP_Text dragonName;
    private TouchScreenKeyboard keyboard;
    private bool isWaitingForName = false;
    private bool nameFinalized = false;

    void Start()
    {
        // Initialize name if already set
        if (!string.IsNullOrWhiteSpace(DragonActive.dragonName))
        {
            dragonName.text = DragonActive.dragonName;
            nameFinalized = true;
        }
        else
        {
            dragonName.text = "";
            nameFinalized = false;
        }
    }

    void Update()
    {
        // Skip update if name is already finalized
        if (nameFinalized) return;

        // Update name text live while keyboard is open
        if (keyboard != null && (keyboard.active || TouchScreenKeyboard.visible))
        {
            dragonName.text = keyboard.text.Trim();
        }

        // Handle keyboard cancel
        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Canceled)
        {
            ShowStatus("Dragon must have a name");
            Invoke(nameof(OpenKeyboard), 1.5f);
            keyboard = null;
            return;
        }

        // Handle keyboard done
        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done && isWaitingForName)
        {
            string trimmedInput = keyboard.text.Trim();

            if (string.IsNullOrEmpty(trimmedInput))
            {
                ShowStatus("Dragon must have a name");
                Invoke(nameof(OpenKeyboard), 1.5f);
                keyboard = null;
                return;
            }

            foreach (string existingName in DragonBookContents.GetDragonNamesInBook())
            {
                if (existingName == trimmedInput)
                {
                    ShowStatus("You already have a dragon by that name");
                    Invoke(nameof(OpenKeyboard), 1.5f);
                    keyboard = null;
                    return;
                }
            }

            // All good — save name
            dragonName.text = trimmedInput;
            DragonActive.dragonName = trimmedInput;
            nameFinalized = true;
            isWaitingForName = false;
            keyboard = null;
        }
    }

    /// <summary>
    /// Call this method manually (e.g., from a UI button) to trigger naming
    /// </summary>
    public void OpenKeyboard()
    {
        if (nameFinalized) return;

        ShowStatus("Please name your dragon");
        isWaitingForName = true;
        keyboard = TouchScreenKeyboard.Open(
            "",
            TouchScreenKeyboardType.Default,
            false, false, false, false,
            "Please name your dragon",
            10
        );
    }

    private void ShowStatus(string message)
    {
        DragonActive.statusText.text = message;
        Invoke(nameof(ClearStatusText), 1.5f);
    }

    private void ClearStatusText()
    {
        DragonActive.statusText.text = "";
    }
}
