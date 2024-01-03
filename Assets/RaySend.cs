using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaySend : MonoBehaviour
{
    public float rayDistance = 2f;
    
    public List<GameObject> Cabbar = new List<GameObject>();

    private Hex _hex;
    public float lerpSpeed = 0.1f;

    private void Start()
    {
       _hex= FindObjectOfType<Hex>();
        //SendRay();
       
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Hexagon"))
                {
                    // Tıklanan nesne "objem" tag'ine sahip
                    Debug.Log("Hex tag'ine sahip bir nesneye tıklandı.");
                   
                    hit.collider.GetComponent<Renderer>().material.color = Color.yellow;

                    _hex.transformList.Remove(hit.collider.transform);
                    _hex.EdgeList.Remove(hit.collider.transform);
                    SendRay();
                }
            }
        }
    }
    
   public void SendRay()
    {
        Cabbar.Clear();
        
        Vector3 startPoint = transform.position;

        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f+30;
            angle = angle * Mathf.PI / 180f;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

            Ray ray = new Ray(startPoint, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.collider.gameObject)
                {
                    Cabbar.Add(hit.collider.gameObject);
                }
                Debug.DrawLine(startPoint, hit.point, Color.red);
            }
            else
            {
                // Ray'in uzunluğu kadar bir çizgi çizin
               Debug.DrawLine(startPoint, startPoint + direction * rayDistance, Color.green);
            }
        }    // Remove yellow tiles from Cabbar

            Cabbar.RemoveAll(tile => tile.GetComponent<Renderer>().material.color == Color.yellow);
if (Cabbar.Count == 0)
    {
        // All surrounding tiles are yellow, take appropriate actions (e.g., end the game)
        Debug.Log("Game Over - All surrounding tiles are yellow!");
                Time.timeScale = 0f;  // Pause time

        // Add your game-ending logic here
    }
        _hex.FindClosest();
        FindNeighbourPath();
        Movement();
    }

   private GameObject closestObject;
   public void FindNeighbourPath()
    {
        // Başlangıçta en yakın olan oyun nesnesini null olarak tanımlayın
        GameObject closestObject = null;

        float closestDistance = Mathf.Infinity;

        // neighbour listesindeki tüm oyun nesnelerinin Transform bileşenlerini alı
        foreach (GameObject obj in Cabbar)
        {
            Transform objTransform = obj.transform;
            // Bu nesne ile arasındaki mesafeyi hesaplayın
            float distance = Vector3.Distance(objTransform.position, _hex.closestTile.transform.position);

            // Eğer bu nesne, şu ana kadar en yakın nesneden daha yakınsa, en yakın nesne olarak bu nesneyi seçin
            if (distance < closestDistance)
            {
                closestObject = obj;
                closestDistance = distance;
            }
        }
       
        // En yakın nesnenin Transform bileşenini a değişkenine atayın
        if (closestObject != null)
        {
            Transform closestTransform = closestObject.transform;
            _hex.closestTile = closestTransform;
        }
        Debug.Log("CLOSEST NEİGH"+ _hex.closestTile);
    } //Find closest metoduna göre gidilmesi gereken yolu bulur

   void Movement()//closestile a göre gidiyor
   {
       if (_hex.closestTile != null)
       {
           Debug.Log("yoldaaa");
           // En yakın noktanın pozisyonunu al
           Vector3 targetPosition = _hex.closestTile.position;
           Debug.Log(" _hex.closestTile.position: "+ _hex.closestTile.position);
           // Transform'unu yavaşça hedef pozisyona doğru hareket ettir
           Vector3 targetPosition2 = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
           transform.position = targetPosition2;

         // Check if the player is on an edge tile
        if (IsPlayerOnEdgeTile(transform.position))
        {
            Debug.Log("Player is on an edge tile. Game paused.");
            Time.timeScale = 0f;  // Pause time
        }
       }
   }
bool IsPlayerOnEdgeTile(Vector3 playerPosition)
{
    foreach (Transform edgeTile in _hex.EdgeList)
    {
        // Check if the player's position is close to the edge tile's position
        if (Vector3.Distance(playerPosition, edgeTile.position) < 1f)
        {
            return true;
        }
    }

    return false;
}
}
