using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTarget : MonoBehaviour
{
    [SerializeField] private List<GameObject> bodyParts;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    private int i = 0;

    private void Start()
    {
        NextBodyPart();
    }

    public void NextBodyPart()
    {
        if (i < bodyParts.Count) HighlightBodyPart(bodyParts[i]);

        i++;
    }

    private void HighlightBodyPart(GameObject bodyPart)
    {
        // Set the highlight material to the body part
        Renderer renderer = bodyPart.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = highlightMaterial;
        }

        bodyPart.GetComponent<BodyPart>().damagable = true;
    }
}

