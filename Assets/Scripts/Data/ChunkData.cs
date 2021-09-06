using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData {
    int x;
    int y;

    public Vector2Int position {
        get {
            return new Vector2Int(Mathf.FloorToInt(x / VoxelData.ChunkWidth), Mathf.FloorToInt(y / VoxelData.ChunkWidth));
        }
    }

    public Vector2Int globalPosition {
        get {
            return new Vector2Int(x, y);
        }

        set {
            x = value.x;
            y = value.y;
        }
    }

    public ChunkData(Vector2Int pos) {
        globalPosition = pos;
    }

    public ChunkData(int _x, int _y) {
        x = _x;
        y = _y;
    }

    [HideInInspector]
    public VoxelState[,,] map = new VoxelState[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    public void Populate() {
        for(int y = 0; y < VoxelData.ChunkHeight; y++) {
            for(int x = 0; x < VoxelData.ChunkWidth; x++) {
                for(int z = 0; z < VoxelData.ChunkWidth; z++) {
                    map[x, y, z] = new VoxelState(World.Instance.GetVoxel(new Vector3(x + globalPosition.x, y, z + globalPosition.y)));
                }
            }
        }

        World.Instance.worldData.AddToModifiedChunksList(this);
    }
}
