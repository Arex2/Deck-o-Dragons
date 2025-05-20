using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Vs screen before the card game starts
/// </summary>
public class CardgameIntroSequence : MonoBehaviour
{
    public bool DoingIntro { get; private set; }

    [SerializeField] private GameObject introSequenceObject;
    [SerializeField] private float duration;

    [Space]
    [SerializeField] private Transform playerPoint;
    [SerializeField] private Transform enemyPoint;

    private Vector3 _oldPlayerPointPos;
    private Vector3 _oldEnemyPointPos;

    [Space]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text enemyNameText;

    [Space]
    [SerializeField] private RawImage playerImage;
    [SerializeField] private RawImage enemyImage;

    [Space]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera enemyCamera;

    private GameObject _player;
    private GameObject _enemy;

    private Vector3 _playerOrigin;
    private Vector3 _enemyOrigin;

    public void Initialize(GameObject player, string playerName, GameObject enemy, string enemyName)
    {
        DoingIntro = true;

        introSequenceObject.SetActive(true);

        playerNameText.text = playerName;
        enemyNameText.text = enemyName;

        ResizeRenderTexture(playerImage.texture as RenderTexture, (playerImage.transform as RectTransform).rect.size);
        ResizeRenderTexture(enemyImage.texture as RenderTexture, (enemyImage.transform as RectTransform).rect.size);

        _player = player;
        _enemy = enemy;

        SetupCamera(_player, playerCamera, ref _playerOrigin);
        SetupCamera(_enemy, enemyCamera, ref _enemyOrigin);

        StartCoroutine(Coroutine());
    }

    private IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(duration);

        DoingIntro = false;

        introSequenceObject.SetActive(false);
    }

    private void ResizeRenderTexture(RenderTexture rt, Vector2 newSize)
    {
        rt.Release();

        rt.width = Mathf.CeilToInt(newSize.x);
        rt.height = Mathf.CeilToInt(newSize.y);

        rt.Create();
    }

    private void SetupCamera(GameObject obj, Camera camera, ref Vector3 pos)
    {
        camera.enabled = false;

        if (obj == null)
        {
            return;
        }

        camera.enabled = true;

        pos = camera.transform.position;

        pos.z = 0;

        pos += obj.transform.position;
        obj.transform.position = pos;
    }

    private void Update()
    {
        if (!DoingIntro)
        {
            return;
        }

        UpdatePos(ref _oldPlayerPointPos, playerPoint, _player, _playerOrigin);
        UpdatePos(ref _oldEnemyPointPos, enemyPoint, _enemy, _enemyOrigin);
    }

    private void UpdatePos(ref Vector3 oldPointPos, Transform point, GameObject obj, Vector3 objOrigin)
    {
        if (obj == null)
        {
            return;
        }

        Vector3 pointPos = point.position;

        if (oldPointPos == pointPos)
        {
            return;
        }

        oldPointPos = pointPos;

        Transform objTransform = obj.transform;

        Vector2 offsetPoint = mainCamera.ScreenToWorldPoint(pointPos);
        offsetPoint -= (Vector2)mainCamera.transform.position;

        objTransform.position = objOrigin + (Vector3)offsetPoint;
    }
}
