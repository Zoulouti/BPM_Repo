using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "New EnemyArchetype", menuName = "EnemyArchetype")]
public class EnemyArchetype : ScriptableObject
{
    public Head _head = new Head();
    [Serializable]
    public class Head
    {
        public TypeOfSpot _typeOfSpot;
    }
    [Space]
    public RightShoulder _rightShoulder = new RightShoulder();
    [Serializable]
    public class RightShoulder
    {
        public TypeOfSpot _typeOfSpot;
    }
    [Space]
    public LeftShoulder _leftShoulder = new LeftShoulder();
    [Serializable]
    public class LeftShoulder
    {
        public TypeOfSpot _typeOfSpot;
    }
    [Space]
    public Torso _torso = new Torso();
    [Serializable]
    public class Torso
    {
        public TypeOfSpot _typeOfSpot;
    }
    [Space]
    public Backo _backo = new Backo();
    [Serializable]
    public class Backo
    {
        public TypeOfSpot _typeOfSpot;
    }
    [Space]
    public RightKnee _rightKnee = new RightKnee();
    [Serializable]
    public class RightKnee
    {
        public TypeOfSpot _typeOfSpot;
    }
    [Space]
    public LeftKnee _leftKnee = new LeftKnee();
    [Serializable]
    public class LeftKnee
    {
        public TypeOfSpot _typeOfSpot;
    }

    TypeOfSpot[] _typeOfSpot;
    public TypeOfSpot[] e_TypeOfSpot { get => _typeOfSpot; set => _typeOfSpot = value; }

    public void PopulateArray()
    {
        _typeOfSpot = new TypeOfSpot[] { _head._typeOfSpot, _rightShoulder._typeOfSpot, _leftShoulder._typeOfSpot, _torso._typeOfSpot, _backo._typeOfSpot, _rightKnee._typeOfSpot, _leftKnee._typeOfSpot };
    }

    public enum TypeOfSpot
    {
        NoSpot,
        WeakSpot,
        ArmorSpot
    }

}
