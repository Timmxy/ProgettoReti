using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AvatarMeshCombiner : MonoBehaviour
{
    [SerializeField] private string _finalMeshName;             // nome della mesh combinata
    [SerializeField] private SkinnedMeshRenderer _meshRenderer; // componente che renderizza la mesh finale
    [SerializeField] private Transform _meshesContainer;        // trasformata dell'avatar

    // lancia la coroutine per aggiornare la mesh dell'avatar 
    // da attivare al salvataggio dell'avatar???
    public void UpdateAvatarMesh()
    {
        StartCoroutine(MergeMeshesRoutine());
    }

    // coroutine per evitare collisioni di risorse
    private IEnumerator MergeMeshesRoutine()
    {
        // Waiting one frame before updating the mesh because otherwise the new equipment won't have spawned
        yield return new WaitForEndOfFrame();
        UpdateAvatarMeshImmediate();
    }


    public void UpdateAvatarMeshImmediate()
    {
        // salvo in un array tutti i componenti SkinnedMeshRenderer dei figli (body,legs,head+ vestiti) dell'avatar
        SkinnedMeshRenderer[] renderers = _meshesContainer.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        MergeSkinnedMeshes(renderers);
    }

    // fonde tutte le mesh che compongono i pezzi dell'avatar in una unica mesh che eredita rigging
    private void MergeSkinnedMeshes(SkinnedMeshRenderer[] targets)
    {
        #region Error Catching
        // controlla che l'array sia inizializzato e non vuoto
        if (targets == null || targets.Length < 1)
        {
            Debug.LogError("Target meshes array is null or too short");
            return;
        }
        #endregion

        #region Create and merge the new mesh
        // crea la mesh che sarà finale, gli assegna il nome finale
        Mesh finalMesh = new Mesh();
        finalMesh.name = _finalMeshName;

        // Get The total number of SubMeshes
        int totalSubmeshes = 0;

        // per ogni MeshRenderer nell'array passato (componenti dell'avatar),
        // conta il numero di subMesh e le aggiunge al contatore totale
        foreach (SkinnedMeshRenderer target in targets)
        {
            totalSubmeshes += target.sharedMesh.subMeshCount;
        }

        // Get Material Array
        Material[] allMaterials = new Material[totalSubmeshes];

        int submeshOffset = 0;

        // per ogni elemento di targets (contiene MeshRenderer)
        for (int i = 0; i < targets.Length; i++)
        {
            // per ogni subMesh
            for (int s = 0; s < targets[i].sharedMesh.subMeshCount; s++)
            {
                // salvo i materiali delle subMesh
                allMaterials[submeshOffset] = targets[i].sharedMaterials[s];
                submeshOffset++;
            }
        }

        // Get Meshes Array
        Mesh[] allMeshes = new Mesh[targets.Length];

        // popolo il nuovo array di mesh con le sharedMesh di ciascun elemento di targets
        for (int i = 0; i < targets.Length; i++)
        {
            allMeshes[i] = targets[i].sharedMesh;
        }

        // creo un array di CombineInstances di dimensione uguale al numero di subMeshes in tutto l'avatar
        CombineInstance[] combineInstances = new CombineInstance[totalSubmeshes];

        int combineOffset = 0;
        // per ciascuna mesh
        for (int i = 0; i < allMeshes.Length; i++)
        {
            // per ciascuna subMesh in questa mesh
            for (int s = 0; s < allMeshes[i].subMeshCount; s++)
            {
                // creo una combine instance nell'array appena creato,
                // ne assegno la mesh,
                // assegno l'indice di subMesh a quello del ciclo for,
                // assegno la trasformata 
                combineInstances[combineOffset] = new CombineInstance();
                combineInstances[combineOffset].mesh = allMeshes[i];
                combineInstances[combineOffset].subMeshIndex = s;
                combineInstances[combineOffset].transform = targets[i].localToWorldMatrix;
                combineOffset++;
            }
        }

        // una volta popolata ciascuna CombineInstance, questo metodo raggruppa tutto in una singola mesh
        // (CombineInstance è una struct apposita per fare merge delle mesh al suo interno)
        finalMesh.CombineMeshes(combineInstances, false, false);

        #endregion
 
        #region Set the bindposes for the new mesh

        Matrix4x4[] bindPoses = targets[0].sharedMesh.bindposes;
        finalMesh.bindposes = bindPoses;
        #endregion

        #region Recalculate bone weights
        BoneWeight[] finalBoneWeights = finalMesh.boneWeights;
        
        // Bones in the submeshes have and index of i + submesh index when the mesh is merged
        

        int bonesArrayLenght = _meshRenderer.bones.Length;

        for (int i = 0; i < finalMesh.subMeshCount; i++)
        {
            SubMeshDescriptor currentSubmesh = finalMesh.GetSubMesh(i);

            //    Debug.Log("Base Vertex: " + currentSubmesh.firstVertex);
            //    Debug.Log("Vertex Count: " + currentSubmesh.vertexCount);

            int boneIndexSubtraction = bonesArrayLenght * i;

            for (int v = 0; v < currentSubmesh.vertexCount; v++)
            {
                finalBoneWeights[v + currentSubmesh.firstVertex].boneIndex0 -= boneIndexSubtraction;
                finalBoneWeights[v + currentSubmesh.firstVertex].boneIndex1 -= boneIndexSubtraction;
                finalBoneWeights[v + currentSubmesh.firstVertex].boneIndex2 -= boneIndexSubtraction;
                finalBoneWeights[v + currentSubmesh.firstVertex].boneIndex3 -= boneIndexSubtraction;
            }
        }

        finalMesh.boneWeights = finalBoneWeights;

        #endregion

        #region Recalculate Mesh Bounds

        finalMesh.RecalculateBounds();

        #endregion

        #region Set the final skinned mesh

        _meshRenderer.sharedMesh = finalMesh;
        _meshRenderer.materials = allMaterials;

        #endregion
        Debug.Log("culo");
    }
}