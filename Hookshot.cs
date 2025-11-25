using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class Hookshot : MonoBehaviour
{
    [SerializeField] private float grappleLength; 
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer rope;
    [SerializeField] private LineRenderer aimRope;
    [SerializeField] private float maxDistance;

    private Transform ConnectedPlatformLocation;

    Color fadedGreen = new Color(0, 1, 0, 0.5f);
    Color fadedRed = new Color(1, 0, 0, 0.5f);


    private Vector3 grapplePoint;
    private DistanceJoint2D joint;

    // Start is called before the first frame update
    void Start()
    {
     joint = gameObject.GetComponent<DistanceJoint2D>();
     joint.enabled = false;
     rope.enabled = false;
     aimRope.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        
        Vector3 currentPosition = transform.position;
        Vector3 direction = (new Vector3(currentPosition.x + x, currentPosition.y + y, 0) - currentPosition).normalized;
        
        RaycastHit2D hit = Physics2D.Raycast(//casts a ray, returning a value if it hits a valid target on the grapple layer
        origin: currentPosition,
        direction: direction,
        distance: maxDistance,
        layerMask: grappleLayer); 

        //draws reticle for aiming
        aimRope.SetPosition(0, currentPosition); 
        aimRope.SetPosition(1, currentPosition + direction);

        //Changes colour of aiming reticle if pointing at  valid target
        if (hit.collider != null)
        {
            aimRope.startColor = fadedGreen;
            aimRope.endColor = fadedGreen;            
        }
        else
        {
            aimRope.startColor = fadedRed;
            aimRope.endColor = fadedRed;
        }  

        if (Input.GetButtonDown("Fire1"))
        { //activates on left shift and left mouse button down                   
            if (hit.collider != null) //must be valid target
            {
                if (hit.collider.name == "Platform") //if ray hits platform object link to platform's rigidbody
                {
                    joint.connectedAnchor = Vector2.zero; 
                    joint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();                    
                    joint.enabled = true;
                    joint.distance = grappleLength;
                    rope.enabled = true;

                    ConnectedPlatformLocation = hit.collider.gameObject.GetComponent<Transform>();                                        
                }
                else //if connected to wall link player to coordinates where ray hit
                {
                    joint.connectedBody = null;
                    grapplePoint = hit.point;
                    grapplePoint.z = 0;
                    joint.connectedAnchor = grapplePoint;
                    joint.enabled = true;
                    joint.distance = grappleLength;
                    rope.enabled = true;
                }      
            }
        }

        if(Input.GetButtonUp("Fire1")) //activates on left shift and left mouse button release
        {            
            joint.enabled = false;
            rope.enabled = false;
            joint.connectedBody = null;
        } //releases grapple

        if (rope.enabled == true) //updates rope renderer
        {
            if (joint.connectedBody != null)
            {
                //joint.distance = grappleLength;
                rope.SetPosition(0, new Vector3(ConnectedPlatformLocation.position.x, ConnectedPlatformLocation.position.y, 0));
            }
            else
            {
                rope.SetPosition(0, grapplePoint);
            }                       
            rope.SetPosition(1, transform.position);
        }
    }
}
