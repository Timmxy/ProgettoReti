using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshesPointer
{
    [SerializeField] private SkinnedMeshRenderer[] _skinnedMeshes;

    public SkinnedMeshRenderer[] SkinnedMeshes { get => _skinnedMeshes; }
}
