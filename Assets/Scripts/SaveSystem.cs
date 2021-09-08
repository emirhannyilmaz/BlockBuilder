using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem {
    public static void SaveWorld(WorldData worldData) {
        string savePath = TitleMenu.appPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + worldData.worldName + Path.DirectorySeparatorChar;

        if(!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        worldData.playerX = World.Instance.player.position.x;
        worldData.playerY = World.Instance.player.position.y;
        worldData.playerZ = World.Instance.player.position.z;
        worldData.playerRotY = World.Instance.player.rotation.eulerAngles.y;
        worldData.cameraRotX = Camera.main.transform.localRotation.eulerAngles.x;

        using(var w = new BinaryWriter(File.OpenWrite(savePath + "world.world"))) {
            w.Write(System.Convert.ToString(worldData.worldName));
            w.Write(System.Convert.ToInt16(worldData.seed));
            w.Write(System.Convert.ToSingle(worldData.playerX));
            w.Write(System.Convert.ToSingle(worldData.playerY));
            w.Write(System.Convert.ToSingle(worldData.playerZ));
            w.Write(System.Convert.ToSingle(worldData.playerRotY));
            w.Write(System.Convert.ToSingle(worldData.cameraRotX));
        }

        List<ChunkData> chunks = new List<ChunkData>(worldData.modifiedChunks);
        worldData.modifiedChunks.Clear();

        foreach(ChunkData chunk in chunks) {
            SaveChunk(chunk, worldData.worldName);
        }
    }

    public static WorldData LoadWorld(string worldName) {
        string loadPath = TitleMenu.appPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + worldName + Path.DirectorySeparatorChar;

        if(File.Exists(loadPath + "world.world")) {
            using(var r = new BinaryReader(File.OpenRead(loadPath + "world.world"))) {
                string name = r.ReadString();
                int seed = r.ReadInt16();
                float playerX = r.ReadSingle();
                float playerY = r.ReadSingle();
                float playerZ = r.ReadSingle();
                float playerRotY = r.ReadSingle();
                float cameraRotX = r.ReadSingle();

                return new WorldData(name, seed, playerX, playerY, playerZ, playerRotY, cameraRotX);
            }
        } else {
            WorldData worldData = new WorldData(worldName);
            return worldData;
        }
    }

    public static void SaveChunk(ChunkData chunk, string worldName) {
        string chunkName = chunk.position.x + "-" + chunk.position.y;
        string savePath = TitleMenu.appPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + worldName + Path.DirectorySeparatorChar + "chunks" + Path.DirectorySeparatorChar;

        if(!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        using(var w = new BinaryWriter(File.OpenWrite(savePath + chunkName + ".chunk"))) {
            w.Write(System.Convert.ToInt16(chunk.globalPosition.x));
            w.Write(System.Convert.ToInt16(chunk.globalPosition.y));
            for(int y = 0; y < VoxelData.ChunkHeight; y++) {
                for(int x = 0; x < VoxelData.ChunkWidth; x++) {
                    for(int z = 0; z < VoxelData.ChunkWidth; z++) {
                        w.Write(System.Convert.ToByte(chunk.map[x, y, z].id));
                    }
                }
            }
        }
    }

    public static ChunkData LoadChunk(string worldName, Vector2Int position) {
        string chunkName = position.x + "-" + position.y;
        string loadPath = TitleMenu.appPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + worldName + Path.DirectorySeparatorChar + "chunks" + Path.DirectorySeparatorChar;

        if(File.Exists(loadPath + chunkName + ".chunk")) {
            using(var r = new BinaryReader(File.OpenRead(loadPath + chunkName + ".chunk"))) {
                int cX = r.ReadInt16();
                int cY = r.ReadInt16();

                ChunkData chunkData = new ChunkData(cX, cY);

                for(int y = 0; y < VoxelData.ChunkHeight; y++) {
                    for(int x = 0; x < VoxelData.ChunkWidth; x++) {
                        for(int z = 0; z < VoxelData.ChunkWidth; z++) {
                            chunkData.map[x, y, z] = new VoxelState(r.ReadByte());
                        }
                    }
                }

                return chunkData;
            }
        }

        return null;
    }

    public static void ResetWorld(string worldName) {
        string worldPath = TitleMenu.appPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + worldName + Path.DirectorySeparatorChar;

        if(Directory.Exists(worldPath)) {
            Directory.Delete(worldPath, true);
        }
    }

    public static bool CheckIfWorldExists(string worldName) {
        string worldPath = TitleMenu.appPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + worldName + Path.DirectorySeparatorChar;

        if(Directory.Exists(worldPath)) {
            return true;
        } else {
            return false;
        }
    }
}