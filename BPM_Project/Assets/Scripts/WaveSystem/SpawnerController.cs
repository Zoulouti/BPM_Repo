using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;
using Sirenix.OdinInspector;

public class SpawnerController : SerializedMonoBehaviour
{
    //public WavesList[] _nbrOfWaves;
    //[System.Serializable]
    //public class WavesList
    //{
    //    [System.Serializable]
    //    public class TypeOfEnemy
    //    {
    //        public EnemyArchetype archetype;
    //        public EnemyType enemy;
    //    }
    //}

    //public TypeOfEnemy[] m_enemyToSummon;
    //[System.Serializable]
    //public class TypeOfEnemy
    //{
    //    public EnemyArchetype archetype;
    //}

    EnemyType enemy = EnemyType.EnemyBase;

    public Dictionary<int, EnemyArchetype[]> waveManager = new Dictionary<int, EnemyArchetype[]>();

    ObjectPooler m_objectPooler;

    private void Start()
    {
        m_objectPooler = ObjectPooler.Instance;

        //Debug.Log(waveManager[0].GetValue(0));
    }
    

    public IEnumerator WaveSpawner(int wave, WaveController controller)
    {
        for (int a = 0, f = waveManager[wave].Length; a < f; ++a)
        {
            if (waveManager.ContainsKey(wave))
            {
                yield return new WaitForSeconds(controller.timeBetweenEachSpawn);

                GameObject go = m_objectPooler.SpawnEnemyFromPool(enemy, transform.position, transform.rotation);

                EnemyCara cara = go.GetComponent<EnemyCara>();
                cara.EnemyArchetype = waveManager[wave].GetValue(a) as EnemyArchetype;

                Spawned_Tracker tracker = go.AddComponent<Spawned_Tracker>();
                tracker.Controller = controller;
                controller.NbrOfEnemy++;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
