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
    public float xBiasInfluence = 2;
    VineCube parent = null;
    VineCube[] children = null;
    VineCube current = null ;
    public static Mesh mesh;
    MeshInfo meshInfo = null;
    int indexOfClosestMesh;
    float distanceXofMesh;
    int zBias;
    int[] meshValues = new int[2];
    int NumOfParents;
    public static int CurrentNumOfNode = 0;
    public bool ChildIsMovingUp = false;
    public bool ChildIsMovingDown = false;

    bool hasCreatedBraches = false;
    bool hasColided = false;
    public bool IsColliding = false;

    protected float angleX = 0.0f;
    protected float angleZ = 0.0f;
    protected float chanceToBranch = 1.0f;
    public float startingRot, newRot, childrenRotationInfluence;

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
            mesh = new Mesh();
            meshInfo = GameObject.Find("meshes").GetComponent<MeshInfo>();
            meshValues = meshInfo.getMeshValues(transform.position);
            zBias = meshValues[0];
            indexOfClosestMesh = meshValues[1];
            NumOfParents = 0;
            // mesh = gameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        else
        {
            zBias = parent.zBias;
            indexOfClosestMesh = parent.indexOfClosestMesh;
            meshInfo = parent.meshInfo;
            NumOfParents = parent.NumOfParents + 1;
            CurrentNumOfNode = parent.NumOfParents + 1;
        }

        startingRot = transform.rotation.x;
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

        float rotationZ = angleZ * Mathf.Deg2Rad;
        float rotationX = angleX * Mathf.Deg2Rad;
        Vector3 position = new Vector3(transform.position.x + (transform.localScale.y * -(Mathf.Sin(rotationZ))), transform.position.y + (transform.localScale.y * (Mathf.Cos(rotationZ) * Mathf.Cos(rotationX))), transform.position.z + (transform.localScale.y * zBias * (Mathf.Sin(rotationX) * Mathf.Cos(rotationZ))));

        if (children[0] != null) {

            children[0].transform.position = position;

        }

        if (children[1] != null)
        {
            children[1].transform.position = position;
        }

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
            startingRot = transform.rotation.x;
            transform.Rotate(Vector3.right * bendingVal * zBias * Time.deltaTime, Space.World);
            angleX += bendingVal * Time.deltaTime;
            newRot = transform.rotation.x;
            RotateParents(3, false);
        }

        if(IsColliding && transform.rotation.x > 0.001f) //Rotate out of collider
        {
            startingRot = transform.rotation.x;
            transform.Rotate(Vector3.left * bendingVal * Time.deltaTime, Space.World);
            angleX -= bendingVal * Time.deltaTime;
            newRot = transform.rotation.x;
            RotateParents(3, true);
        }

        if (ChildIsMovingUp) {

             transform.Rotate(Vector3.left * bendingVal * Time.deltaTime, Space.World);
             angleX -= bendingVal * Time.deltaTime;
             startingRot = transform.rotation.x;
             childrenRotationInfluence = 6;
             ChildIsMovingUp = false;
        }

        if (ChildIsMovingDown)
        {

            transform.Rotate(Vector3.right * bendingVal * zBias * Time.deltaTime, Space.World);
            angleX += bendingVal * Time.deltaTime;
            startingRot = transform.rotation.x;
            childrenRotationInfluence = 6;
            ChildIsMovingDown = false;
        }

        if ((CurrentNumOfNode - NumOfParents) > 5)
        {
            Destroy(current);
           // CombineInstance[] combines = new CombineInstance[2];
           // combines[0].mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
          //  combines[0].transform = gameObject.transform.localToWorldMatrix;
           // combines[1].mesh = mesh;
            

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

        float BiasX = distanceXofMesh / xBiasInfluence;

        //Debug.Log(BiasX);

        float rotationValue = Random.Range(-rotatorValue - BiasX, rotatorValue - BiasX); //Turn right left

        if (IsA)
        {
            float rotationValue2 = Random.Range(-rotatorValue, rotatorValue);
            if (Random.value + chanceToBranch >= 1.0f)
            {
                maxNumVines--;
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
            GameObject vineObject = Instantiate(nextVine, position, transform.rotation * Quaternion.Euler(0, 0, rotationValue));  //Creates Vine A
            VineCube v1 = vineObject.GetComponent<VineCube>();
            v1.IsA = true;
            v1.chanceToBranch = chanceToBranch - branchingFactor;
            v1.angleX = angleX;
            v1.angleZ = angleZ + rotationValue;
            children[0] = v1; //Is child 1 of current game object
            v1.setParent(current); //Set Parent of Child 1 to current game object
        }

    }

    public void setParent(VineCube p) {

        parent = p;
        //Debug.Log(parent.name);

    }

    public void RotateParents(int index, bool sens) {

        float rot = newRot - startingRot;

        if (parent == null) {
           // Debug.Log("NULL PARENT");
        }

        if (parent!=null && index > 0) {

           
            Debug.Log(sens);
            if (sens)
            {
                Debug.Log(sens);
                parent.childrenRotationInfluence = childrenRotationInfluence / 4;
                parent.ChildIsMovingUp = true;
            }
            else
            {
                parent.childrenRotationInfluence = childrenRotationInfluence / 8;
                parent.ChildIsMovingDown = true;
            }

            index--;
            parent.RotateParents(index, sens);

        }       

    }

}
