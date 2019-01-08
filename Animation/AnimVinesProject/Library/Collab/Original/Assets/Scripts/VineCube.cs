using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineCube : MonoBehaviour
{
    protected bool IsA = false;
    float growthSpeed = 1;
    public GameObject nextVine;
    public static int maxNumVines = 400;    //Change here for num of vines
    public float rotatorValue = 15.0f;
    public float VineLenght = 6;
    public float bendingVal = 4;
    public float branchingFactor = 0.08f;
    public float GrowingSpeed = 1.0f;
    VineCube parent;
    VineCube[] children;
    VineCube current;
    MeshInfo meshInfo;
    int indexOfClosestMesh;
    float distanceXofMesh;
    int zBias;
    int[] meshValues = new int[2];
    int NumOfParents;
    public static int CurrentNumOfNode = 0;


    bool hasCreatedBraches = false;
    bool hasColided = false;
    public bool IsColliding = false;

    protected float angleX = 0.0f;
    protected float angleZ = 0.0f;
    protected float chanceToBranch = 1.0f;

    float startingScale;
    float scaler;



    // NOTE: it shouldn't be a child...
    // FIXME: don't make me a child
    private ParticleSystem _CachedParticleSystem;
    ParticleSystem particleSystem {
        get {
            if (_CachedParticleSystem == null) {
                _CachedParticleSystem = GetComponentInChildren<ParticleSystem>();
            }
            return _CachedParticleSystem;
        }
    }

	void Start ()
    {
        if (parent == null)
        {
            meshInfo = GameObject.Find("meshes").GetComponent<MeshInfo>();
            meshValues = meshInfo.getMeshValues(transform.position);
            zBias = meshValues[0];
            indexOfClosestMesh = meshValues[1];
            NumOfParents = 0;
        }

        else
        {
            zBias = parent.zBias;
            indexOfClosestMesh = parent.indexOfClosestMesh;
            meshInfo = parent.meshInfo;
            NumOfParents = parent.NumOfParents + 1;
            CurrentNumOfNode = parent.NumOfParents + 1;
        }


        distanceXofMesh = meshInfo.distanceInXfromMesh(indexOfClosestMesh, transform.position);       
        children = new VineCube[2];
        current = GetComponent<VineCube>();
        Vector3 vinePos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        growthSpeed = Random.Range(.6f, 1.4f) * GrowingSpeed;
        transform.localScale = new Vector3(transform.localScale.x, .01f, transform.localScale.z);
        startingScale = transform.localScale.y;
     
    }
	
	void Update ()
    {
        //Debug.Log(angleX);

        scaler = growthSpeed * Time.deltaTime;

        if (transform.localScale.y < startingScale + VineLenght)
            transform.localScale += new Vector3(0, scaler * 2, 0);
        else if (!hasCreatedBraches && !IsColliding && maxNumVines > 0)
        {
            hasCreatedBraches = true;
            CreateNextBranches();

            // set the shape to the vine
            var sh = particleSystem.shape;
            sh.scale = transform.localScale;
            
            // start particle system
            particleSystem.Play();
        }

        if(!hasColided && !hasCreatedBraches && angleX < 180)   //Bends toward the ground
        {
            transform.Rotate(Vector3.right * bendingVal * zBias * Time.deltaTime, Space.World);
            angleX += bendingVal * Time.deltaTime;
        }

        if(IsColliding && transform.rotation.x > 0.001f) //Rotate out of collider
        {
            transform.Rotate(Vector3.left * bendingVal * Time.deltaTime, Space.World);
            angleX -= bendingVal * Time.deltaTime;
        }

        if ((CurrentNumOfNode-NumOfParents) > 3) {

            Debug.Log("Current# nodes: " + CurrentNumOfNode);
            Debug.Log("Destroy parent # :" + NumOfParents);
            Destroy(current);

        }
            
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Collider")
        {
            hasColided = true;
            IsColliding = true;
        } 
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Collider")
        {
            IsColliding = false;
        }
    }

    void CreateNextBranches()
    {
        float rotationZ = angleZ * Mathf.Deg2Rad;
        float rotationX = angleX * Mathf.Deg2Rad;

        Vector3 position = new Vector3(transform.position.x + (transform.localScale.y * -(Mathf.Sin(rotationZ))), transform.position.y + (transform.localScale.y * (Mathf.Cos(rotationZ) * Mathf.Cos(rotationX))), transform.position.z + (transform.localScale.y * zBias * (Mathf.Sin(rotationX) * Mathf.Cos(rotationZ))));

        if (IsA)
        {
            if(Random.value + chanceToBranch >= 1.0f)
            {
                maxNumVines--;
                float rotationValue = Random.Range(-rotatorValue, rotatorValue); //Turn right left
                GameObject vineObject = Instantiate(nextVine, position, transform.rotation * Quaternion.Euler(0, 0, rotationValue));  //Creates Vine A
                VineCube v1 = vineObject.GetComponent<VineCube>();
                v1.IsA = true;
                v1.chanceToBranch = chanceToBranch - branchingFactor;
                v1.angleX = angleX; // know the rotation
                v1.angleZ = angleZ + rotationValue; // know the rotation 
                children[0] = v1; //Is child 1 of current game object
                v1.setParent(current); //Set Parent of Child 1 to current game object
            }

            maxNumVines--;
            float rotationValue2 = Random.Range(-rotatorValue, rotatorValue);
            GameObject vineObject2 = Instantiate(nextVine, position , transform.rotation * Quaternion.Euler(0, 0, rotationValue2));               //Creates Vine B
            VineCube v2 = vineObject2.GetComponent<VineCube>();
            v2.chanceToBranch = chanceToBranch - branchingFactor;
            v2.angleX = angleX;
            v2.angleZ = angleZ + rotationValue2;
            children[1] = v2; //Is child 1 of current game object
            v2.setParent(current); //Set Parent of Child 1 to current game object
        }
        else
        {
            maxNumVines--;
            float rotationValue = Random.Range(-rotatorValue, rotatorValue);
            GameObject vineObject = Instantiate(nextVine, position, transform.rotation * Quaternion.Euler(0, 0, rotationValue));  //Creates Vine A
            VineCube v1 = vineObject.GetComponent<VineCube>();
            v1.IsA = true;
            v1.chanceToBranch = chanceToBranch - branchingFactor;
            v1.angleX = angleX;
            v1.angleZ = angleZ + rotationValue;
            children[0] = v1; //Is child 1 of current game object
            v1.setParent(current); //Set Parent of Child 1 to current game object
        }

        if (children[0] != null) { }
        if (children[1] != null) { }

    }

    public void setParent(VineCube p) {

        parent = p;

    }

    public void RotateParent(Vector3 rot) {

        float rotationValue2 = Random.Range(-rotatorValue, rotatorValue);
        angleX = angleX;
        angleZ = angleZ + rotationValue2;

    }

}
