using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour
{
    [SerializeField] private List<DragonController> _dragonList = new List<DragonController>();
    [SerializeField] private MovementController _movementController;
}