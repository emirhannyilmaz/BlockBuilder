using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {
    public float walkSpeed = 3f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;
    public float playerWidth = 0.15f;
    public bool isGrounded;

    private Camera cam;
    public float camXRotation = 0f;
    private World world;
    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0f;
    private bool jumpRequest = false;

    public Transform highlightBlock;
    public Transform placeHighlightBlock;
    public float checkIncrement = 0.1f;
    public float reach = 8f;

    public byte selectedBlockIndex = 1;

    float pressedTime;
    bool isPressedDown = false;

    private Touch currentPointer;
    private Vector3 highlightBlockStart;
    private bool destroyingMode = false;

    private void Start() {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        world = GameObject.Find("World").GetComponent<World>();
    }

    private void Update() {
        GetPlayerInputs();
    }

    private void FixedUpdate() {
        CalculateVelocity();

        if(jumpRequest)
            Jump();

        transform.Rotate((mouseHorizontal * 0.1f) * world.settings.sensitivity * Vector3.up);
        camXRotation -= (mouseVertical * 0.1f) * world.settings.sensitivity;
        camXRotation = Mathf.Clamp(camXRotation, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(camXRotation, 0f, 0f);
        transform.Translate(velocity, Space.World);
    }

    private void Jump() {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }

    public void RequestJump() {
        if(isGrounded)
            jumpRequest = true;
    }

    private void CalculateVelocity() {
        if(verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        velocity = Time.fixedDeltaTime * walkSpeed * ((transform.forward * vertical) + (transform.right * horizontal));

        velocity += Time.fixedDeltaTime * verticalMomentum * Vector3.up;

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

        if(Input.touchCount > 0) {
            foreach(Touch t in Input.touches) {
                if(!IsPointerOverUIObject(t)) {
                    if(t.phase == TouchPhase.Began) {
                        currentPointer = t;
                        PlaceCursorBlocks();

                        if(highlightBlock.gameObject.activeSelf) {
                            isPressedDown = true;
                            pressedTime = Time.time;
                            highlightBlockStart = highlightBlock.position;
                        }
                    }

                    if(t.phase == TouchPhase.Moved) {
                        mouseHorizontal = t.deltaPosition.x;
                        mouseVertical = t.deltaPosition.y;
                        PlaceCursorBlocks();
                    }

                    if(t.phase == TouchPhase.Stationary) {
                        if(highlightBlock.gameObject.activeSelf) {
                            if(Time.time - pressedTime >= 0.5f && isPressedDown && (highlightBlockStart == highlightBlock.position || destroyingMode)) {
                                world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);
                                destroyingMode = true;
                                pressedTime = Time.time;
                            }
                        }

                        mouseHorizontal = 0f;
                        mouseVertical = 0f;
                    }

                    if(t.phase == TouchPhase.Ended) {
                        if(highlightBlock.gameObject.activeSelf) {
                            if(isPressedDown) {
                                if(!destroyingMode && highlightBlockStart == highlightBlock.position) {
                                    world.GetChunkFromVector3(placeHighlightBlock.position).EditVoxel(placeHighlightBlock.position, selectedBlockIndex);
                                    isPressedDown = false;
                                    PlaceCursorBlocks();
                                } else if(destroyingMode) {
                                    isPressedDown = false;
                                    destroyingMode = false;
                                    PlaceCursorBlocks();
                                }
                            }
                        }

                        mouseHorizontal = 0f;
                        mouseVertical = 0f;
                    }
                } else {
                    mouseHorizontal = 0f;
                    mouseVertical = 0f;
                }
            }
        }
    }

    private void PlaceCursorBlocks() {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while(step < reach) {
            Vector3 pos = cam.transform.position + (cam.ScreenPointToRay(currentPointer.position).direction * step);

            if(world.CheckForVoxel(pos)) {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeHighlightBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);

                return;
            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

            step += checkIncrement;
        }

        highlightBlock.gameObject.SetActive(false);
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

    bool IsPointerOverUIObject(Touch touch) {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(touch.position.x, touch.position.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
