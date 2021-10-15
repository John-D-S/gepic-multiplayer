using System.Collections.Generic;

using UnityEngine;

namespace AltarChase.LevelGen
{
	public class TempLevelTileData
	{
		public GameObject tileGameObject = null;
		public TileConnectorData connectorData;
		public Vector3Int position;
		public int quaterRots;
        
		public TempLevelTileData(TileConnectorData _connectorData, Vector3Int _position, int _quaterRots)
		{
			connectorData = _connectorData;
			position = _position;
			quaterRots = _quaterRots;
		}
        
		/// <summary>
		/// Returns the given position rotated by 90 degrees about the origin clockwise.
		/// </summary>
		public Vector3Int RotIntPosBy90(Vector3Int _pos) => new Vector3Int(_pos.z, _pos.y, -_pos.x);
        
		/// <summary>
		/// returns this tiles' connector positions relative to this tile, if it was rotated by _quaterRots
		/// </summary>
		public List<Vector3Int> ConnectorPositionsWithRotation(int _quaterRots)
		{
			List<Vector3Int> returnVal = connectorData.DefaultConnectorPositions();
			//modulo quater rots by 4 so that it stays between 0 and 3
			int actualQuaterRots = _quaterRots % 4;
			for(int i = 0; i < returnVal.Count; i++)
			{
				for(int j = 0; j < _quaterRots; j++)
				{
					returnVal[i] = RotIntPosBy90(returnVal[i]);
				}
			}
			return returnVal;
		}

		/// <summary>
		/// returns the positions of this tile's connection points on the grid
		/// </summary>
		public List<Vector3Int> ConnectorPositionsOnGrid()
		{
			List<Vector3Int> returnVal = connectorData.DefaultConnectorPositions();
			for(int i = 0; i < returnVal.Count; i++)
			{
				for(int j = 0; j < quaterRots; j++)
				{
					returnVal[i] = RotIntPosBy90(returnVal[i]);
				}
			}
			for(int i = 0; i < returnVal.Count; i++)
			{
				returnVal[i] += position;
			}
			return returnVal;
		}
        
		public bool TileFitsInPosition(Dictionary<Vector3Int, TempLevelTileData> _currentLevelTiles)
		{
			List<Vector3Int> connectorPositons = ConnectorPositionsOnGrid();
			foreach(Vector3Int connectorPosition in connectorPositons)
			{
				if(!_currentLevelTiles.ContainsKey(connectorPosition) || _currentLevelTiles[connectorPosition].ConnectorPositionsOnGrid().Contains(position))
				{
					continue;
				}
				return false;
			}
			return true;
		}

		public bool TileFitsInPositionWithNoEmptyConnectors(Dictionary<Vector3Int, TempLevelTileData> _currentLevelTiles)
		{
			List<Vector3Int> connectorPositons = ConnectorPositionsOnGrid();
			foreach(Vector3Int connectorPosition in connectorPositons)
			{
				if(!_currentLevelTiles.ContainsKey(connectorPosition))
				{
					return false;
				}
				if(_currentLevelTiles[connectorPosition].ConnectorPositionsOnGrid().Contains(position))
				{
					continue;
				}
				return false;
			}
			return true;
		}
	}
}