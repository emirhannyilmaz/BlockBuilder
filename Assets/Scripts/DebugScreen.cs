using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour {
    World world;
    Text text;

    float fps;
    float timer;

    int halfWorldSizeInVoxels;
    int halfWorldSizeInChunks;

    void Start() {
        world = GameObject.Find("World").GetComponent<World>();
        text = GetComponent<Text>();

        halfWorldSizeInVoxels = VoxelData.WorldSizeInVoxels / 2;
        halfWorldSizeInChunks = VoxelData.WorldSizeInChunks / 2;
    }

    void Update() {
        string debugText = "FPS: " + fps;
        debugText += "\n";
        debugText += "Player Position: " + (Mathf.FloorToInt(world.player.position.x) - halfWorldSizeInVoxels) + "/" + Mathf.FloorToInt(world.player.position.y) + "/" + (Mathf.FloorToInt(world.player.position.z) - halfWorldSizeInVoxels);
        debugText += "\n";
        debugText += "Player Chunk Position: " + (world.playerChunkCoord.x - halfWorldSizeInChunks) + "/" + (world.playerChunkCoord.z - halfWorldSizeInChunks);

        text.text = debugText;

        if(timer > 1f) {
            fps = (int) (1f / Time.unscaledDeltaTime);
            timer = 0f;
        } else
            timer += Time.deltaTime;
    }
}
