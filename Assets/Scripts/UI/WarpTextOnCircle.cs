using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WarpTextOnCircle : MonoBehaviour
{
    [CacheComponent]
    [SerializeField]
    private TMP_Text textComponent;

    [SerializeField]
    private float radius = 10.0f;

    [SerializeField]
    private Optional<float> arcDegrees = new(true, 90.0f);

    [SerializeField]
    private float degreesPerLetter;

    [Space]
    [SerializeField]
    private float angularOffset = 0;

    public float Radius
    {
        get => radius;
        set => radius = value;
    }

    public float? ArcDegrees
    {
        get => arcDegrees.Enabled ? arcDegrees.Value : null;
        set
        {
            if (!value.HasValue)
            {
                arcDegrees.Enabled = false;
                return;
            }

            arcDegrees.Enabled = true;
            arcDegrees = value.Value;
        }
    }

    public float DegreesPerLetter
    {
        get => degreesPerLetter;
        set => degreesPerLetter = value;
    }

    public float AngularOffset
    {
        get => angularOffset;
        set => angularOffset = value;
    }

    public string Text
    {
        get => textComponent.text;
        set => textComponent.text = value;
    }

    private void OnEnable()
    {
        UpdateText();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        /*
        if (!Application.isPlaying)
        {
            return;
        }
        */

        UpdateText();
    }
#endif

    private void Update()
    {
        if (textComponent == null)
        {
            return;
        }

        if (!textComponent.havePropertiesChanged)
        {
            return;
        }

        UpdateText();
    }

    public void UpdateText()
    {
        if (textComponent == null)
        {
            return;
        }

        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        // Based on this: https://github.com/TonyViT/CurvedTextMeshPro/tree/master
        textComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = textComponent.textInfo;

        if (textInfo == null)
        {
            return;
        }

        int characterCount = textInfo.characterCount;

        if (characterCount <= 0)
        {
            return;
        }

        Vector3[] vertices;
        Matrix4x4 matrix;

        float boundsMinX = textComponent.bounds.min.x;
        float boundsMaxX = textComponent.bounds.max.x;

        for (int i = 0; i < characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
            {
                continue;
            }

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            vertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 charMidBaselinePos = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i].baseLine);

            vertices[vertexIndex + 0] += -charMidBaselinePos;
            vertices[vertexIndex + 1] += -charMidBaselinePos;
            vertices[vertexIndex + 2] += -charMidBaselinePos;
            vertices[vertexIndex + 3] += -charMidBaselinePos;

            float zeroToOnePos = (charMidBaselinePos.x - boundsMinX) / (boundsMaxX - boundsMinX);

            float actualArcDegrees;
            float degreesPerLetter = (float)(textInfo.characterCount / textInfo.lineCount) * this.degreesPerLetter;

            if (arcDegrees.Enabled)
            {
                actualArcDegrees = arcDegrees;
            }
            else
            {
                actualArcDegrees = degreesPerLetter;
            }

            float angle = ((zeroToOnePos - 0.5f) * actualArcDegrees - 90 + angularOffset) * Mathf.Deg2Rad;

            float x0 = Mathf.Cos(angle);
            float y0 = Mathf.Sin(angle);
            float radiusForThisLine = radius - textInfo.lineInfo[0].lineExtents.max.y * textInfo.characterInfo[i].lineNumber;
            Vector2 newMideBaselinePos = new Vector2(x0 * radiusForThisLine, -y0 * radiusForThisLine); //actual new position of the character

            matrix = Matrix4x4.TRS(new Vector3(newMideBaselinePos.x, newMideBaselinePos.y, 0), Quaternion.AngleAxis(-Mathf.Atan2(y0, x0) * Mathf.Rad2Deg - 90, Vector3.forward), Vector3.one);

            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
        }

        textComponent.UpdateVertexData();
    }
}
