using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInfo : MonoBehaviour {

    public List<GameObject> meshes;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


		
	}

    public int[] getMeshValues(Vector3 vinePos) {

        float dist;
        float min = 10000000;
        int index = 0;
        int zBias;
        int[] values = new int[2];

        Vector3 DirectionVector;
        for (int i = 0; i < meshes.Count; i++) {

                dist = Mathf.Sqrt(Mathf.Pow((vinePos.x - meshes[i].transform.position.x), 2) + Mathf.Pow((vinePos.y - meshes[i].transform.position.y), 2) + Mathf.Pow((vinePos.z - meshes[i].transform.position.z), 2));
                if (dist < min) {

                    min = dist;
                    index = i;
              
                }
            }

       
        DirectionVector = vinePos - meshes[index].transform.position;
        zBias = -1 * (int)Mathf.Clamp(DirectionVector.z, -1, 1);
        values[0] = zBias;
        values[1] = index;

        return values;

    }


    public float distanceInXfromMesh(int index, Vector3 vinePos) {

        float distance = 0;

        distance = meshes[index].transform.position.x - vinePos.x;

        return distance;

    }



}
