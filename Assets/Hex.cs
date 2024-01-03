using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    [SerializeField] private GameObject prefabToInstantiate; //Instantiate edeceğimiz objenin prefab'ını bu değişkene atayacağız.
    public GameObject hexTilePrefab;
    
    private List<GameObject> changedTiles = new List<GameObject>(); // Değiştirilen tile'ları tutmak için liste oluşturuyoruz.
    private List<GameObject> hexTiles = new List<GameObject>(); // Liste oluşturuyoruz.

    
    public List<Transform> transformList = new List<Transform>(); // Transform bilgilerini tutmak için liste oluşturuyoruz.
    public List<Transform> EdgeList = new List<Transform>();

    
    [SerializeField] private int mapWidth=26;
    [SerializeField] private int mapHeight=12;
    private float tileXOffset = 1.6f;
    private float tileZOffset = .7f;


    
    public Transform closestTile;
    private RaySend _raySend;
    public void Start()
    {
        _raySend= FindObjectOfType<RaySend>();
        
        CreateHexTileMap();//1 kere
        
        ChangeRandomTileColor(); // 1 kere Rastgele bir tile'ın rengini değiştiriyoruz.

        EdgeRegulation(); //1 kere boyananları listeden cıkardık 
       
        InstantiateOnTile(); //1 kez
        
       // FindClosest(); //cok kez //cıkıs için en yakın cıkıs kapısı bulunur
        

    }

    void CreateHexTileMap()
    {
        for (int x = 0; x <= mapWidth; x++)
        {
            for (int z = 0; z <= mapHeight; z++)
            {
                GameObject TempGo = Instantiate(hexTilePrefab);

                if (z % 2 == 0)
                {
                    TempGo.transform.position = new Vector3(x * tileXOffset, 0, z * tileZOffset);
                }
                else
                {
                    TempGo.transform.position = new Vector3(x * tileXOffset + tileXOffset / 2, 0, z * tileZOffset);
                }
                SetTileInfo(TempGo, x, z);

                hexTiles.Add(TempGo); // Oluşturulan tile'ı listeye ekliyoruz.
                transformList.Add(TempGo.transform); // Oluşturulan tile'ın transform bilgisini listeye ekliyoruz.
            }
        }
    }

    void SetTileInfo(GameObject GO, int x, int z)
    {
        GO.transform.parent = transform;
        GO.name = x.ToString() + " , " + z.ToString();

        GO.GetComponent<Edges>().Position = new Vector2(x, z);

        FindEdges();
    }
    void FindEdges()

    //deneme123
    {
        float minX =0;
        float minZ = 0;
        float maxX = mapWidth;
        float maxZ = mapHeight;

        foreach(Transform child in transform)
        {
            Vector2 pos = child.GetComponent<Edges>().Position;
            minX = Mathf.Min(minX, pos.x);
            minZ = Mathf.Min(minZ, pos.y);
            maxX = Mathf.Max(maxX, pos.x);
            maxZ = Mathf.Max(maxZ, pos.y);
        }

        EdgeList.Clear();
        
        foreach(Transform child in transform)
        {
            //Debug.Log("AA");
            Vector2 pos = child.GetComponent<Edges>().Position;

            float isDouble = pos.y % 2;
                    
            if (pos.x ==0 && isDouble==0)
            {
                if (!EdgeList.Contains(child))
                    EdgeList.Add(child);
                // break;
            }
            if (pos.x ==maxX && isDouble==1)
            {
                if (!EdgeList.Contains(child))
                    EdgeList.Add(child);
            }
            if (pos.y ==0 )
            {
                if (!EdgeList.Contains(child))
                    EdgeList.Add(child);
            }
            if (pos.y ==maxZ )
            {
                if (!EdgeList.Contains(child))
                    EdgeList.Add(child);
            }
        }
    }
    void ChangeRandomTileColor()
    {
        for (int i = 0; i < 12; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, hexTiles.Count); // Rastgele bir tile seçiyoruz.
            GameObject randomTile = hexTiles[randomIndex];
            randomTile.GetComponent<Renderer>().material.color = Color.yellow; // Rastgele tile'ın rengini sarı yapıyoruz.
          //  Debug.Log("Değiştirilen Tile: " + randomTile.name); // Seçilen tile'ın adını konsola yazdırıyoruz.

            changedTiles.Add(randomTile); // Değiştirilen tile'ın referansını listede tutuyoruz.

            // Değiştirilen tile'ın transform bilgilerini içeren transformList listesinden kaldırıyoruz.
            for (int j = 0; j < transformList.Count; j++)
            {
                if (transformList[j] == randomTile.transform)
                {
                    transformList.RemoveAt(j);
                    break;
                }
            }
        }
    }

    void EdgeRegulation()
    {
        for (int i = 0; i < EdgeList.Count; i++)
        {
            if (changedTiles.Contains(EdgeList[i].gameObject))
            {
                EdgeList.RemoveAt(i);
                i--;
            }
        }
    }//boyanan köşeleri EdgeListten çıkardık
    
    private GameObject newObject;
    void InstantiateOnTile()
    {
        Vector3 targetPosition = GameObject.Find("3 , 3").transform.position; // Hedef pozisyonu "2,2" objesinin pozisyonu olarak alıyoruz.
        targetPosition = new Vector3(targetPosition.x, 0.244f, targetPosition.z);
         newObject= Instantiate(prefabToInstantiate, targetPosition, Quaternion.identity); // Yeni objeyi hedef pozisyona instantiate ediyoruz.
    }//kediyi koy

    public void FindClosest() //cıkıs için en yakın cıkıs kapısı bulunur
    {
        Vector3 targetPosition = newObject.transform.position;
        float minDistance = float.MaxValue;
        closestTile = null;

        // EdgeList içindeki her bir tile'ın pozisyonunu alın.
        foreach (Transform tile in EdgeList)
        {
          
            float distance = Vector3.Distance(tile.position, targetPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTile = tile;
                
            }
        }   
        Debug.Log(" en yakını" + closestTile);
       
    }
    
    
}