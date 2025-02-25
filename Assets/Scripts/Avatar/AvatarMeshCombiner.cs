using System.Collections;
using UnityEngine;

public class AvatarMeshCombiner : MonoBehaviour
{
    [SerializeField] private string _finalMeshName;             // Nome della mesh combinata
    [SerializeField] private MeshFilter _meshFilter;            // Componente per la mesh finale
    [SerializeField] private MeshRenderer _meshRenderer;        // Renderer per la mesh finale
    [SerializeField] private Transform _meshesContainer;        // Contenitore delle mesh dell'avatar

    // Lancia la coroutine per aggiornare la mesh dell'avatar 
    public void UpdateAvatarMesh()
    {
        StartCoroutine(MergeMeshesRoutine());
    }

    // Coroutine per evitare collisioni di risorse
    private IEnumerator MergeMeshesRoutine()
    {
        yield return new WaitForEndOfFrame();
        UpdateAvatarMeshImmediate();
    }

    public void UpdateAvatarMeshImmediate()
    {
        MeshFilter[] meshFilters = _meshesContainer.GetComponentsInChildren<MeshFilter>(true);
        MergeMeshes(meshFilters);
    }

    // Unisce tutte le mesh dell'avatar in una singola mesh
    private void MergeMeshes(MeshFilter[] targets)
    {
        if (targets == null || targets.Length < 1)
        {
            Debug.LogError("Nessuna mesh trovata per la combinazione");
            return;
        }

        Mesh finalMesh = new Mesh { name = _finalMeshName };

        // Creazione degli array per la combinazione delle mesh
        CombineInstance[] combineInstances = new CombineInstance[targets.Length];
        Material[] allMaterials = new Material[targets.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            combineInstances[i].mesh = targets[i].sharedMesh;
            combineInstances[i].transform = targets[i].transform.localToWorldMatrix;
            allMaterials[i] = targets[i].GetComponent<MeshRenderer>().sharedMaterial;
        }

        // Combina le mesh
        finalMesh.CombineMeshes(combineInstances, false, true);
        finalMesh.RecalculateNormals();
        finalMesh.RecalculateBounds();

        // Assegna la mesh combinata al MeshFilter e il materiale al MeshRenderer
        _meshFilter.mesh = finalMesh;
        _meshRenderer.materials = allMaterials;
    }
}
