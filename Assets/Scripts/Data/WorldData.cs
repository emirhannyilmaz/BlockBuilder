using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData {
    public string worldName = "New World";
    public int seed;
    public float playerX;
    public float playerY;
    public float playerZ;
    public float playerRotY;
    public float cameraRotX;

    [System.NonSerialized]
    public Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();

    [System.NonSerialized]
    public List<ChunkData> modifiedChunks = new List<ChunkData>();

    public void AddToModifiedChunksList(ChunkData chunkData) {
        if(!modifiedChunks.Contains(chunkData))
            modifiedChunks.Add(chunkData);
    }

    public WorldData(string _worldName) {
        worldName = _worldName;
        seed = Random.Range(1234, 9876);
        playerX = (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f;
        playerY = VoxelData.ChunkHeight - 50f;
        playerZ = (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f;
    }

    public WorldData(WorldData worldData) {
        worldName = worldData.worldName;
        seed = worldData.seed;
        playerX = worldData.playerX;
        playerY = worldData.playerY;
        playerZ = worldData.playerZ;
        playerRotY = worldData.playerRotY;
        cameraRotX = worldData.cameraRotX;
    }

    public ChunkData RequestChunk(Vector2Int coord, bool create) {
        ChunkData c;

        lock(World.Instance.ChunkListThreadLock) {
            if(chunks.ContainsKey(coord))
                c = chunks[coord];
            else if(!create)
                c = null;
            else {
                LoadChunk(coord);
                c = chunks[coord];
            }
        }

        return c;

    }

    public void LoadChunk(Vector2Int coord) {
        if(chunks.ContainsKey(coord))
            return;

        ChunkData chunk = SaveSystem.LoadChunk(worldName, coord);
        if(chunk != null) {
            chunks.Add(coord, chunk);
            return;
        }

        chunks.Add(coord, new ChunkData(coord.x * VoxelData.ChunkWidth, coord.y * VoxelData.ChunkWidth));
        chunks[coord].Populate();
    }

    bool IsVoxelInWorld(Vector3 pos) {
        if(pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
            return true;
        else
            return false;
    }

    public void SetVoxel(Vector3 pos, byte value) {
        if(!IsVoxelInWorld(pos))
            return;

        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);

        x *= VoxelData.ChunkWidth;
        z *= VoxelData.ChunkWidth;

        Vector3Int voxel = new Vector3Int((int) (pos.x - x), (int) pos.y, (int) (pos.z - z));

        chunk.map[voxel.x, voxel.y, voxel.z].id = value;
        AddToModifiedChunksList(chunk);
    }

    public VoxelState GetVoxel(Vector3 pos) {
        if(!IsVoxelInWorld(pos))
            return null;

        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);

        x *= VoxelData.ChunkWidth;
        z *= VoxelData.ChunkWidth;

        Vector3Int voxel = new Vector3Int((int) (pos.x - x), (int) pos.y, (int) (pos.z - z));

        return chunk.map[voxel.x, voxel.y, voxel.z];
    }
}
