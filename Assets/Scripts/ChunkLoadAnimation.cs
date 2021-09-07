using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIRECTION {
    UP,
    DOWN
};

public class ChunkLoadAnimation : MonoBehaviour {
    float speed = 3f;
    Vector3 targetPosUp;
    Vector3 targetPosDown;

    float waitTimer;
    float timer;

    private DIRECTION _direction;

    void Start() {
        waitTimer = Random.Range(0f, 3f);
        targetPosUp = transform.position;
        targetPosDown = transform.position - new Vector3(0f, VoxelData.ChunkHeight, 0f);
        transform.position = new Vector3(transform.position.x, -VoxelData.ChunkHeight, transform.position.z);
    }

    public DIRECTION direction {
        get {
            return _direction;
        }
        set {
            _direction = value;
            if(value == DIRECTION.UP) {
                transform.gameObject.SetActive(true);
            }
        }
    }

    void Update() {
        if(direction == DIRECTION.UP && transform.position != targetPosUp) {
            if(timer < waitTimer) {
                timer += Time.deltaTime;
            } else {
                transform.position = Vector3.Lerp(transform.position, targetPosUp, Time.deltaTime * speed);
                if((targetPosUp.y - transform.position.y) < 0.05f) {
                    transform.position = targetPosUp;
                    timer = 0f;
                }
            }
        } else if(direction == DIRECTION.DOWN && transform.position != targetPosDown) {
            if(timer < waitTimer) {
                timer += Time.deltaTime;
            } else {
                transform.position = Vector3.Lerp(transform.position, targetPosDown, Time.deltaTime * speed);
                if((targetPosDown.y - transform.position.y) > -0.05f) {
                    transform.position = targetPosDown;
                    timer = 0f;
                    transform.gameObject.SetActive(false);
                }
            }
        }
    }
}
