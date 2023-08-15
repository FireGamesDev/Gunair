using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckHuntSkyTarget : MonoBehaviour, ITarget
{
    [SerializeField] private Material blue;
    [SerializeField] private Material red;
    [SerializeField] private MeshRenderer mesh;

    private Coroutine routine;

    public void Hit(RaycastHit contactPoint)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(OnHit());
    }

    private IEnumerator OnHit()
    {
        mesh.material = red;

        yield return new WaitForSeconds(0.2f);

        mesh.material = blue;
    }
}
