using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroState : IState
{
    private GameBehaviour gameBehaviour;
    private CardgameIntroSequence introSequence;

    private bool _doingIntro;
    private bool _nextState;
    private bool? _doTutorial;

    private GameObject _player;
    private GameObject _enemyClone;

    private EnemyBoss _enemyBoss;

    public void Enter(GameBehaviour gameBehaviour)
    {
        this.gameBehaviour = gameBehaviour;
        introSequence = gameBehaviour.GetComponentInChildren<CardgameIntroSequence>(true);

        Debug.Log("DOING AN INTRO!");

        gameBehaviour.StatusButton.ProceedStatus(CardgameStatusButton.INTRO, CardgameStatusButton.PLAYER_TURN);

        _enemyBoss = TargetManager.GetLeader(Team.Enemy) as EnemyBoss;

        if (DragonActive.Instance == null)
        {
            return;
        }

        _player = DragonActive.Instance.SpawnDragon();

        if (_player == null)
        {
            return;
        }

        string playerName = DragonActive.dragonName;

        // Clean the dragon prefab
        Object.Destroy(_player.GetComponentInChildren<Dragon>(true));
        Object.Destroy(_player.GetComponentInChildren<DragonController>(true));

        Transform playerTransform = _player.transform;
        int length = playerTransform.childCount;
        for (int i = length - 1; i >= 0; i--)
        {
            Transform child = playerTransform.GetChild(i);
            GameObject childObj = child.gameObject;
            string name = childObj.name.ToLower().Trim();

            if (name.StartsWith("dirt") || name.StartsWith("glitter") || name.StartsWith("heart"))
            {
                Object.Destroy(childObj);
            }
        }

        // Create enemy clone
        _enemyClone = new GameObject("Enemy Clone (VS Screen)");
        _enemyClone.transform.position = new Vector3(0, -0.5f, 0);

        SpriteRenderer spriteRenderer = _enemyClone.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = _enemyBoss.Sprite;

        introSequence.Initialize(_player, playerName, _enemyClone, _enemyBoss.Name);
        _doingIntro = true;
    }

    public IEnumerator Coroutine()
    {
        if (_doingIntro)
        {
            yield return new WaitWhile(() => introSequence.DoingIntro);
        }

        _enemyBoss.Appear();

        // Tutorial popup
        if (!SaveManager.SeenBattleTutorial)
        {
            SaveManager.SeenBattleTutorial = true;

            yield return new WaitForSeconds(0.75f);

            PopupWindow.Open(
                "Need some help?",
                "This seems like your first time in a BATTLE!\n\nWant to do the BATTLE Tutorial?",
                ("Yes", () => _doTutorial = true),
                ("No", () => _doTutorial = false)
                );

            yield return new WaitUntil(() => _doTutorial.HasValue);

            if (_doTutorial.Value)
            {
                CardgameTutorialManager.StartTutorial();

                yield return new WaitUntil(() => CardgameTutorialManager.TutorialStep >= 1);
            }
        }

        _nextState = true;
    }

    public IState Execute()
    {
        if (_nextState)
        {
            return new NewTurnState();
        }

        return null;
    }

    public void Exit()
    {
        if (_player != null)
        {
            Object.Destroy(_player);
        }

        if (_enemyClone != null)
        {
            Object.Destroy(_enemyClone);
        }
    }
}
