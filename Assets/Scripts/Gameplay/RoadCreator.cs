using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plane.Gameplay
{
    public class RoadCreator : MonoBehaviour
    {
        public static RoadCreator m_Current;

        public GameObject[] m_RoadPartPrefabs;
        public GameObject[] m_ObjectPackPrefabs;
        public GameObject[] m_ItemPackPrefabs; 

        [HideInInspector]
        public RoadPart m_LastPart;
        [HideInInspector]
        public ObstaclePack m_LastObstacle;
        [HideInInspector]
        public ItemPack m_LastItem; 
        
        [HideInInspector]
        public List<ObstaclePack> m_Obstacles;

        [HideInInspector]
        public int ObstacleCounter = 0;
        [HideInInspector]
        public int ItemCounter = 0;

        [Header("Coin Spawn Lanes")]
        [Tooltip("Atur posisi X dan Y untuk spawn koin. (Nilai Z akan diabaikan)")]
        public Vector3[] coinLanes = new Vector3[]
        {
            new Vector3(-12f, 10f, 0f),  // Bawah Kiri
            new Vector3(0f, 10f, 0f),    // Bawah Tengah
            new Vector3(12f, 10f, 0f),   // Bawah Kanan
            new Vector3(-12f, 22f, 0f),  // Atas Kiri
            new Vector3(0f, 22f, 0f),    // Atas Tengah
            new Vector3(12f, 22f, 0f)    // Atas Kanan
        };

        void Awake()
        {
            // PENGAMAN 1: Mencegah infinite loop jika prefab ini mengandung dirinya sendiri
            if (m_Current != null && m_Current != this)
            {
                Destroy(gameObject);
                return;
            }
            m_Current = this;
        }

        void Start()
        {
            m_Obstacles = new List<ObstaclePack>();

            RoadPart last = null;
            for (int i = 0; i < 10; i++)
            {
                // PENGAMAN 2: Cek apakah array prefab jaman kosong
                if (m_RoadPartPrefabs.Length == 0) break;

                GameObject obj = Instantiate(m_RoadPartPrefabs[0]);
                RoadPart p = obj.GetComponent<RoadPart>();

                // PENGAMAN 3: Cek apakah prefab tidak punya script RoadPart
                if (p == null)
                {
                    Debug.LogError("Prefab jalan tidak memiliki script RoadPart!");
                    Destroy(obj);
                    continue;
                }

                if (i == 0)
                {
                    obj.transform.position = Vector3.zero;
                }
                else if (last != null)
                {
                    obj.transform.position = last.EndPoint.position;
                }

                if (last != null)
                {
                    last.m_NextPart = p;
                }
                last = p;
                m_LastPart = last;
            }

            // Spawn Obstacle Awal
            if (m_ObjectPackPrefabs.Length > 0)
            {
                int r = Random.Range(0, m_ObjectPackPrefabs.Length);
                GameObject obj1 = Instantiate(m_ObjectPackPrefabs[r]);
                obj1.transform.position = new Vector3(0, 0, 500);
                m_LastObstacle = obj1.GetComponent<ObstaclePack>();
            }

            // Spawn Item Awal
            if (m_ItemPackPrefabs != null && m_ItemPackPrefabs.Length > 0)
            {
                int rItem = Random.Range(0, m_ItemPackPrefabs.Length);
                Vector3 startPos = GetRandomCoinLane(350f); 
                GameObject itemObj = Instantiate(m_ItemPackPrefabs[rItem], startPos, Quaternion.identity);
                m_LastItem = itemObj.GetComponent<ItemPack>();
            }
        }

        void Update()
        {
            // PENGAMAN 4: Hentikan update jika jalan belum siap
            if (m_LastPart == null) return;

            // INFINITE ROAD SPAWNING
            if (m_LastPart.transform.position.z < 200)
            {
                if (m_RoadPartPrefabs.Length > 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int r = Random.Range(0, m_RoadPartPrefabs.Length);
                        GameObject obj = Instantiate(m_RoadPartPrefabs[r]);
                        RoadPart p = obj.GetComponent<RoadPart>();

                        if (p == null) continue; // Lewati jika prefab error

                        obj.transform.position = m_LastPart.EndPoint.position;
                        m_LastPart.m_NextPart = p;
                        m_LastPart = p;
                    }
                }
            }

            // INFINITE OBSTACLE SPAWNING
            if (m_LastObstacle != null && m_LastObstacle.transform.position.z < 200)
            {
                m_LastObstacle = null;

                if (m_ObjectPackPrefabs.Length > 0)
                {
                    int r = Random.Range(0, m_ObjectPackPrefabs.Length);
                    GameObject obj1 = Instantiate(m_ObjectPackPrefabs[r], new Vector3(0, 0, 400), Quaternion.identity);
                    m_LastObstacle = obj1.GetComponent<ObstaclePack>();
                }
            }

            // INFINITE ITEM SPAWNING
            if (m_LastItem != null && m_LastItem.transform.position.z < 200)
            {
                m_LastItem = null;

                if (m_ItemPackPrefabs != null && m_ItemPackPrefabs.Length > 0)
                {
                    int rItem = Random.Range(0, m_ItemPackPrefabs.Length);
                    Vector3 spawnPos = GetRandomCoinLane(400f);
                    GameObject itemObj = Instantiate(m_ItemPackPrefabs[rItem], spawnPos, Quaternion.identity);
                    m_LastItem = itemObj.GetComponent<ItemPack>();
                }
            }
        }

        private Vector3 GetRandomCoinLane(float zPosition)
        {
            if (coinLanes == null || coinLanes.Length == 0)
            {
                return new Vector3(0, 10f, zPosition);
            }

            int laneIndex = Random.Range(0, coinLanes.Length);
            Vector3 lane = coinLanes[laneIndex];
            
            return new Vector3(lane.x, lane.y, zPosition);
        }
    }
}