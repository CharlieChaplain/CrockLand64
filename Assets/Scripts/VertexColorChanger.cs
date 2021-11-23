using UnityEngine;
using System.Collections;
using TMPro;


public class VertexColorChanger : MonoBehaviour
{
    public Color color;

    private TMP_Text m_TextComponent;

    void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }

    public void StartColorChanger(float textSpeed)
    {
        textSpeed = 1f / textSpeed; //inverses it for better readablity outside of this class
        StartCoroutine(AnimateVertexColors(textSpeed));
    }

    /// <summary>
    /// Method to reveal each character of a TMPro one by one
    /// </summary>
    /// <param name="textSpeed">default 1 (twice a frame length), modulates how fast the text appears.</param>
    /// <returns></returns>
    IEnumerator AnimateVertexColors(float textSpeed)
    {
        // Force the text object to update right away so we can have geometry to modify right from the start.
        m_TextComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextComponent.textInfo;
        int currentCharacter = 0;

        Color32[] newVertexColors;

        int characterCount = textInfo.characterCount;

        while (currentCharacter < characterCount)
        {
            // If No Characters then just yield and wait for some text to be added
            if (characterCount == 0)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }

            // Get the index of the material used by the current character.
            int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

            // Only change the vertex color if the text element is visible.
            if (textInfo.characterInfo[currentCharacter].isVisible)
            {
                newVertexColors[vertexIndex + 0] = color;
                newVertexColors[vertexIndex + 1] = color;
                newVertexColors[vertexIndex + 2] = color;
                newVertexColors[vertexIndex + 3] = color;

                // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
                m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                // This last process could be done to only update the vertex data that has changed as opposed to all of the vertex data but it would require extra steps and knowing what type of renderer is used.
                // These extra steps would be a performance optimization but it is unlikely that such optimization will be necessary.
            }

            currentCharacter++;

            //will scroll text faster if jump is held down
            if (Input.GetButtonDown("Jump"))
                yield return new WaitForSeconds(0.04f);
            else
                yield return new WaitForSeconds(0.08f * textSpeed);
        }

        DialogueManager.Instance.FinishedTyping();
    }

}
