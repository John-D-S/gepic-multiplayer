using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltarChase.LevelGen
{
    public class LevelTile : MonoBehaviour
    {
        [SerializeField, Tooltip("The size of the tile.")] private float tileWidth;

        private TileConnectorData connectorData;

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach(Vector3Int connectorPosition in connectorData.DefaultConnectorPositions())
            {
                Gizmos.DrawLine(transform.position, transform.position + transform.rotation * (connectorPosition * 2));
            }
        }
    }
}
