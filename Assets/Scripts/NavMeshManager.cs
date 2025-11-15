using System;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    private static NavMeshManager _instance;
    
    private NavMeshSurface _navMeshSurface;

    public static NavMeshManager GetInstance() => _instance;
    
    private void Awake()
    {
        _instance = this;
        _navMeshSurface = GetComponent<NavMeshSurface>();
    }

    private void Start()
    {
        ReloadNavMesh();
    }

    public void ReloadNavMesh()
    {
        StartCoroutine(UpdateNavMesh());
    }

    private IEnumerator UpdateNavMesh()
    {
        yield return new WaitForEndOfFrame();
        _navMeshSurface.BuildNavMesh();
    }
}
