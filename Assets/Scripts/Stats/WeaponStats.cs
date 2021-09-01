using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponStats : ScriptableObject
{
    public new string name;
    public int cost;
    public float penetrattionThickness;
    public int damageHead, damageBody, damageArmsLegs;
    public int dropOffDsitance;
    public int decreseDamageRate;
    public float fireRate;
    public float runSpeed;
    public float reloadSpeed;
    public float magazine;
    public Sprite artwork;
    public Transform casingPrefab;
    public TrailRenderer bulletTracer;
    public ParticleSystem hitEffectPrefab;
}
