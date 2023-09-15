using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Coordinate[] Coordinates =
    {
        new Coordinate(10,0, 0)
    };
    
    public GameObject[] itemPrefabs;
    
    void Start()
    {
        GameObject CoinPrefab = GetItem(Items.Coin);
        GameObject Accelation = GetItem(Items.Accelation);
        GameObject Magnetic = GetItem(Items.Magnetic);
        
        GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        Vector3 itemPostion = Coordinates[Random.Range(0, Coordinates.Length)].GetPosition();
        SpawnItem(itemPrefab, itemPostion);
    }

    public void SpawnItem(GameObject itemPrefab, Vector3 position)
    {
        GameObject item = Instantiate(itemPrefab);
        item.transform.position = position;
    }
    
    GameObject GetItem(Items item)
    {
        return itemPrefabs[(int)item];
    }
    
    public struct Coordinate
    {
        public int x;
        public int y;
        public int z;

        public Coordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(x, y, z);
        }
    }
}

public enum Items
{
    Coin, Accelation, Magnetic
}
