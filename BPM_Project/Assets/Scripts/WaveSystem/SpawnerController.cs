using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class SpawnerController : MonoBehaviour
{
    public WavesList[] _nbrOfWaves;
    [System.Serializable]
    public class WavesList
    {
        public TypeOfEnemy[] m_enemyToSummon;
        [System.Serializable]
        public class TypeOfEnemy
        {
            public EnemyArchetype archetype;
            public EnemyType enemy;
        }
    }

    ObjectPooler m_objectPooler;

    private void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
    }


    public IEnumerator WaveSpawner(int i, int wave, WaveController controller)
    {
        for (int a = 0, f = _nbrOfWaves[wave].m_enemyToSummon.Length; a < f; ++a)
        {
            yield return new WaitForSeconds(controller.timeBetweenEachSpawn);
            //GameObject go = Instantiate(_nbrOfWaves[wave].m_enemyToSummon[a], transform);
            GameObject go = m_objectPooler.SpawnEnemyFromPool(_nbrOfWaves[wave].m_enemyToSummon[a].enemy, transform.position, transform.rotation);
            Spawned_Tracker tracker = go.AddComponent<Spawned_Tracker>();
            tracker.Controller = controller;
            controller.NbrOfEnemy++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
