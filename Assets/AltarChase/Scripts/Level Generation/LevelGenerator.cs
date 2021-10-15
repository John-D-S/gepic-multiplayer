using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

using Random = UnityEngine.Random;

namespace AltarChase.LevelGen
{
    public class TempLevelTileData
    {

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

    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private int numerOfTiles; 
        
        [SerializeField] private List<LevelTile> levelTiles;

        //use network server.spawn for instantiation.
        //all modules should have a network identity and all module prefabs should be in registered spawnable prefabs in the network manager. 
        private Dictionary<Vector3Int, TempLevelTileData> levelTilesByGridPos = new Dictionary<Vector3Int, TempLevelTileData>();

        private Dictionary<TileConnectorData, List<LevelTile>> levelTilesByConnectorData = new Dictionary<TileConnectorData, List<LevelTile>>();
        //private List<TempLevelTileData> availableTempLevelTileDatas = new List<TempLevelTileData>();

        #region GenerateTiles
        public void GenerateTiles()
        {
            if(levelTiles == null || levelTiles.Count == 0)
            {
                Debug.Log("No level tiles to spawn from.");
                return;
            }
            
            //add all the leveltiles in levelTiles to the list of available tempLevelTileDatas
            foreach(LevelTile levelTile in levelTiles)
            {
                levelTilesByConnectorData[levelTile.ConnectorData].Add(levelTile);
            }

            List<TileConnectorData> availableConnectorDatas = new List<TileConnectorData>();
            foreach(TileShape tileShape in Enum.GetValues(typeof(TileShape)))
            {
                if(levelTilesByConnectorData.ContainsKey(new TileConnectorData(tileShape)))
                    availableConnectorDatas.Add(new TileConnectorData(tileShape));
            }
            
            Queue<Vector3Int> openPositions = new Queue<Vector3Int>();
            List<Vector3Int> closedPositions = new List<Vector3Int>();

            //set the tiletype of the tile at 000 to the one with the most avilable connections out of availableConnectorDatas.
            levelTilesByGridPos[Vector3Int.zero] = new TempLevelTileData(availableConnectorDatas[availableConnectorDatas.Count - 1], Vector3Int.zero, 0);
            closedPositions.Add(Vector3Int.zero);
            
            //this loop adds all the tiles that make up the bulk of the maze until it reaches all the tiles that will end up being placed
            while(closedPositions.Count + openPositions.Count < numerOfTiles)
            {
                foreach(Vector3Int closedPosition in closedPositions)
                {
                    foreach(Vector3Int connectorPosition in levelTilesByGridPos[closedPosition].ConnectorPositionsOnGrid())
                    {
                        //if the connectorPosition is not already in the list of open or closed positions, add it to the open positions lis
                        if(!openPositions.Contains(connectorPosition) && !closedPositions.Contains(connectorPosition))
                        {
                            openPositions.Enqueue(connectorPosition);
                        }
                    }    
                }
                
                Vector3Int positionToFill = openPositions.Dequeue();
                //add all available connector datas except for the first one (which should be the cap) to this new list
                List<TileConnectorData> tileDatasToTryPlaceInOrder = availableConnectorDatas.GetRange(1, availableConnectorDatas.Count - 1);
                //shuffle the list so that it will try to place down a random tile in the given position
                tileDatasToTryPlaceInOrder = tileDatasToTryPlaceInOrder.OrderBy(x => Guid.NewGuid()).ToList();
                //add a cap tile at the end so that if no other tile can be placed here, a cap tile can be.
                tileDatasToTryPlaceInOrder.Add(availableConnectorDatas[0]);

                //initialise the new tile that will be placed.
                TempLevelTileData newTile = new TempLevelTileData(new TileConnectorData(TileShape.FourWay), positionToFill, 0);
                //this is the loop that runs over all the all the tile types and rotations until it finds one that fits in the open space.
                foreach(TileConnectorData tileConnectorData in tileDatasToTryPlaceInOrder)
                {
                    bool tilePlaced = false;
                    //set the connector data to tileConnectorData
                    newTile.connectorData = tileConnectorData;
                    //give the tile rotation a random offset to eliminate too much of the same rotation in tile placement.
                    int randomRotOffset = Random.Range(0, 4);
                    for(int j = 0; j < 4; j++)
                    {
                        newTile.quaterRots = (randomRotOffset + j) % 4;
                        if(newTile.TileFitsInPosition(levelTilesByGridPos))
                        {
                            //set the tile at positionToFill to newTile
                            levelTilesByGridPos[positionToFill] = newTile;
                            //add the new tile's position to the list of closed positions.
                            closedPositions.Add(positionToFill);
                            //when the tile is placed, set this bool to true so that the foreach loop will not need to continue
                            tilePlaced = true;
                        }
                        else
                            continue;
                        break;
                    }

                    //if the tile has been placed, break this loop
                    if(tilePlaced)
                        break;
                }
            }
            
            //this loop closes off all the open tiles left by the last loop
            while(openPositions.Count > numerOfTiles)
            {
                Vector3Int positionToFill = openPositions.Dequeue();
                //add all available connector datas in order of the number of connectors each one has
                List<TileConnectorData> tileDatasToTryPlaceInOrder = availableConnectorDatas.OrderBy(x => x.DefaultConnectorPositions()).ToList();

                //initialise the new tile that will be placed.
                TempLevelTileData newTile = new TempLevelTileData(new TileConnectorData(TileShape.Cap), positionToFill, 0);
                //this is the loop that runs over all the all the tile types and rotations until it finds one that fits in the open space.
                foreach(TileConnectorData tileConnectorData in tileDatasToTryPlaceInOrder)
                {
                    bool tilePlaced = false;
                    //set the connector data to tileConnectorData
                    newTile.connectorData = tileConnectorData;
                    //give the tile rotation a random offset to eliminate too much of the same rotation in tile placement.
                    int randomRotOffset = Random.Range(0, 4);
                    for(int j = 0; j < 4; j++)
                    {
                        newTile.quaterRots = (randomRotOffset + j) % 4;
                        if(newTile.TileFitsInPositionWithNoEmptyConnectors(levelTilesByGridPos))
                        {
                            //set the tile at positionToFill to newTile
                            levelTilesByGridPos[positionToFill] = newTile;
                            //add the new tile's position to the list of closed positions.
                            closedPositions.Add(positionToFill);
                            //when the tile is placed, set this bool to true so that the foreach loop will not need to continue
                            tilePlaced = true;
                        }
                        else
                            continue;
                        break;
                    }

                    //if the tile has been placed, break this loop
                    if(tilePlaced)
                        break;
                }
            }
        }
    #endregion
    }    
}