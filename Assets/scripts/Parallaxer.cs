using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Parallaxer : MonoBehaviour
{
    class PoolObject {
        public Transform transform;
        public bool inUse;

        //Constructor with Transform t
        public PoolObject(Transform t) { 
            transform = t;
        }

        //Use the PoolObject
        public void Use() {
            inUse = true;
        }

        //Dispose the PoolObject
        public void Dispose() {
            inUse = false;
        }
    }

    [System.Serializable]
    public struct YSpawnRange {  //y range of pipes
        public float minY;
        public float maxY;
    }

    //Pipes sử dụng Pipe Prefab, Clouds -> clouds Prefab, Stars -> stars Prefab
    public GameObject Prefab; 
    public int poolSize; //Bao nhiêu pipes/clouds/stars dc sinh ra
    public float shiftSpeed; //shifting speed
    public float spawnRate; //How often objects are spawning (sinh ra)
    
    public YSpawnRange ySpawnRange; //chứa y range of pipes (minY,maxY)
    public Vector3 defaultSpawnPos; //Default positon to spawn
    
    //Spawn at the start: prewarm
    //true for clouds and stars, false for pipes
    public bool spawnImmediate; 
    public Vector3 immediateSpawnPos; //Position for immediate spawn
    
    //To handle different screen sizes 
    public Vector2 targetAspectRatio; //(width, height)

    float spawnTimer; //Keep track spawning moment
    float targetAspect; // width/height = 10/16
    PoolObject[] poolObjects; //array of poolObjects

    //A reference to GameManager
    GameManager game;

    void Awake() {
        Configure();
    }
    
    //Start() is called with await
    //So everything is initialized before get started
    void Start() {
        game = GameManager.Instance;
    }

    void OnEnable() {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable() {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed() {
        for (int i = 0 ; i < poolObjects.Length ; i++) {
            poolObjects[i].Dispose();
            //Move it somewhere off-screen
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
        if (spawnImmediate) {
            SpawnImmediate();
        }
    }

    void Update() {
        if (game.GameOver)
            return;

        Shift();
        spawnTimer += Time.deltaTime;
        //Sau 1 khoảng t/g spawnRate, 1 pipe dc sinh thêm
        if (spawnTimer > spawnRate) {
            Spawn();  //Sinh thêm pipe
            spawnTimer = 0;
        }
    }

    //Instantiating things: spawning pool objects
    void Configure() {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0 ; i < poolObjects.Length ; i++) {
            //Instantiate and cast to GameObject
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            //t is set a parent of whatever object the script is attached to
            t.SetParent(transform);
            //Must use localPosition
            t.position = Vector3.one * 1000; //off-screen
            //t.localScale = Vector3.one;

            //Use t to instantiate the poolObjects[i]
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate) {
            SpawnImmediate();
        }
    }

    //Moving pool objects into place
    void Spawn() {
        //Get the first available poolObject
        Transform t = GetPoolObject();
        if (t == null) 
            return;

        Vector3 pos = Vector3.zero;
        //Place at defaultSpawnPos.x
        if (spawnImmediate) 
            pos.x = defaultSpawnPos.x; //for Clouds and Stars
        else
            pos.x = defaultSpawnPos.x * Camera.main.aspect / targetAspect; //for Pipes
        pos.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
        //Set position for t
        t.position = pos; 
    }

    //Spawn 2 objects: 1 at the immediate position, 1 at default spawning position
    void SpawnImmediate() {
        //Get the first available poolObject
        Transform t = GetPoolObject();
        if (t == null) 
            return;

        Vector3 pos = Vector3.zero;
        //Place at immediateSpawnPos.x
        pos.x = immediateSpawnPos.x * Camera.main.aspect / targetAspect; 
        pos.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
        //Set position for t
        t.position = pos;

        Spawn();
    }

    void Shift() {
        for (int i = 0 ; i < poolObjects.Length ; i++) {
            //Vector3.right: (1, 0, 0)
            //-Vector3.right --> shift left
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }

    void CheckDisposeObject(PoolObject poolObject) {
        //Use aspect to suit all screen sizes
        if (poolObject.transform.position.x < -defaultSpawnPos.x * Camera.main.aspect / targetAspect) {
            poolObject.Dispose();
            
            //Move it somewhere off-screen
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    //Get unused 1 pool object
    Transform GetPoolObject() {
        for (int i = 0 ; i < poolObjects.Length ; i++) {
            if (!poolObjects[i].inUse) {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }
}
