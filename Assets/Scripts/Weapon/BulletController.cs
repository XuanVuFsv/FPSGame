using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
	private static BulletController instance;

	public Transform casingPrefab;
	public ParticleSystem hitEffectPrefab;
	public TrailRenderer bulletTracer;
	[Range(10, 200)]
	public float damage;
	[Range(0, 100)]
	public float destroyAfterTime;
	public bool destroyOnImpact = false;
	public float minDestroyTime;
	public float maxDestroyTime;

	public Transform[] metalImpactPrefabs;

	[Header("Bullet Settings")]
	public float bulletForce = 400.0f;
	[Tooltip("How long after reloading that the bullet model becomes visible " +
		"again, only used for out of ammo reload animations.")]
	public float showBulletInMagDelay = 0.5f;
	[Tooltip("The bullet model inside the mag, not used for all weapons.")]
	public SkinnedMeshRenderer bulletInMagRenderer;

	private new Rigidbody rigidbody;
	private float initTime;

	void MakeInstance()
	{
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
		}
		else instance = this;
	}

	public static BulletController Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		MakeInstance();
	}

	private void Start()
	{
		initTime = Time.time;
		rigidbody = GetComponent<Rigidbody>();

		//rigidbody.velocity = transform.forward * bulletForce;
        //GetComponent<Rigidbody>().AddForce(transform.forward * bulletForce, ForceMode.Acceleration);

        //Start destroy timer
        //if (destroyAfterTime > 0) StartCoroutine(DestroyAfter());
		//Add velocity to the bullet
	}

	//If the bullet collides with anything
	private void OnCollisionEnter(Collision collision)
	{
		//Debug.Log(collision.collider.name);
		//Debug.Log(Time.time - initTime + " " + rigidbody.velocity);
		//If destroy on impact is false, start 
		//coroutine with random destroy timer
		if (!destroyOnImpact && destroyAfterTime == 0)
		{
            //rigidbody.velocity = Vector3.zero;
            //StartCoroutine(DestroyTimer());
        }
		//Otherwise, destroy bullet on impact
		else
		{
			//Destroy(gameObject);
		}

        {
			////If bullet collides with "Metal" tag
			//if (collision.transform.tag == "Metal")
			//{
			//	//Instantiate random impact prefab from array
			//	Instantiate(metalImpactPrefabs[Random.Range
			//		(0, metalImpactPrefabs.Length)], transform.position,
			//		Quaternion.LookRotation(collision.contacts[0].normal));
			//	//Destroy bullet object
			//	Destroy(gameObject);
			//}

			////If bullet collides with "Target" tag
			//if (collision.transform.tag == "Target")
			//{
			//	//Toggle "isHit" on target object
			//	collision.transform.gameObject.GetComponent
			//		<TargetScript>().isHit = true;
			//	//Destroy bullet object
			//	Destroy(gameObject);
			//}

			////If bullet collides with "ExplosiveBarrel" tag
			//if (collision.transform.tag == "ExplosiveBarrel")
			//{
			//	//Toggle "explode" on explosive barrel object
			//	collision.transform.gameObject.GetComponent
			//		<ExplosiveBarrelScript>().explode = true;
			//	//Destroy bullet object
			//	Destroy(gameObject);
			//}
		}
	}

	private IEnumerator DestroyTimer()
	{
		yield return new WaitForSeconds
			(Random.Range(minDestroyTime, maxDestroyTime));
		Destroy(gameObject);
	}

	private IEnumerator DestroyAfter()
	{
		yield return new WaitForSeconds(destroyAfterTime);
		Destroy(gameObject);
	}
}
