﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider2D))]
public class CubeGenerator<T> : SingletonBehaviour<CubeGenerator<T>>
{
    public float CubeFallHeight = 2;
    public float Spacing = 0.1f;
    public Vector2 RangeRotationX = new Vector2(-2, 2);
    public Vector2 RangeRotationY = new Vector2(-3, 3);
    public GameObject CubePrefab;
    public Action<Cube<T>> OnCubeSpawned;

    private Vector2 _gridsize;
    private Vector2 _cubeSize;
    private Vector3 _startLocation;
    private List<Cube<T>> _cubes = new List<Cube<T>>();


    [Header("Cube Selector")]
    public int selectedCube = 3;

    protected virtual void OnValidate()
    {
        if (CubePrefab != null && CubePrefab.GetComponent<Cube<T>>() == null)
        {
            Debug.LogError("The prefab must contain a Cube<" + typeof(T).Name + "> component.");
            CubePrefab = null;
        }
    }
    protected virtual void Awake()
    {
        BoxCollider2D collider2D = GetComponent<BoxCollider2D>();
        Bounds bounds = collider2D.bounds;
        collider2D.enabled = false;
        GameObject cubeInstance = Instantiate(CubePrefab.gameObject);
        _cubeSize = cubeInstance.GetComponent<BoxCollider2D>().bounds.size;
        Destroy(cubeInstance);
        _startLocation = new Vector3(_cubeSize.x / 2f + bounds.min.x, bounds.max.y + (CubeFallHeight * _cubeSize.y), bounds.min.z);
        _gridsize = new Vector2((int)Mathf.Floor((bounds.size.x + Spacing) / _cubeSize.x) - 1, (int)Mathf.Floor(bounds.size.y / _cubeSize.y));
    }

    public virtual Cube<T> CreateCube(int row)
    {
        //CORRIGIDO
        if (WordManager.Instance.IsAllWordsUsed() != true)
        {
            Vector3 position = _startLocation + new Vector3((_cubeSize.x + Spacing) * row, 0);
            float randomRotationx = Random.Range(RangeRotationX.x, RangeRotationX.y);
            //float randomRotationY = Random.Range(RangeRotationY.x, RangeRotationY.y);
            //Vector3 randomRotation = new Vector3(randomRotationx, randomRotationY);
            Cube<T> cube = Instantiate(CubePrefab, position, Quaternion.Euler(0,0, randomRotationx), transform).GetComponent<Cube<T>>();
            cube.Row = row;
            cube.Setup();
            cube.OnClicked += OnCubeClicked;
            _cubes.Add(cube);
            OnCubeSpawned?.Invoke(cube);
            return cube;
        }
        else return null;
    }

    private void OnCubeClicked(Cube<T> cube)
    {
        CreateCube(cube.Row);
        _cubes.Remove(cube);
        //StartCoroutine(SelectDelay());

    }

    public virtual Cube<T> CreateCube(T data, int row)
    {
        var created = CreateCube(row);
        created.Data = data;
        return created;
    }

    public virtual void GenerateGrid()
    {
        for (int i = 0; i < _gridsize.x; i++)
        {
            for (int j = 0; j < _gridsize.y; j++)
            {
                CreateCube(i);
            }
        }

        //StartCoroutine(SelectDelay());
    }

    public virtual void Clear()
    {
        int cubeCount = _cubes.Count;
        for (var i = 0; i < cubeCount; i++)
        {
            _cubes[0].Destroy();
            _cubes.RemoveAt(0);
        }
    }

    public void SelectCube()
    {
        foreach (Cube<T> c in _cubes)
        {
            if (c.Id == selectedCube)
            {
                c.SelectCube();
            }
            else
            {
                c.DeselectCube();
            }
        }
    }


    public void ClickSelectCube()
    {
        foreach (Cube<T> c in _cubes)
        {
            if (c.Id == selectedCube)
            {
                c.Click();
            }
        }
    }


    public IEnumerator SelectDelay()
    {
        yield return new WaitForSeconds(1f); // waits 3 seconds
        SelectCube();
    }




    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (selectedCube + 1 <= 19)
            {
                selectedCube += 1;
                SelectCube();
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (selectedCube - 1 >= 0)
            {
                selectedCube -= 1;
                SelectCube();
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            selectedCube -= 4;
            SelectCube();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            selectedCube += 4;
            SelectCube();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClickSelectCube();
        }
    }
    */
}
