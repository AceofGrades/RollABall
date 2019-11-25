using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    public GameObject bombPrefab;

    public Camera attachedCamera;
    public Transform attachedVirtualCamera;

    public float speed = 10f, jump = 10f;
    public LayerMask ignoreLayers;
    public float rayDistance = 10f;
    public bool isGrounded = false;
    private Rigidbody rigid;
    #region Unity Events
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Sets gizmos colour to red
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);
    }

    private void OnDestroy()
    {
        Destroy(attachedCamera.gameObject); // Destroys camera
        Destroy(attachedVirtualCamera.gameObject); // Destroys virtual camera
    }
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();

        attachedCamera.transform.SetParent(null);
        attachedVirtualCamera.SetParent(null);

        if (isLocalPlayer) // If local player is there
        {
            attachedCamera.enabled = true; // Camera is enabled
            attachedVirtualCamera.gameObject.SetActive(true); // Virtual camera is active
        }
        else
        {
            attachedCamera.enabled = false; // Camera is disabled
            attachedVirtualCamera.gameObject.SetActive(false); // Virtual camera is not active
        }
    }
    private void FixedUpdate()
    {
        Ray groundRay = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.Raycast(groundRay, rayDistance, ~ignoreLayers);
    }
    private void OnTriggerEnter(Collider col)
    {
        Item item = col.GetComponent<Item>();
        if (item)
        {
            item.Collect(); // Collects item, if the object is labeled as such
        }
    }
    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                // Spawn bomb on server
                CmdSpawnBomb(transform.position);
            }

            float inputH = Input.GetAxis("Horizontal");
            float inputV = Input.GetAxis("Vertical");
            Move(inputH, inputV);
            if (Input.GetButtonDown("Jump"))
            {
                Jump(); // Player jumps
            }
        }
    }
    #endregion
    #region Commands
    [Command]
    public void CmdSpawnBomb(Vector3 position)
    {
        GameObject bomb = Instantiate(bombPrefab, position, Quaternion.identity); // Instantiates the bomb gameObject
        NetworkServer.Spawn(bomb); // Server spawns bomb
    }
    #endregion
    #region Custom
    private void Jump()
    {
        if (isGrounded)
        {
            rigid.AddForce(Vector3.up * jump, ForceMode.Impulse); // Calls for player to Jump ONLY if isGrounded
        }
    }
    private void Move(float inputH, float inputV)
    {
        Vector3 direction = new Vector3(inputH, 0, inputV);

        // [Optional] Move with camera
        Vector3 euler = Camera.main.transform.eulerAngles;
        direction = Quaternion.Euler(0, euler.y, 0) * direction; // Convert direction to relative direction to camera only on Y

        rigid.AddForce(direction * speed);
    }
    #endregion
}

