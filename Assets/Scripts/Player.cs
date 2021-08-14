using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float walkSpeed = 3f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;
    public float playerWidth = 0.15f;
    public bool isGrounded;

    private Transform cam;
    private World world;
    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0f;

    public Transform highlightBlock;
    public Transform placeHighlightBlock;
    public float checkIncrement = 0.1f;
    public float reach = 8f;

    public byte selectedBlockIndex = 1;

    float pressedTime;
    bool isPressedDown = false;

    private void Start() {
        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();
    }

    private void Update() {
        GetPlayerInputs();
        PlaceCursorBlocks();
    }

    private void FixedUpdate() {
        CalculateVelocity();

        transform.Rotate(Vector3.up * mouseHorizontal * world.settings.sensitivity);
        cam.Rotate(Vector3.right * -mouseVertical * world.settings.sensitivity);
        transform.Translate(velocity, Space.World);
    }

    public void Jump() {
        if(isGrounded) {
            verticalMomentum = jumpForce;
            isGrounded = false;
        }
    }

    private void CalculateVelocity() {
        if(verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0f;
        if((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0f;

        if(velocity.y < 0)
            velocity.y = CheckDownSpeed(velocity.y);
        else if(velocity.y > 0)
            velocity.y = CheckUpSpeed(velocity.y);
    }

    private void GetPlayerInputs() {
        horizontal = SimpleInput.GetAxis("Horizontal");
        vertical = SimpleInput.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if(highlightBlock.gameObject.activeSelf) {
            if(Input.GetMouseButtonDown(0)) {
                pressedTime = Time.time;
                isPressedDown = true;
            }

            if(Input.GetMouseButton(0) && Time.time - pressedTime > 0.3f && isPressedDown) {
                world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);
                isPressedDown = false;
            }

            if(Input.GetMouseButtonDown(0))
                world.GetChunkFromVector3(placeHighlightBlock.position).EditVoxel(placeHighlightBlock.position, selectedBlockIndex);
        }
    }

    private void PlaceCursorBlocks() {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while(step < reach) {
            Vector3 pos = cam.position + (Camera.main.ScreenPointToRay(Input.mousePosition).direction * step);

            if(world.CheckForVoxel(pos)) {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeHighlightBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeHighlightBlock.gameObject.SetActive(true);

                return;
            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

            step += checkIncrement;
        }

        highlightBlock.gameObject.SetActive(false);
        placeHighlightBlock.gameObject.SetActive(false);
    }

    private float CheckDownSpeed(float downSpeed) {
        if(world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
           world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
           world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
           world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth))) {
            isGrounded = true;
            return 0;
        } else {
            isGrounded = false;
            return downSpeed;
        }
    }

    private float CheckUpSpeed(float upSpeed) {
        if(world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
           world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
           world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)) ||
           world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)))
            return 0;
        else
            return upSpeed;
    }

    public bool front {
        get {
            if(world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
               world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth)))
                return true;
            else
                return false;
        }
    }

    public bool back {
        get {
            if(world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
               world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth)))
                return true;
            else
                return false;
        }
    }

    public bool left {
        get {
            if(world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
               world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z)))
                return true;
            else
                return false;
        }
    }

    public bool right {
        get {
            if(world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
               world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z)))
                return true;
            else
                return false;
        }
    }
}
