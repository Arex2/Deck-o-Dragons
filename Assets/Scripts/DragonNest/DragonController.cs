using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DragonController : MonoBehaviour
{
    private DragonActive dragonActive;
    [SerializeField] private Vector2 eggStart = Vector2.zero;

    private CanvasGroup screenCover;

    private bool _oldHasName = true;

    private void Start()
    {
        screenCover = GameObject.Find("ScreenCover").GetComponent<CanvasGroup>();
        dragonActive = GameObject.Find("DragonActive").GetComponent<DragonActive>();
        /*
        evolutionSlider = GameObject.Find("EvolutionSlider").GetComponent<Slider>();

        evolutionSlider.minValue = 0;
        evolutionSlider.maxValue = 3;
        evolutionSlider.value = DragonActive.evolutionProcess;
        */

        DragonActive.CurrentDragon = this;
    }

    void Update()
    {
#if UNITY_EDITOR
        /*
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnNewDragon();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            StepProgress();
        }
        */
#endif

        // Block raycasts while we don't have a name for the dragon
        bool hasName = !string.IsNullOrEmpty(DragonActive.dragonName);
        if (_oldHasName == hasName)
        {
            return;
        }

        _oldHasName = hasName;

        screenCover.blocksRaycasts = !hasName;
    }

    public void StepProgress()
    {
        DragonActive.evolutionProcess++;

        if (DragonActive.evolutionProcess >= 3 && DragonActive.age < 3)
        {
            DragonActive.evolutionProcess = 0;
            SaveManager.Save();

            if (!SaveManager.SeenEvolutionPopup)
            {
                SaveManager.SeenEvolutionPopup = true;

                PopupWindow.Open("Huh?", $"\nSomething seems to be happening to {DragonActive.dragonName}?", ("???", () =>
                {
                    Invoke(nameof(SpawnNewDragon), 0.4f);
                }));
            }
            else
            {
                SpawnNewDragon();
            }
        }

        // Special case for adults
        else if (DragonActive.age >= 3 && DragonActive.evolutionProcess >= 2)
        {
            //DragonActive.statusText.text = "Congratulations! " + DragonActive.dragonName + " has completed their training and will be added to your registry!";
            PopupWindow.Open("Congratulations!",
                $"{DragonActive.dragonName} has fully completed their training!",
                ("OK", () =>
                {
                    DragonBookContents.SetNewDragonNameAndTypeInBook(DragonActive.dragonName, DragonActive.index);

                    //Reset the game
                    ProgressManager.currentLevel = 0;
                    ProgressManager.wonLastBattle = false;

                    DeckManager.Instance.InitializeDeck(DeckManager.Instance.DefaultStarterDeck);
                    DeckManager.Instance.BlackListCards.Clear();

                    if (DragonActive.dragonName != null && DragonBookContents.GetDragonsActiveInBackyard().Count < DragonBookContents.LimitOfDragons)
                    {
                        DragonBookContents.SetNewDragonNamesAndElementsActiveInBackyard(DragonActive.dragonName, DragonActive.index);
                    }

                    DragonActive.isDragonActive = false;
                    DragonActive.evolutionProcess = 0;

                    SaveManager.Save();

                    if (!SaveManager.SeenBackyardPopup)
                    {
                        Invoke(nameof(BackYardPopup), 1f);
                    }
                    else
                    {
                        Invoke(nameof(SpawnNewDragon), 1f);
                    }
                }
            ));

        }
        else
        {
            screenCover.blocksRaycasts = false;
        }
    }

    private void BackYardPopup()
    {
        if (SaveManager.SeenBackyardPopup)
        {
            return;
        }

        SaveManager.SeenBackyardPopup = true;

        PopupWindow.Open("The Backyard",
            "When a dragon is done with it's training it'll go to your backyard.\nTo access the backyard, tap the arrow on the top left side of the screen!",
            ("OK", () => Invoke(nameof(SpawnNewDragon), 1f)));
    }

    public void SpawnNewDragon()
    {
        DragonController dragonController = null;

        if (DragonActive.age == 1)
        {
            dragonController = Instantiate(dragonActive.teenDragons[DragonActive.index], new Vector3(0, -3, 0), Quaternion.identity).GetComponent<DragonController>();
            DragonActive.age++;
        }
        else if (DragonActive.age == 2)
        {
            dragonController = Instantiate(dragonActive.adultDragons[DragonActive.index], new Vector3(0, -4, 0), Quaternion.identity).GetComponent<DragonController>();
            DragonActive.age++;
        }
        else if (DragonActive.age == 3)
        {
            Egg egg = Instantiate(dragonActive.egg, eggStart, Quaternion.identity);
            egg.canBeCracked = false;
            egg.animator.enabled = true;
            egg.animator.SetTrigger("Fall");
            DragonActive.age = 0;
        }
        else
        {
            return;
        }

        if (dragonController != null)
        {
            dragonController.EvolutionPopup();
        }

        //Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
        //Instantiate(drakPrefab, drakStart, Quaternion.identity);*/
        Destroy(gameObject);
    }

    private void EvolutionPopup()
    {
        Invoke(nameof(DoEvolutionPopup), 1);
    }

    private void DoEvolutionPopup()
    {
        PopupWindow.Open("Congratulations!",
                $"\n{DragonActive.dragonName} has aged and evolved to its next stage!",
                ("OK", () => screenCover.blocksRaycasts = false));
    }
}