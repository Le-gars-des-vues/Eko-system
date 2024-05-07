using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
	ParticleSystem ps;
	[SerializeField] bool isCasino;

	void Start()
	{
		ps = GetComponent<ParticleSystem>();
	}

	void Update()
	{
		if (ps)
		{
			if (!ps.IsAlive())
			{
				if (isCasino)
					gameObject.SetActive(false);
				else
					Destroy(gameObject);
			}
		}
	}
}
