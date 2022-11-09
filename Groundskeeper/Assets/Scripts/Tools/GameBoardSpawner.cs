using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardSpawner : MonoBehaviour {


    [SerializeField] GameObject holder, envHolder;


    [HideInInspector] public KdTree<LumberjackInstance> aHelpers = new KdTree<LumberjackInstance>();
    [HideInInspector] public KdTree<MonsterInstance> monsters = new KdTree<MonsterInstance>();
    [HideInInspector] public KdTree<EnvironmentInstance> environment = new KdTree<EnvironmentInstance>();


    public const float boardRadius = 750f;
}
