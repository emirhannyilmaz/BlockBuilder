using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {
    public static void SaveWorld(WorldData worldData) {
        string savePath = World.Instance.appPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + worldData.worldName + Path.DirectorySeparatorChar;

        if(!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + "world.world", FileMode.Create);

        worldData.playerX = World.Instance.player.position.x;
        worldData.playerY = World.Instance.player.position.y;
        worldData.playerZ = World.Instance.player.position.z;
        worldData.playerRotY = World.Instance.player.rotation.eulerAngles.y;
        worldData.cameraRotX = Camera.main.transform.localRotation.eulerAngles.x;

        formatter.Serialize(stream, worldData);
        stream.Close();

        List<ChunkData> chunks = new List<ChunkData>(worldData.modifiedChunks);
        worldData.modifiedChunks.Clear();

        foreach(ChunkData chunk in chunks) {
            SaveChunk(chunk, worldData.worldName);
        }
    }

    public static WorldData LoadWorld(string worldName) {
        string loadPath = World.Instance.appPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + worldName + Path.DirectorySeparatorChar;

        if(File.Exists(loadPath + "world.world")) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath + "world.world", FileMode.Open);

            WorldData worldData = formatter.Deserialize(stream) as WorldData;
            stream.Close();

            return new WorldData(worldData);
        } else {
            WorldData worldData = new WorldData(worldName);
            return worldData;
        }
    }

    public static void SaveChunk(ChunkData chunk, string worldName) {
        string chunkName = chunk.position.x + "-" + chunk.position.y;
        string savePath = World.Instance.appPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + worldName + Path.DirectorySeparatorChar + "chunks" + Path.DirectorySeparatorChar;

        if(!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + chunkName + ".chunk", FileMode.Create);

        formatter.Serialize(stream, chunk);
        stream.Close();
    }

    public static ChunkData LoadChunk(string worldName, Vector2Int position) {
        string chunkName = position.x + "-" + position.y;
        string loadPath = World.Instance.appPath + Path.DirectorySeparatorChar + "saves" + Path.DirectorySeparatorChar + worldName + Path.DirectorySeparatorChar + "chunks" + Path.DirectorySeparatorChar;

        if(File.Exists(loadPath + chunkName + ".chunk")) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath + chunkName + ".chunk", FileMode.Open);

            ChunkData chunkData = formatter.Deserialize(stream) as ChunkData;
            stream.Close();

            return chunkData;
        }

        return null;
    }
}
