using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnomalyEffect
{
    void ApplyEffect(GameObject target, float deltaTime);
    void RemoveEffect(GameObject target);
}
