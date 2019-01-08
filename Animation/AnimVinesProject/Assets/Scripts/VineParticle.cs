using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineParticle : MonoBehaviour {

	// public variables
	public Vector3 pivot;

	// private variables
	private ParticleSystem ps;
	private ParticleSystemRenderer psr;

	// properties
	ParticleSystem particleSystem {
		get {
			if (ps == null) {
				ps = GetComponent<ParticleSystem>();
			}
			return ps;
		}
	}

	ParticleSystemRenderer particleSystemRenderer {
		get {
			if (psr == null) {
				psr = GetComponent<ParticleSystemRenderer>();
			}
			return psr;
		}
	}

	// Use this for initialization
	void Start () {
        particleSystemRenderer.pivot = pivot;
	}
	
	// Update is called once per frame
	void Update () {
        // 
	}
}
