using Mirror;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltarChase.LevelGen
{   
    public class LevelTile : NetworkBehaviour
    {
        [SerializeField] private TileConnectorData connectorData = new TileConnectorData(TileShape.Cap);
        [SerializeField] private Vector3 spawnPosition = Vector3.zero;
        [SerializeField] private float spawnRotation = 0;
        
        public int QuaterRots
        {
            set
            {
                transform.rotation = Quaternion.Euler(0, (value % 4) * 90,0);
            }
        }

        public TileConnectorData ConnectorData => connectorData;
        public List<Vector3Int> DefaultConnectorPositions() => connectorData.DefaultConnectorPositions();
        public int NumberOfConnections() => connectorData.NumberOfConnections();

        public void SpawnObject(GameObject _gameObject)
        {
            GameObject spawnedGO = Instantiate(_gameObject, transform.position + transform.rotation * spawnPosition, Quaternion.Euler(0,transform.rotation.y + spawnRotation,0), transform);
            NetworkServer.Spawn(spawnedGO);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach(Vector3Int connectorPosition in connectorData.DefaultConnectorPositions())
            {
                Gizmos.DrawLine(transform.position, transform.position + transform.rotation * (connectorPosition * 2));
            }
            Gizmos.color = Color.blue;
            
            Vector3 spawnGizmoPos = transform.position + transform.rotation * spawnPosition;
            Gizmos.DrawSphere(spawnGizmoPos, 0.25f);
            Gizmos.DrawLine(spawnGizmoPos, transform.rotation * Quaternion.Euler(0,spawnRotation,0) * Vector3.forward + spawnGizmoPos);
        }
    }
}
