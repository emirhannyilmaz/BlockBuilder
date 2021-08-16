using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {
    public static void SaveWorld(WorldData worldData) {
        string savePath = World.Instance.appPath + "/saves/" + worldData.worldName + "/";

        if(!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + "world.world", FileMode.Create);

        formatter.Serialize(stream, worldData);
        stream.Close();

        List<ChunkData> chunks = new List<ChunkData>(worldData.modifiedChunks);
        worldData.modifiedChunks.Clear();

        foreach(ChunkData chunk in chunks) {
            SaveChunk(chunk, worldData.worldName);
        }
    }

    public static WorldData LoadWorld(string worldName, int seed = 0) {
        string loadPath = World.Instance.appPath + "/saves/" + worldName + "/";

        if(File.Exists(loadPath + "world.world")) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath + "world.world", FileMode.Open);

            WorldData worldData = formatter.Deserialize(stream) as WorldData;
            stream.Close();

            return new WorldData(worldData);
        } else {
            WorldData worldData = new WorldData(worldName, seed);
            SaveWorld(worldData);
            return worldData;
        }
    }

    public static void SaveChunk(ChunkData chunk, string worldName) {
        string chunkName = chunk.position.x + "-" + chunk.position.y;
        string savePath = World.Instance.appPath + "/saves/" + worldName + "/chunks/";

        if(!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + chunkName + ".chunk", FileMode.Create);

        formatter.Serialize(stream, chunk);
        stream.Close();
    }

    public static ChunkData LoadChunk(string worldName, Vector2Int position) {
        string chunkName = position.x + "-" + position.y;
        string loadPath = World.Instance.appPath + "/saves/" + worldName + "/chunks/";

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
